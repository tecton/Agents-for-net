// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Protocols.Serializer;

namespace Microsoft.Agents.Protocols.Connector
{
    internal class BotSignInRestClient(HttpPipeline pipeline, Uri endpoint = null) : IBotSignIn
    {
        private readonly HttpPipeline _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        private readonly Uri _endpoint = endpoint ?? new Uri("");

        internal HttpMessage CreateGetSignInUrlRequest(string state, string codeChallenge, string emulatorUrl, string finalRedirect)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/botsignin/GetSignInUrl", false);
            uri.AppendQuery("state", state, true);
            if (codeChallenge != null)
            {
                uri.AppendQuery("code_challenge", codeChallenge, true);
            }
            if (emulatorUrl != null)
            {
                uri.AppendQuery("emulatorUrl", emulatorUrl, true);
            }
            if (finalRedirect != null)
            {
                uri.AppendQuery("finalRedirect", finalRedirect, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "text/plain");
            return message;
        }

        /// <inheritdoc/>
        public async Task<string> GetSignInUrlAsync(string state, string codeChallenge = null, string emulatorUrl = null, string finalRedirect = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            using var message = CreateGetSignInUrlRequest(state, codeChallenge, emulatorUrl, finalRedirect);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        StreamReader streamReader = new StreamReader(message.Response.ContentStream);
                        return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                    }
                default:
                    throw new RequestFailedException(message.Response);
            }
        }

        internal HttpMessage CreateGetSignInResourceRequest(string state, string codeChallenge, string emulatorUrl, string finalRedirect)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/botsignin/GetSignInResource", false);
            uri.AppendQuery("state", state, true);
            if (codeChallenge != null)
            {
                uri.AppendQuery("code_challenge", codeChallenge, true);
            }
            if (emulatorUrl != null)
            {
                uri.AppendQuery("emulatorUrl", emulatorUrl, true);
            }
            if (finalRedirect != null)
            {
                uri.AppendQuery("finalRedirect", finalRedirect, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<SignInResource> GetSignInResourceAsync(string state, string codeChallenge = null, string emulatorUrl = null, string finalRedirect = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            using var message = CreateGetSignInResourceRequest(state, codeChallenge, emulatorUrl, finalRedirect);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        SignInResource value = ProtocolJsonSerializer.ToObject<SignInResource>(message.Response.ContentStream);
                        return Response.FromValue(value, message.Response);
                    }
                default:
                    throw new RequestFailedException(message.Response);
            }
        }
    }
}
