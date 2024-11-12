// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Protocols.Primitives;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Microsoft.Agents.Protocols.Serializer
{
    public static class EntityExtension
    {
        /// <summary>
        /// Retrieve internal payload.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <returns>T as T.</returns>
        public static T GetAs<T>(this Entity entity)
        {
            return ProtocolJsonSerializer.GetAs<T, Entity>(entity);
        }

        /// <summary>
        /// Set internal payload.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="entity"></param>
        /// <param name="obj">obj.</param>
        public static void SetAs<T>(this Entity entity, T obj)
        {
            var copy = ProtocolJsonSerializer.CloneTo<Entity>(obj);
            entity.Type = copy.Type;
            entity.Properties = copy.Properties;
        }

        /// <summary>
        /// Resolves the mentions from the entities of this activity.
        /// </summary>
        /// <returns>The array of mentions; or an empty array, if none are found.</returns>
        /// <remarks>This method is defined on the <see cref="Activity"/> class, but is only intended
        /// for use with a message activity, where the activity <see cref="Activity.Type"/> is set to
        /// <see cref="ActivityTypes.Message"/>.</remarks>
        /// <seealso cref="Mention"/>
        public static Mention[] GetMentions(this IActivity activity)
        {
            var result = new List<Mention>();
            if (activity.Entities != null)
            {
                foreach (var entity in activity.Entities)
                {
                    if (string.Compare(entity.Type, "mention", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        result.Add(ProtocolJsonSerializer.CloneTo<Mention>(entity));
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Remove recipient mention text from Text property.
        /// Use with caution because this function is altering the text on the Activity.
        /// </summary>
        /// <returns>new .Text property value.</returns>
        public static string RemoveRecipientMention<T>(this T activity) where T : IActivity
        {
            return activity.RemoveMentionText(activity.Recipient.Id);
        }

        /// <summary>
        /// Remove any mention text for given id from the Activity.Text property.  For example, given the message
        /// @echoBot Hi Bot, this will remove "@echoBot", leaving "Hi Bot".
        /// </summary>
        /// <param name="activity"></param>
        /// <description>
        /// Typically this would be used to remove the mention text for the target recipient (the bot usually), though
        /// it could be called for each member.  For example:
        ///    turnContext.Activity.RemoveMentionText(turnContext.Activity.Recipient.Id);
        /// The format of a mention Activity.Entity is dependent on the Channel.  But in all cases we
        /// expect the Mention.Text to contain the exact text for the user as it appears in
        /// Activity.Text.
        /// For example, Teams uses &lt;at&gt;username&lt;/at&gt;, whereas slack use @username. It
        /// is expected that text is in Activity.Text and this method will remove that value from
        /// Activity.Text.
        /// </description>
        /// <param name="id">id to match.</param>
        /// <returns>new Activity.Text property value.</returns>
        public static string RemoveMentionText(this IActivity activity, string id)
        {
            foreach (var mention in activity.GetMentions().Where(mention => mention.Mentioned.Id == id))
            {
                if (mention.Text == null)
                {
                    activity.Text = Regex.Replace(activity.Text, "<at>" + Regex.Escape(mention.Mentioned.Name) + "</at>", string.Empty, RegexOptions.IgnoreCase).Trim();
                }
                else
                {
                    activity.Text = Regex.Replace(activity.Text, Regex.Escape(mention.Text), string.Empty, RegexOptions.IgnoreCase).Trim();
                }
            }

            return activity.Text;
        }
    }
}
