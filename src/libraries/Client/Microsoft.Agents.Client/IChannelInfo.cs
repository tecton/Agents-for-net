// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Agents.Client
{
    public interface IChannelInfo
    {
        /// <summary>
        /// Gets or sets Id of the channel.
        /// </summary>
        /// <value>
        /// Id of the channel.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets appId of the channel.
        /// </summary>
        /// <value>
        /// AppId of the channel.
        /// </value>
        public string AppId { get; set; }

        public string ResourceUrl { get; set; }

        /// <summary>
        /// Gets or sets provider name for tokens.
        /// </summary>
        public string TokenProvider { get; set; }

        /// <summary>
        /// Gets or sets the client factory name for the channel.
        /// </summary>
        public string ChannelFactory { get; set; }

        /// <summary>
        /// Gets or sets endpoint for the channel.
        /// </summary>
        /// <value>
        /// Uri for the channel.
        /// </value>
        public Uri Endpoint { get; set; }
    }
}
