// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable disable

using System.Collections.Generic;

namespace Microsoft.Agents.Protocols.Primitives
{
    /// <summary> Conversations result. </summary>
    public class ConversationsResult
    {
        /// <summary> Initializes a new instance of ConversationsResult. </summary>
        public ConversationsResult()
        {
            Conversations = new List<ConversationMembers>();
        }

        /// <summary> Initializes a new instance of ConversationsResult. </summary>
        /// <param name="continuationToken"> Paging token. </param>
        /// <param name="conversations"> List of conversations. </param>
        public ConversationsResult(string continuationToken = default, IList<ConversationMembers> conversations = default)
        {
            ContinuationToken = continuationToken;
            Conversations = conversations ?? new List<ConversationMembers>();
        }

        /// <summary> Paging token. </summary>
        public string ContinuationToken { get; set; }
        /// <summary> List of conversations. </summary>
        public IList<ConversationMembers> Conversations { get; set; }
    }
}
