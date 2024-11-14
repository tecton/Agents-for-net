// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Protocols.Primitives;

namespace Microsoft.Agents.Memory.Tests
{
    public class PocoStoreItem : IStoreItem
    {
        public string ETag { get; set; }

        public string Id { get; set; }

        public int Count { get; set; }
    }
}
