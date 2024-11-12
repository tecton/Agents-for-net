// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Microsoft.Agents.Protocols.Serializer;
using Microsoft.Agents.Teams.Primitives;

namespace Microsoft.Agents.Teams.Serialization
{
    // This is required because ConnectorConverter supports derived type handling.
    // In this case for the 'Task' property of type TaskModuleResponse.
    internal class TaskModuleResponseConverter : ConnectorConverter<TaskModuleResponse>
    {
        protected override bool TryReadCollectionProperty(ref Utf8JsonReader reader, TaskModuleResponse value, string propertyName, JsonSerializerOptions options)
        {
            return false;
        }

        protected override bool TryReadGenericProperty(ref Utf8JsonReader reader, TaskModuleResponse value, string propertyName, JsonSerializerOptions options)
        {
            return false;
        }

        protected override void ReadExtensionData(ref Utf8JsonReader reader, TaskModuleResponse value, string propertyName, JsonSerializerOptions options)
        {
        }

        protected override bool TryReadExtensionData(ref Utf8JsonReader reader, TaskModuleResponse value, string propertyName, JsonSerializerOptions options)
        {
            return false;
        }

        protected override bool TryWriteExtensionData(Utf8JsonWriter writer, TaskModuleResponse value, string propertyName)
        {
            return false;
        }
    }
}
