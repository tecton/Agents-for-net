using System.Runtime.Serialization;

namespace Microsoft.Agents.CopilotStudio.Client.Discovery
{
    /// <summary>
    /// Bot types that can be connected to by the Copilot Studio Client
    /// </summary>
    public enum BotType
    {
        /// <summary>
        /// Copilot Studio Published Copilot
        /// </summary>
        [EnumMember(Value = "Published")]
        Published = 0,
        /// <summary>
        /// System PreBuilt Copilot
        /// </summary>
        [EnumMember(Value = "Prebuilt")]
        Prebuilt = 1
    }
}
