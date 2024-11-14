// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Protocols.Serializer;

namespace Microsoft.Agents.Protocols.Connector
{
    /// <summary> The ClientDiagnostics is used to provide tracing support for the client library. </summary>
    /// <summary> Initializes a new instance of AttachmentsRestClient. </summary>
    /// <param name="clientDiagnostics"> The handler for diagnostic messaging in the client. </param>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> server parameter. </param>
    internal class AttachmentsRestClient(HttpPipeline pipeline, Uri endpoint = null) : IAttachments
    {
        private readonly HttpPipeline _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        private readonly Uri _endpoint = endpoint ?? new Uri("");

        internal HttpMessage CreateGetAttachmentInfoRequest(string attachmentId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/attachments/", false);
            uri.AppendPath(attachmentId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <summary> GetAttachmentInfo. </summary>
        /// <param name="attachmentId"> attachment id. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="attachmentId"/> is null. </exception>
        /// <remarks> Get AttachmentInfo structure describing the attachment views. </remarks>
        public async Task<AttachmentInfo> GetAttachmentInfoAsync(string attachmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(attachmentId))
            {
                throw new ArgumentNullException(nameof(attachmentId));
            }

            using var message = CreateGetAttachmentInfoRequest(attachmentId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<AttachmentInfo>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"GetAttachmentInfo operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateGetAttachmentRequest(string attachmentId, string viewId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/attachments/", false);
            uri.AppendPath(attachmentId, true);
            uri.AppendPath("/views/", false);
            uri.AppendPath(viewId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/octet-stream, application/json");
            return message;
        }

        /// <summary> GetAttachment. </summary>
        /// <param name="attachmentId"> attachment id. </param>
        /// <param name="viewId"> View id from attachmentInfo. </param>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="attachmentId"/> or <paramref name="viewId"/> is null. </exception>
        /// <remarks> Get the named view as binary content. </remarks>
        public async Task<Stream> GetAttachmentAsync(string attachmentId, string viewId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(attachmentId))
            {
                throw new ArgumentNullException(nameof(attachmentId));
            }
            if (string.IsNullOrEmpty(viewId))
            {
                throw new ArgumentNullException(nameof(viewId));
            }

            using var message = CreateGetAttachmentRequest(attachmentId, viewId);
            RedirectPolicy.SetAllowAutoRedirect(message, true);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return message.ExtractResponseContent();
                    }
                case 301:
                case 302:
                    return null;
                default:
                    {
                        var ex = new ErrorResponseException($"GetAttachment operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }
    }
}
