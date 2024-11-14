# MSALAuthConfigurationOptions


## About

Supports the usage of Entra ID tokens to interact with Agents

## Key Features

Defines the Configuration used to specify the Entra ID properties

```json
  "Connections": {
    "BotServiceConnection": {
      "Assembly": "Microsoft.Agents.Authentication.Msal",
      "Type": "MsalAuth",
      "Settings": {
        "AuthType": "ClientSecret",
        "ClientSecret": "<ClientSecret>",
        "AuthorityEndpoint": "https://login.microsoftonline.com/botframework.com",
        "ClientId": "<ClientID>",
        "Scopes": [
            "https://api.botframework.com/.default"
          ],
          "TenantId": "<Client Tenant Id>"
      }
    }
  }
```