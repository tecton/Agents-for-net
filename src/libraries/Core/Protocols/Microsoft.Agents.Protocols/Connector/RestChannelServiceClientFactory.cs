// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Microsoft.Agents.Authentication;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Teams.Connector;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Agents.Protocols.Connector
{
    /// <summary>
    /// A factory to create REST clients to interact with a Channel Service.
    /// </summary>
    /// <remarks>
    /// Connector and UserToken client factory.
    /// </remarks>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>
    /// A factory to create REST clients to interact with a Channel Service.
    /// </remarks>
    /// <param name="connections"></param>
    /// <param name="tokenServiceEndpoint"></param>
    /// <param name="tokenServiceAudience"></param>
    /// <param name="logger"></param>
    /// <param name="customClient">For testing purposes only.</param>
    public class RestChannelServiceClientFactory(
        IConnections connections,
        string tokenServiceEndpoint = AuthenticationConstants.BotFrameworkOAuthUrl,
        string tokenServiceAudience = AuthenticationConstants.BotFrameworkScope,
        ILogger logger = null,
        HttpClient? customClient = null) : IChannelServiceClientFactory
    {
        private readonly string _tokenServiceEndpoint = tokenServiceEndpoint ?? throw new ArgumentNullException(nameof(tokenServiceEndpoint));
        private readonly string _tokenServiceAudience = tokenServiceAudience ?? throw new ArgumentNullException(nameof(tokenServiceAudience));
        private readonly HttpClient _httpClient = customClient;
        private readonly ILogger _logger = logger ?? NullLogger.Instance;
        private readonly IConnections _connections = connections ?? throw new ArgumentNullException(nameof(connections));

        /// <inheritdoc />
        public Task<IConnectorClient> CreateConnectorClientAsync(ClaimsIdentity claimsIdentity, string serviceUrl, string audience, CancellationToken cancellationToken, IList<string> scopes = null, bool UseAnonymous = false)
        {
            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentNullException(nameof(serviceUrl));
            }

            IAccessTokenProvider tokenAccess = null;
            if (!UseAnonymous)
            {
                tokenAccess = _connections.GetTokenProvider(claimsIdentity, serviceUrl)
                    ?? throw new InvalidOperationException($"An instance of IAccessTokenProvider not found for {serviceUrl}");
            }

            // Intentionally create the TeamsConnectorClient since it supports the same operations as for ABS plus the Teams operations.
            if (_httpClient == null)
            {
                return Task.FromResult<IConnectorClient>(new RestTeamsConnectorClient(new Uri(serviceUrl), tokenAccess, audience, scopes));
            }
            else
            {
                var options = new ConnectorClientOptions()
                {
                    Transport = new HttpClientTransport(_httpClient)
                };
                return Task.FromResult<IConnectorClient>(new RestTeamsConnectorClient(new Uri(serviceUrl), tokenAccess, audience, scopes, options));
            }
        }

        /// <inheritdoc />
        public Task<IUserTokenClient> CreateUserTokenClientAsync(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken, bool UseAnonymous = false)
        {
            ArgumentNullException.ThrowIfNull(claimsIdentity);

            IAccessTokenProvider tokenAccess = null;
            if (!UseAnonymous)
            {
                tokenAccess = _connections.GetTokenProvider(claimsIdentity, _tokenServiceEndpoint)
                     ?? throw new InvalidOperationException($"An instance of IAccessTokenProvider not found for {_tokenServiceEndpoint}");
            }

            //if (tokenAccess == null)
            //{
            //    throw new InvalidOperationException($"An instance of IAccessTokenProvider not found for {_tokenServiceEndpoint}");
            //}
            var appId = BotClaims.GetAppId(claimsIdentity)?? Guid.Empty.ToString();
            return Task.FromResult<IUserTokenClient>(new RestUserTokenClient(appId, new Uri(_tokenServiceEndpoint), tokenAccess, _tokenServiceAudience, null, _logger));
        }
    }
}
