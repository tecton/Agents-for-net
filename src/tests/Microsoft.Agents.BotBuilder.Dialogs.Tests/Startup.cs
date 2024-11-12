using Microsoft.Agents.BotBuilder.Dialogs;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("Microsoft.Agents.BotBuilder.Startup", "Microsoft.Agents.BotBuilder.Dialogs.Tests")]

namespace Microsoft.Agents.BotBuilder
{
    public class Startup : XunitTestFramework
    {
        public Startup(IMessageSink messageSink)
            : base(messageSink)
        {
            //ComponentRegistration.Add(new DialogsComponentRegistration());
        }
    }
}
