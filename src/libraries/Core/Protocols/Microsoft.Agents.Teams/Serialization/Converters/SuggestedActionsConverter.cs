// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Reflection;
using System.Text.Json;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Protocols.Serializer;
using Microsoft.Agents.Teams.Primitives;

namespace Microsoft.Agents.Teams.Serialization
{
    // This class is used to convert the 'SuggestedActions' property of type SuggestedActions for Teams.
    internal class SuggestedActionsConverter : ConnectorConverter<SuggestedActions>
    {
        protected override bool TryReadCollectionProperty(ref Utf8JsonReader reader, SuggestedActions value, string propertyName, JsonSerializerOptions options)
        {
            PropertyInfo propertyInfo = typeof(SuggestedActions).GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
            {
                return true;
            }
            return base.TryReadCollectionProperty(ref reader, value, propertyName, options);
        }
    }
}
