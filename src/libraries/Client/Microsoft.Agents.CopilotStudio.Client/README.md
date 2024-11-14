# Microsoft.Agents.CopilotStudio.Client

Provides a client to interact with agents built in Copilot Studio

## How-to use

```cs
var copilotClient = new CopilotClient(settings, s.GetRequiredService<IHttpClientFactory>(), logger, "mcs");
await foreach (Activity act in copilotClient.StartConversationAsync(emitStartConversationEvent:true, cancellationToken:cancellationToken))
{

}
```