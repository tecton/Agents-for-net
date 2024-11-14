// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net.Http;

namespace Microsoft.Agents.Client
{
    /// <summary>
    /// An HTTP based IBotClient factory.
    /// </summary>
    /// <param name="tokenAccess"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="logger"></param>
    public class HttpBotChannelFactory(IHttpClientFactory httpClientFactory, ILogger<HttpBotChannelFactory> logger) : IChannelFactory
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly ILogger<HttpBotChannelFactory> _logger = logger ?? NullLogger<HttpBotChannelFactory>.Instance;

        /// <inheritdoc />
        public IChannel CreateChannel(IAccessTokenProvider tokenAccess)
        {
            return new HttpBotChannel(tokenAccess, _httpClientFactory, _logger);
        }
    }
}
