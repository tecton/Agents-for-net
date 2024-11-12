// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System;
using Azure.Core;

namespace Microsoft.Agents.Protocols.Connector
{
    /// <summary> Client options for TokenClient. </summary>
    public class TokenClientOptions : ClientOptions
    {
        private const ServiceVersion LatestVersion = ServiceVersion.V3_1_12;

        /// <summary> The version of the service to use. </summary>
        public enum ServiceVersion
        {
            /// <summary> Service version "3.1.12". </summary>
            V3_1_12 = 1,
        }

        internal string Version { get; }

        /// <summary> Initializes new instance of TokenClientOptions. </summary>
        public TokenClientOptions(ServiceVersion version = LatestVersion)
        {
            Version = version switch
            {
                ServiceVersion.V3_1_12 => "3.1.12",
                _ => throw new NotSupportedException()
            };
        }
    }
}
