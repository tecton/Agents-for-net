# Microsoft.Agents.Protocols

## About

Contains the implementation for the Bot Framework Activity Protocol

## How to use with WebAPI

```cs
 public class EchoBot : ActivityHandler
 {
     protected override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken) 
        => turnContext.SendActivityAsync(turnContext.Activity.Text, cancellationToken)
 }


[Authorize]    
[ApiController]
[Route("api/messages")]
public class BotController(IBotHttpAdapter adapter, IBot bot) : ControllerBase
{
    [HttpPost]
    public Task PostAsync(CancellationToken cancellationToken) 
        => adapter.ProcessAsync(Request, Response, bot, cancellationToken);            
}
```

## Main Types

The main types provided by this library are:

- `Microsoft.Agents.Protocols.Primitives.Activity`
- `Microsoft.Agents.Protocols.Adapter.ActivityHandler`
- `Microsoft.Agents.Protocols.Adapter.TurnContext`

