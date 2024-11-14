// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Microsoft.Agents.Protocols.Primitives;

namespace Microsoft.Agents.Protocols.Serializer
{
    internal static class Utf8JsonWriterExtensions
    {
        public static void WriteObjectValue(this Utf8JsonWriter writer, Activity value) => Write(writer, value);

        public static void WriteObjectValue(this Utf8JsonWriter writer, ChannelAccount value) => Write(writer, value);

        public static void WriteObjectValue(this Utf8JsonWriter writer, Attachment value) => Write(writer, value);

        public static void WriteObjectValue(this Utf8JsonWriter writer, Entity value) => Write(writer, value);

        private static void Write(Utf8JsonWriter writer, object value)
        {
            var json = JsonSerializer.Serialize(value, ProtocolJsonSerializer.SerializationOptions);
            JsonDocument.Parse(json).WriteTo(writer);
        }
    }
}
