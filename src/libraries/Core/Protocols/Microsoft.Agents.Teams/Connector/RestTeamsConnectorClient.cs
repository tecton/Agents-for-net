// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Authentication;
using Microsoft.Agents.Protocols.Connector;
using Microsoft.Agents.Teams.Primitives;
using System;
using System.Collections.Generic;

namespace Microsoft.Agents.Teams.Connector
{
    /// <summary>
    /// TeamsConnectorClient REST implementation.  This ConnectorClient is suitable for either ABS or SMBA.
    /// </summary>
    public class RestTeamsConnectorClient : RestConnectorClient, ITeamsConnectorClient
    {
        public RestTeamsConnectorClient(Uri endpoint, IAccessTokenProvider tokenAccess, string resource, IList<string> scopes = null, bool useAnonymousConnection = false)
            : this(endpoint, tokenAccess, resource, scopes, new ConnectorClientOptions(), useAnonymousConnection)
        {
        }

        public RestTeamsConnectorClient(Uri endpoint, IAccessTokenProvider tokenAccess, string resource, IList<string> scopes, ConnectorClientOptions options, bool useAnonymousConnection = false)
            : base(endpoint, tokenAccess, resource, scopes, options, useAnonymousConnection)
        {
            Teams = new RestTeamsOperations(this, _pipeline);
        }

        /// <inheritdoc/>
        public ITeamsOperations Teams { get; private set; }
    }
}
