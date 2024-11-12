// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Protocols.Primitives;

namespace Microsoft.Agents.Teams.Primitives
{
    /// <summary>
    /// ﻿﻿The Connector for Microsoft Teams allows your bot to perform extended operations on a Microsoft Teams channel.
    /// </summary>
    public interface ITeamsConnectorClient : IConnectorClient
    {
        /// <summary>
        /// Gets the ITeamsOperations.
        /// </summary>
        /// <value>The ITeamsOperations.</value>
        ITeamsOperations Teams { get; }
    }
}
