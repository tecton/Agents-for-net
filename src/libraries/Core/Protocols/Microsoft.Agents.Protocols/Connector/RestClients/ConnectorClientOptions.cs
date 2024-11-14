// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace Microsoft.Agents.Protocols.Connector
{
    /// <summary> Client options for ConnectorClient. </summary>
    /// <remarks> Initializes new instance of ConnectorClientOptions. </remarks>
    public class ConnectorClientOptions(ConnectorClientOptions.ServiceVersion version = ConnectorClientOptions.LatestVersion) : ClientOptions
    {
        private const ServiceVersion LatestVersion = ServiceVersion.V3_1_12;

        /// <summary> The version of the service to use. </summary>
        public enum ServiceVersion
        {
            /// <summary> Service version "3.1.12". </summary>
            V3_1_12 = 1,
        }

        internal string Version { get; } = version switch
        {
            ServiceVersion.V3_1_12 => "3.1.12",
            _ => throw new NotSupportedException()
        };

        /// <summary>
        /// Default Headers to set on outgoing requests.
        /// </summary>
        /// <remarks>
        /// To add an additional user agent value, include a "User-Agent" key.  This will be appended to the
        /// default User-Agent value.
        /// </remarks>
        public IDictionary<string, string> DefaultHeaders { get; set; }
    }
}
