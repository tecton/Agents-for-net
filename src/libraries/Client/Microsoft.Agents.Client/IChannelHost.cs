// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.Agents.Client
{
    /// <summary>
    /// Represents a host the contains IChannels for Bot-to-bot.
    /// </summary>
    public interface IChannelHost
    {
        /// <summary>
        /// The endpoint to use in Activity.ServiceUrl.
        /// </summary>
        Uri HostEndpoint { get; }

        string HostAppId { get; }

        /// <summary>
        /// The bots the host knows about.
        /// </summary>
        IDictionary<string, IChannelInfo> Channels { get; }

        IChannel GetChannel(IChannelInfo channelInfo);

        IChannel GetChannel(string name);
    }
}
