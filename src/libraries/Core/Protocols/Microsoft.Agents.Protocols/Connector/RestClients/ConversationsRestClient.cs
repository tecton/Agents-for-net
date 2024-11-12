// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Protocols.Serializer;

namespace Microsoft.Agents.Protocols.Connector
{
    internal class ConversationsRestClient : IConversations
    {
        private readonly HttpPipeline _pipeline;
        private readonly Uri _endpoint;

        public ConversationsRestClient(HttpPipeline pipeline, Uri endpoint = null)
        {
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _endpoint = endpoint ?? new Uri("");
        }

        internal HttpMessage CreateGetConversationsRequest(string continuationToken)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations", false);
            if (continuationToken != null)
            {
                uri.AppendQuery("continuationToken", continuationToken, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<ConversationsResult> GetConversationsAsync(string continuationToken = null, CancellationToken cancellationToken = default)
        {
            using var message = CreateGetConversationsRequest(continuationToken);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<ConversationsResult>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"GetConversations operation returned an invalid status code '{message.Response.Status}'");

                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }

                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateCreateConversationRequest(ConversationParameters body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        /// <inheritdoc/>
        public async Task<ConversationResourceResponse> CreateConversationAsync(ConversationParameters body = null, CancellationToken cancellationToken = default)
        {
            using var message = CreateCreateConversationRequest(body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 201:
                case 202:
                    {
                        return ProtocolJsonSerializer.ToObject<ConversationResourceResponse>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"CreateConversation operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateSendToConversationRequest(string conversationId, IActivity body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/activities", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        public async Task<ResourceResponse> SendToConversationAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendToConversationAsync(activity.Conversation.Id, activity, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<ResourceResponse> SendToConversationAsync(string conversationId, IActivity body = null, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            using var message = CreateSendToConversationRequest(conversationId, body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 201:
                case 202:
                    {
                        return ProtocolJsonSerializer.ToObject<ResourceResponse>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"SendToConversation operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateSendConversationHistoryRequest(string conversationId, Transcript body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/activities/history", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        /// <inheritdoc/>
        public async Task<ResourceResponse> SendConversationHistoryAsync(string conversationId, Transcript body = null, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            using var message = CreateSendConversationHistoryRequest(conversationId, body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 201:
                case 202:
                    {
                        return ProtocolJsonSerializer.ToObject<ResourceResponse>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"SendConversationHistory operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateUpdateActivityRequest(string conversationId, string activityId, IActivity body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Put;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/activities/", false);
            uri.AppendPath(activityId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        public async Task<ResourceResponse> UpdateActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await UpdateActivityAsync(activity.Conversation.Id, activity.Id, activity, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<ResourceResponse> UpdateActivityAsync(string conversationId, string activityId, IActivity body = null, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            if (activityId == null)
            {
                throw new ArgumentNullException(nameof(activityId));
            }

            using var message = CreateUpdateActivityRequest(conversationId, activityId, body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 201:
                case 202:
                    {
                        return ProtocolJsonSerializer.ToObject<ResourceResponse>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"UpdateActivity operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateReplyToActivityRequest(string conversationId, string activityId, IActivity body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/activities/", false);
            uri.AppendPath(activityId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        public async Task<ResourceResponse> ReplyToActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return activity == null
                ? throw new ArgumentNullException(nameof(activity))
                : await ReplyToActivityAsync(activity.Conversation.Id, activity.ReplyToId, activity, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<ResourceResponse> ReplyToActivityAsync(string conversationId, string activityId, IActivity body = null, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            if (activityId == null)
            {
                throw new ArgumentNullException(nameof(activityId));
            }

            using var message = CreateReplyToActivityRequest(conversationId, activityId, body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 201:
                case 202:
                    {
                        // Teams is famous for not returning a response body for these.
                        if (message.Response.ContentStream.Length == 0)
                        {
                            return new ResourceResponse() { Id = string.Empty };
                        }
                        return ProtocolJsonSerializer.ToObject<ResourceResponse>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"ReplyToActivity operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateDeleteActivityRequest(string conversationId, string activityId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Delete;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/activities/", false);
            uri.AppendPath(activityId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task DeleteActivityAsync(string conversationId, string activityId, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            if (activityId == null)
            {
                throw new ArgumentNullException(nameof(activityId));
            }

            using var message = CreateDeleteActivityRequest(conversationId, activityId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 202:
                    return;
                default:
                    {
                        var ex = new ErrorResponseException($"DeleteActivity operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateGetConversationMembersRequest(string conversationId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/members", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ChannelAccount>> GetConversationMembersAsync(string conversationId, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            using var message = CreateGetConversationMembersRequest(conversationId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<IReadOnlyList<ChannelAccount>>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"GetConversationMembers operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateGetConversationMemberRequest(string conversationId, string userId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/members/", false);
            uri.AppendPath(userId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<ChannelAccount> GetConversationMemberAsync(string userId, string conversationId, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            using var message = CreateGetConversationMemberRequest(conversationId, userId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<ChannelAccount>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"GetConversationMember operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateDeleteConversationMemberRequest(string conversationId, string memberId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Delete;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/members/", false);
            uri.AppendPath(memberId, true);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task DeleteConversationMemberAsync(string conversationId, string memberId, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            if (memberId == null)
            {
                throw new ArgumentNullException(nameof(memberId));
            }

            using var message = CreateDeleteConversationMemberRequest(conversationId, memberId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 204:
                    return;
                default:
                    {
                        var ex = new ErrorResponseException($"DeleteConversationMember operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }
        internal HttpMessage CreateGetConversationPagedMembersRequest(string conversationId, int? pageSize, string continuationToken)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/pagedmembers", false);
            if (pageSize != null)
            {
                uri.AppendQuery("pageSize", pageSize.Value.ToString(), true);
            }
            if (continuationToken != null)
            {
                uri.AppendQuery("continuationToken", continuationToken, true);
            }
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<PagedMembersResult> GetConversationPagedMembersAsync(string conversationId, int? pageSize = null, string continuationToken = null, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            using var message = CreateGetConversationPagedMembersRequest(conversationId, pageSize, continuationToken);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<PagedMembersResult>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"GetConversationPagedMembers operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateGetActivityMembersRequest(string conversationId, string activityId)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Get;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/activities/", false);
            uri.AppendPath(activityId, true);
            uri.AppendPath("/members", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            return message;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ChannelAccount>> GetActivityMembersAsync(string conversationId, string activityId, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            if (activityId == null)
            {
                throw new ArgumentNullException(nameof(activityId));
            }

            using var message = CreateGetActivityMembersRequest(conversationId, activityId);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                    {
                        return ProtocolJsonSerializer.ToObject<IReadOnlyList<ChannelAccount>>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"GetActivityMembers operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }

        internal HttpMessage CreateUploadAttachmentRequest(string conversationId, AttachmentData body)
        {
            var message = _pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RequestUriBuilder();
            uri.Reset(_endpoint);
            uri.AppendPath("/v3/conversations/", false);
            uri.AppendPath(conversationId, true);
            uri.AppendPath("/attachments", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            if (body != null)
            {
                request.Headers.Add("Content-Type", "application/json");
                var content = new JsonRequestContent();
                content.WriteObjectValue(body);
                request.Content = content;
            }
            return message;
        }

        /// <inheritdoc/>
        public async Task<ResourceResponse> UploadAttachmentAsync(string conversationId, AttachmentData body = null, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            using var message = CreateUploadAttachmentRequest(conversationId, body);
            await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            switch (message.Response.Status)
            {
                case 200:
                case 201:
                case 202:
                    {
                        return ProtocolJsonSerializer.ToObject<ResourceResponse>(message.Response.ContentStream);
                    }
                default:
                    {
                        var ex = new ErrorResponseException($"UploadAttachment operation returned an invalid status code '{message.Response.Status}'");
                        try
                        {
                            ErrorResponse errorBody = ProtocolJsonSerializer.ToObject<ErrorResponse>(message.Response.ContentStream);
                            if (errorBody != null)
                            {
                                ex.Body = errorBody;
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore the exception
                        }
                        throw ex;
                    }
            }
        }
    }
}
