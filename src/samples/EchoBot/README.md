# EchoBot Sample

This is a sample of a simple Copilot that is hosted on an Asp.net core web service.  This Copilot is configured to accept a request and echo the text of the request back to the caller.

This Copilot Sample is intended to introduce you the basic operation of the Copilot SDK's messaging loop. It can also be used as a the base for a custom Copilot that you choose to develop.

## Prerequisites

**To run the sample on a development workstation (local development), the following tools and SDK's are required:**

- [.NET SDK](https://dotnet.microsoft.com/download) version 8.0
- Visual Studio 2022+ with the .net workload installed.
- [Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator/releases) for Testing Web Chat.

**To run the sample connected to Azure Bot Service, the following additional tools are required:**

- Access to an Azure Subscription with access to preform the following tasks:
    - Create and configure Entra ID Application Identities
    - Create and configure an [Azure Bot Service](https://aka.ms/azuredeployment) for your bot
    - Create and configure an [Azure App Service](https://learn.microsoft.com/azure/app-service/) to deploy your bot on to.
    - A tunneling tool to allow for local development and debugging should you wish to do local development whilst connected to a external client such as Microsoft Teams.

## Getting Started with EchoBot Sample

### Local development

Local development means running the Sample on 'your' workstation for development and debugging purposes.

Local development begins with utilizing the Bot Framework Emulator and Visual Studio on your workstation to build and run your Copilot while debugging from Visual Studio.

If you do not wish to configure authentication at this time, Skip to "Running the Copilot for the first time".

### Authentication and Local Development

There are two ways to support local development, depending on what your working with.

**- Anonymous or No-Authentication**

While this is the simplest way to get started and run your Copilot, there are important limitations to consider.

When running in Anonymous mode, your Copilot will not be able to create authentication tokens to access other services, Nor can it interact with Azure Bot Services. Therefor, Anonymous Mode is there to support testing basic operational features of the Copilot and to work with and test various events that your Copilot can process. It should be used only during initial development.

> [!IMPORTANT]
> This sample is configured, by default, for Anonymous Authentication. Before using this sample with Azure Bot Service, it is necessary to configure authentication.

**- Configured Authentication with Entra ID.**

Configuring authentication for your Copilot will allow it to communicate with Azure Bot Services and create access tokens for other services.

However there are a few key items to consider when configuring authentication for your Copilot.

1. Both Azure Bot Service's Bot registration and your Copilot Must use the same ClientID for creating an authentication token.
    1. By default Azure Bot Service will create a Managed Identity when you initially configure the bot registration.  **This type of identity cannot currently be used when working with Local Development**.
    1. To successfully use **Local Development** with an Azure bot Service Identity, you must utilize either **Client Secret** or **Client Certificate** based authentication.
1. Once you are ready to deploy to Azure App Services, you can use all types of Identity supported.
    1. Its often more efficient to have an Azure Bot Service Registration for Local Development and a separate one configured for your App Services Deployment.

#### Configuring authentication in the Echobot Sample Project

To configure authentication into the EchoBot Sample Project you will need the following information:

1. Client ID of the Application identity you wish to use.
1. Client Secret of the Application identity you wish to use or the Certificate that has been registered for the Client ID in Entra AD

Once you have that information, to configure authentication, Open the `appsettings.json` file in the root of the sample project.

Find the section labeled `Connections`,  it should appear similar to this:

```json
"Connections": {
  "BotServiceConnection": {
    "Assembly": "Microsoft.Agents.Authentication.Msal",
    "Type":  "MsalAuth",
    "Settings": {
      "AuthType": "ClientSecret", // this is the AuthType for the connection, valid values can be found in Microsoft.Agents.Authentication.Msal.Model.AuthTypes.  The default is ClientSecret.
      "AuthorityEndpoint": "https://login.microsoftonline.com/botframework.com",
      "ClientId": "00000000-0000-0000-0000-000000000000", // this is the Client ID used for the connection.
      "ClientSecret": "00000000-0000-0000-0000-000000000000", // this is the Client Secret used for the connection.
      "Scopes": [
        "https://api.botframework.com/.default"
      ],
      "TenantId": "" // This is the Tenant ID used for the Connection. 
    }
  }
```

1. Set the **AuthType** to either `ClientSecret` or `Certificate`
1. Set the **ClientId** to the Client Id of your identity
1. if you chose `ClientSecret`
    1. Set the ClientSecret to the Secret that was created for your identity.
1. if you chose `Certificate`
    1. Import the PFX Certificate to your local machine and note which certificate store it was loaded into.
    1. Replace the key **ClientSecret** with **CertificateThumbPrint** and set the value of the **CertificateThumbPrint** to the thumbprint of your certificate
1. Set the **TenantId** to the Tenant Id where your application is registered.

## Running the Copilot for the first time

To run the Echobot Sample for the first time:

1. Open the Echobot Sample in Visual Studio 2022
1. Run it in Debug Mode (F5)
1. A blank web page will open, note down the URL which should be similar too `https://localhost:65349/`
1. Open the [BotFramework Emulator](https://github.com/Microsoft/BotFramework-Emulator/releases)
    1. Click **Open Bot**
    1. In the bot URL field input the URL you noted down from the web page and add /api/messages to it. It should appear similar to `https://localhost:65349/api/messages`
    1. Click **Connect**

if all is working correctly, the Bot Emulator should show you a Web Chat experience with the words **"Hello and Welcome!"**

If you type a message and hit enter, or the send arrow, your messages should be returned to you with **Echo:your message**

## Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading
To learn more about building Bots and Copilots, see our [Copilot SDK GitHub](https://github.com/microsoft/copilot-sdk) repo.