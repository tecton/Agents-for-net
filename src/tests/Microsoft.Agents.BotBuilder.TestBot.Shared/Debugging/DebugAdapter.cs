
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Extensions.Logging;

namespace Microsoft.Agents.BotBuilder.TestBot.Shared.Debugging
{
    public class DebugAdapter : CloudAdapter
    {
        public DebugAdapter(IChannelServiceClientFactory channelServiceClientFactory, ILogger logger = null) : base(channelServiceClientFactory, logger)
        {
        }
    }
}
