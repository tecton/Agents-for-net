// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Protocols.Serializer;

namespace Microsoft.Agents.Protocols.Connector
{
    internal class UserTokenRestClient(HttpPipeline pipeline, Uri endpoint = null) : IUserToken
    {
        private readonly HttpPipeline _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        private readonly Uri _endpoint = endpoint ?? new Uri("");

        internal HttpMessage CreateGetTokenRequest(string userId, string connectionName, string channelId, string code)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/usertoken/GetToken", false);
            uri.AppendQuery("userId", userId, true);
            uri.AppendQuery("connectionName", connectionName, true);
            if (channelId != null)
            {
                uri.AppendQuery("channelId", channelId, true);
            }
            if (code != null)
            {
                uri.AppendQuery("code", code, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        internal HttpMessage CreateExchangeRequest(string userId, string connectionName, string channelId, TokenExchangeRequest body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/usertoken/exchange", false);
            uri.AppendQuery("userId", userId, true);
            uri.AppendQuery("connectionName", connectionName, true);
            uri.AppendQuery("channelId", channelId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");

            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }

            return message;
        }

        /// <inheritdoc/>
        public async Task<object> ExchangeAsyncAsync(string userId, string connectionName, string channelId, TokenExchangeRequest exchangeRequest, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException(nameof(connectionName));
            }
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException(nameof(channelId));
            }
            if (exchangeRequest == null)
            {
                throw new ArgumentNullException(nameof(exchangeRequest));
            }

            using var message = CreateExchangeRequest(userId, connectionName, channelId, exchangeRequest);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 400:
                case 404:
                    {
                        return ProtocolJsonSerializer.ToObject<object>(message.Response.ContentStream);
                    }
                default:
                    throw new RequestFailedException(message.Response);
            }
        }

        /// <inheritdoc/>
        public async Task<TokenResponse> GetTokenAsync(string userId, string connectionName, string channelId = null, string code = null, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (connectionName == null)
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            using var message = CreateGetTokenRequest(userId, connectionName, channelId, code);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    return ProtocolJsonSerializer.ToObject<TokenResponse>(message.Response.ContentStream);
                case 404:
                    return null;
                default:
                    throw new RequestFailedException(message.Response);
            }
        }

        internal HttpMessage CreateGetAadTokensRequest(string userId, string connectionName, string channelId, AadResourceUrls body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/usertoken/GetAadTokens", false);
            uri.AppendQuery("userId", userId, true);
            uri.AppendQuery("connectionName", connectionName, true);
            if (channelId != null)
            {
                uri.AppendQuery("channelId", channelId, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyDictionary<string, TokenResponse>> GetAadTokensAsync(string userId, string connectionName, AadResourceUrls aadResourceUrls, string channelId = null, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (connectionName == null)
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            using var message = CreateGetAadTokensRequest(userId, connectionName, channelId, aadResourceUrls);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<IReadOnlyDictionary<string, TokenResponse>>(message.Response.ContentStream);
                    }
                default:
                    throw new RequestFailedException(message.Response);
            }
        }

        internal HttpMessage CreateSignOutRequest(string userId, string connectionName, string channelId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Delete;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/usertoken/SignOut", false);
            uri.AppendQuery("userId", userId, true);
            if (connectionName != null)
            {
                uri.AppendQuery("connectionName", connectionName, true);
            }
            if (channelId != null)
            {
                uri.AppendQuery("channelId", channelId, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<object> SignOutAsync(string userId, string connectionName = null, string channelId = null, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            using var message = CreateSignOutRequest(userId, connectionName, channelId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<object>(message.Response.ContentStream);
                    }
                case 204:
                    return null;
                default:
                    throw new RequestFailedException(message.Response);
            }
        }

        internal HttpMessage CreateGetTokenStatusRequest(string userId, string channelId, string include)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/usertoken/GetTokenStatus", false);
            uri.AppendQuery("userId", userId, true);
            if (channelId != null)
            {
                uri.AppendQuery("channelId", channelId, true);
            }
            if (include != null)
            {
                uri.AppendQuery("include", include, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<TokenStatus>> GetTokenStatusAsync(string userId, string channelId = null, string include = null, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            using var message = CreateGetTokenStatusRequest(userId, channelId, include);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<IReadOnlyList<TokenStatus>>(message.Response.ContentStream);
                    }
                default:
                    throw new RequestFailedException(message.Response);
            }
        }

        internal HttpMessage CreateExchangeTokenRequest(string userId, string connectionName, string channelId, TokenExchangeRequest body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/api/usertoken/exchange", false);
            uri.AppendQuery("userId", userId, true);
            uri.AppendQuery("connectionName", connectionName, true);
            uri.AppendQuery("channelId", channelId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        /// <inheritdoc/>
        public async Task<TokenResponse> ExchangeTokenAsync(string userId, string connectionName, string channelId, TokenExchangeRequest body = null, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (connectionName == null)
            {
                throw new ArgumentNullException(nameof(connectionName));
            }
            if (channelId == null)
            {
                throw new ArgumentNullException(nameof(channelId));
            }

            using var message = CreateExchangeTokenRequest(userId, connectionName, channelId, body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 404:
                    {
                        return ProtocolJsonSerializer.ToObject<TokenResponse>(message.Response.ContentStream);
                    }
                default:
                    throw new RequestFailedException(message.Response);
            }
        }
    }
}
