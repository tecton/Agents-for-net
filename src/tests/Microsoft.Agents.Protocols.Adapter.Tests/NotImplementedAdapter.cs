using Microsoft.Agents.Protocols.Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Agents.Protocols.Adapter.Tests
{
    internal class NotImplementedAdapter : BotAdapter
    {
        public override Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, IActivity[] activities, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
