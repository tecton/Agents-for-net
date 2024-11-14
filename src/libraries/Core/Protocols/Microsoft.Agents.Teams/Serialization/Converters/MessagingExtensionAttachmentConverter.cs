// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Protocols.Serializer;
using Microsoft.Agents.Teams.Primitives;

namespace Microsoft.Agents.Teams.Serialization
{
    // This is required because ConnectorConverter supports derived type handling.
    // In this case for the 'Task' property of type TaskModuleResponseBase.
    internal class MessagingExtensionAttachmentConverter : ConnectorConverter<MessagingExtensionAttachment>
    {
    }
}
