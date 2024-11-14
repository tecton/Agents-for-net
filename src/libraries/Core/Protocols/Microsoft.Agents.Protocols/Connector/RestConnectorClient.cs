// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Core.Pipeline;
using Microsoft.Agents.Authentication;
using Microsoft.Agents.Protocols.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Agents.Protocols.Connector
{
    /// <summary>
    /// The Bot Connector REST API allows your bot to send and receive messages to channels configured in the Azure Bot Service.
    /// The Connector service uses industry-standard REST and JSON over HTTPS.
    /// </summary>
    public class RestConnectorClient : IConnectorClient
    {
        private readonly Uri _endpoint;
        private readonly IList<string> _scopes;

        protected readonly HttpPipeline _pipeline;

        public IAttachments Attachments { get; }

        public IConversations Conversations { get; }

        public Uri BaseUri => _endpoint;

        public RestConnectorClient(Uri endpoint, IAccessTokenProvider tokenAccess, string resource, IList<string> scopes = null, bool useAnonymousConnection = false)
            : this(endpoint, tokenAccess, resource, scopes, new ConnectorClientOptions(), useAnonymousConnection)
        {
        }

        public RestConnectorClient(Uri endpoint, IAccessTokenProvider tokenAccess, string resource, IList<string> scopes, ConnectorClientOptions options, bool useAnonymousConnection = false)
        {
            _endpoint = endpoint;
            _scopes = scopes ?? [];

            // Build pipeline policy list
            var perRetryPolicies = new List<HttpPipelinePolicy>
            {
                new DefaultHeadersPolicy(options)
            };
            if (!useAnonymousConnection) 
            {
                perRetryPolicies.Add(new BearerTokenPolicy(tokenAccess, resource, _scopes));
            }

            // Manually create a pipeline using bearer tokens shared between Attachments and Conversations clients.
            _pipeline = HttpPipelineBuilder.Build(options, [.. perRetryPolicies]);

            Conversations = new ConversationsRestClient(_pipeline, endpoint);     
            Attachments = new AttachmentsRestClient(_pipeline, endpoint);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    // Adds header values to Connector requests.  This also handles setting the builtin User-Agent value, and allows
    // for a custom User-Agent addition.
    internal class DefaultHeadersPolicy : HttpPipelinePolicy
    {
        private readonly string _userAgent;
        private readonly IDictionary<string, string> _headers;

        /// <summary>
        /// The product info header value for the SDK. Stored as Static for performance.
        /// </summary>
        private static ProductInfoHeaderValue _frameworkProductInfo = null;
        /// <summary>
        /// SDK Version String. Stored as Static for performance.
        /// </summary>
        private static ProductInfoHeaderValue _versionString = null;

        public DefaultHeadersPolicy(ConnectorClientOptions options)
        {
            _userAgent = BuildUserAgent(options);
            _headers = options.DefaultHeaders;
        }

        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            throw new NotImplementedException();
        }

        public async override ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            if (!string.IsNullOrEmpty(_userAgent))
            {
                message.Request.Headers.SetValue(HttpHeader.Names.UserAgent, _userAgent);
            }

            if (_headers != null)
            {
                foreach (var header in _headers)
                {
                    if (!header.Key.Equals(HttpHeader.Names.UserAgent, StringComparison.OrdinalIgnoreCase))
                    {
                        message.Request.Headers.SetValue(header.Key, header.Value);
                    }
                }
            }

            await ProcessNextAsync(message, pipeline).ConfigureAwait(continueOnCapturedContext: false);
        }

        private string BuildUserAgent(ConnectorClientOptions options)
        {
            var userAgent = new List<ProductInfoHeaderValue>();

            userAgent.Add(new ProductInfoHeaderValue("Microsoft-Agents", "1.0"));
            userAgent.Add(GetClientProductInfo());

            var framework = GetFrameworkProductInfo();
            if (framework != null)
            {
                userAgent.Add(framework);
            }

            if (_headers != null)
            {
                foreach (var header in _headers)
                {
                    if (header.Key.Equals(HttpHeader.Names.UserAgent, StringComparison.OrdinalIgnoreCase))
                    {
                        if (ProductInfoHeaderValue.TryParse(header.Value, out ProductInfoHeaderValue customUserAgent))
                        {
                            userAgent.Add(customUserAgent);
                        }
                    }
                }
            }

            var userAgentValue = string.Empty;
            foreach (var productInfo in userAgent)
            {
                if (string.IsNullOrEmpty(userAgentValue))
                {
                    userAgentValue = productInfo.ToString();
                }
                else
                {
                    userAgentValue += " " + productInfo.ToString();
                }
            }
            return userAgentValue;
        }

        private ProductInfoHeaderValue GetClientProductInfo()
        {
            if (_versionString != null)
            {
                return _versionString;
            }

            var type = typeof(RestConnectorClient);
            var assembly = type.GetTypeInfo().Assembly;

            var version = assembly.GetName().Version.ToString();

            _versionString = new ProductInfoHeaderValue("Microsoft-Agent-SDK", version);
            return _versionString;
        }

        private static readonly Regex FrameworkRegEx = new Regex(@"(?:(\d+)\.)?(?:(\d+)\.)?(?:(\d+)\.\d+)", RegexOptions.Compiled);

        private ProductInfoHeaderValue GetFrameworkProductInfo()
        {
            if (_frameworkProductInfo != null)
                return _frameworkProductInfo;

            var frameworkName = Assembly
                    .GetEntryAssembly()?
                    .GetCustomAttribute<TargetFrameworkAttribute>()?
                    .FrameworkName ?? RuntimeInformation.FrameworkDescription;

            if (!string.IsNullOrWhiteSpace(frameworkName) && frameworkName.Length > 0)
            {
                // from:
                // .NETCoreApp,Version=v3.1
                // to:
                // .NETCoreAppVersion/v3.1

                // from:
                // .NET Framework 4.8.4250.0
                // to:
                // .NETFramework/4.8.4250.0

                var splitFramework = frameworkName.Replace(",", string.Empty).Replace(" ", string.Empty).Split('=');
                if (splitFramework.Length > 1)
                {
                    ProductInfoHeaderValue.TryParse($"{splitFramework[0]}/{splitFramework[1]}", out _frameworkProductInfo);
                }
                else if (splitFramework.Length > 0)
                {
                    frameworkName = splitFramework[0];

                    // Parse the version from the framework string.
                    var version = FrameworkRegEx.Match(frameworkName);

                    if (version.Success)
                    {
                        frameworkName = frameworkName.Replace(version.Value, string.Empty).Trim();
                        ProductInfoHeaderValue.TryParse($"{frameworkName}/{version.Value}", out _frameworkProductInfo);
                    }
                }
            }

            return _frameworkProductInfo;
        }
    }

    // This is a wrapper around IAccessTokenProvider so that a Bearer token can be acquired.
    internal class BearerTokenPolicy : HttpPipelinePolicy
    {
        private readonly IAccessTokenProvider _tokenAccess;
        private readonly string _resource;
        private readonly IList<string> _scopes;

        public BearerTokenPolicy(IAccessTokenProvider tokenAccess, string resource, IList<string> scopes)
        {
            _tokenAccess = tokenAccess;
            _resource = resource;
            _scopes = scopes;
        }

        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            throw new NotImplementedException();
        }

        public async override ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            if (_tokenAccess != null)
            {
                var token = await _tokenAccess.GetAccessTokenAsync(_resource, _scopes).ConfigureAwait(false);
                message.Request.Headers.SetValue(HttpHeader.Names.Authorization, $"Bearer {token}");
            }
            await ProcessNextAsync(message, pipeline).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
