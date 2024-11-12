// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Agents.SharePoint.Primitives
{
    /// <summary>
    /// Base Card text input button class.
    /// </summary>
    public class CardTextInputBaseButton : ICardButtonBase
    {
        /// <summary>
        /// Gets or sets the button's action.
        /// </summary>
        /// <value>Button's action.</value>
        public IAction Action { get; set; }

        /// <summary>
        /// Gets or sets unique Id of the button.
        /// </summary>
        /// <value>Unique Id of the button.</value>
        public string Id { get; set; }
    }
}
