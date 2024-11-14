// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Microsoft.Agents.Protocols.Serializer;
using Microsoft.Agents.SharePoint.Primitives;

namespace Microsoft.Agents.Sharepoint.Serialization
{
    internal class AceRequestConverter : ConnectorConverter<AceRequest>
    {
        protected override bool TryReadGenericProperty(ref Utf8JsonReader reader, AceRequest value, string propertyName, JsonSerializerOptions options)
        {
            if (propertyName.Equals(nameof(value.Properties)))
            {
                SetGenericProperty(ref reader, data => value.Properties = data, options);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
