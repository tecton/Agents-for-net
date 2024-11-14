// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable enable

using Azure.Core;
using Microsoft.Agents.Protocols.Serializer;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Agents.Protocols.Connector
{
    internal class JsonRequestContent: RequestContent
    {
        private readonly MemoryStream _stream;
        private readonly RequestContent _content;

        public JsonRequestContent()
        {
            _stream = new MemoryStream();
            _content = Create(_stream);
        }

        public void WriteObjectValue(object body)
        {
            JsonSerializer.Serialize(_stream, body, ProtocolJsonSerializer.SerializationOptions);
        }

        public override async Task WriteToAsync(Stream stream, CancellationToken cancellation)
        {
            await _content.WriteToAsync(stream, cancellation).ConfigureAwait(false);
        }

        public override void WriteTo(Stream stream, CancellationToken cancellation)
        {
            _content.WriteTo(stream, cancellation);
        }

        public override bool TryComputeLength(out long length)
        {
            length = _stream.Length;
            return true;
        }

        public override void Dispose()
        {
            _content.Dispose();
            _stream.Dispose();
        }
    }
}
