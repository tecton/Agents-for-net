# Microsoft Copilot Studio Skill Sample

This is a sample of a simple conversational Agent that can be used from the Microsoft Copilot Studio Skills feature.

This sample is intended to introduce you to:
- The basic operation of the Agents SDK messaging handling.
- Requirements of a conversational Agent being used by another bot.
- Using this Agent from Microsoft Copilot Studio.

## Basic concepts
- A Skill is an Agent.  There is nothing inherently different from other Agents.
- While many Agents are conversational, they can respond to any messages sent using the Activity Protocol.
- Conversations are multi-turn interactions, including possibly a large number of exchanges with the user until the conversation is complete.  
- Agents being called by another Agent are expected to:
  - Indicate when the conversation is over, with a "success" result and optional return value.
  - Indicate the conversation is over after a critical error.
- Microsoft Copilot Studio requires a manifest that describes the Agent capabilities.

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
      - [devtunnels](https://learn.microsoft.com/en-us/azure/developer/dev-tunnels/get-started?tabs=windows)
      - [Microsoft Teams TunnelRelay](https://github.com/OfficeDev/microsoft-teams-tunnelrelay/releases/tag/v2.2)

## Getting Started with EchoSkill Sample

### Local development

Local development means running the sample on 'your' workstation for development and debugging purposes.

Local development begins with utilizing the Bot Framework Emulator and Visual Studio on your workstation to build and run your Agent while debugging from Visual Studio.

If you do not wish to configure authentication at this time, Skip to "Running the Agent for the first time".

### Authentication and Local Development

There are two ways to support local development, depending on what your working with.

**- Anonymous or No-Authentication**

While this is the simplest way to get started and run your Agent, there are important limitations to consider.

When running in Anonymous mode, your Agent will not be able to create authentication tokens to access other services, Nor can it interact with Azure Bot Services. Therefor, Anonymous Mode is there to support testing basic operational features of the Agent and to work with and test various events that your Agent can process. It should be used only during initial development.

> [!IMPORTANT]
> This sample is configured, by default, for Anonymous Authentication. Before using this sample with Azure Bot Service, it is necessary to configure authentication.

**- Configured Authentication with Entra ID.**

Configuring authentication for your Agent will allow it to communicate with Azure Bot Services and create access tokens for other services.

However there are a few key items to consider when configuring authentication for your Agent.

1. Both Azure Bot Service's Bot registration and your Agent Must use the same ClientID for creating an authentication token.
    1. By default Azure Bot Service will create a Managed Identity when you initially configure the bot registration.  **This type of identity cannot currently be used when working with Local Development**.
    1. To successfully use **Local Development** with an Azure bot Service Identity, you must utilize either **Client Secret** or **Client Certificate** based authentication.
1. Once you are ready to deploy to Azure App Services, you can use all types of Identity supported.
    1. Its often more efficient to have an Azure Bot Service Registration for Local Development and a separate one configured for your App Services Deployment.

#### Configuring authentication in the EchoSkill Sample Project

To configure authentication into the EchoSkill Sample Project you will need the following information:

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
1. When using a tunnel, the "Messaging Endpoint" on the Azure Bot should be: `{tunnel-url}/api/messages`

## Running the Agent for the first time

To run the EchoSkill Sample for the first time:

1. Open the EchoSkill Sample in Visual Studio 2022
1. Run it in Debug Mode (F5)
1. A blank web page will open, note down the URL which should be similar too `https://localhost:3978/`
1. Open the [BotFramework Emulator](https://github.com/Microsoft/BotFramework-Emulator/releases)
    1. Click **Open Bot**
    1. In the bot URL field input the URL you noted down from the web page and add /api/messages to it. It should appear similar to `https://localhost:39783/api/messages`
    1. Click **Connect**

If all is working correctly, the Bot Emulator should show you a Web Chat experience with the words **"Hi, This is EchoSkill"**

If you type a message and hit enter, or the send arrow, your messages should be returned to you with **Echo(EchoSkill):your message** and **Echo(EchoSkill): Say “end” or “stop” and I’ll end the conversation and return to the parent.**

## Using this Agent from Microsoft Copilot Studio as a Skill
- In order to use this sample from Copilot Studio, the bot will need to be created on Azure, and authentication setup.
- It is possible to run the Agent locally for debugging purposes.  
  - Managed Identity does not work locally so `ClientSecret` or `Certificate` will need to be used.
  - A tunnel will be required to route messages sent to the bot to you locally running agent.
- Copilot Studio requires a manifest for the agent ("Skills Manifest").  This is discussed below.

### Creating the Agent Manifest

The EchoSkill sample contains the minimal manifest in `wwwroot/manifest/echoskill-manifest-1.0.json`

- Open the [sample manifest](./wwwroot/manifest/echoskill-manifest-1.0.json)
- Update the `privacyUrl`, `iconUrl`, `msAppId`, and `endpointUrl`
  - If you are running the bot locally, this will be `{tunnel-url}/api/messages`
- Once EchoSkill is started, the manifest is available via a GET to `{host-or-tunnel-url}/manifest/echoskill-manifest-1.0.json`

### Using EchoSkill in Copilot Studio
- In order to use an Agent from Copilot Studio Skills
  - The Azure Bot and Identity must have been created on Azure
  - If the bot is running locally, the "Messaging Endpoint" on the Azure Bot must be set to `{tunnel-url}/api/messages`
  - On the App Registration on Azure, the "Home page URL" should be set to the App Service URL (or the Tunnel URL)
- Create a new, or open an existing, Copilot Studio Agent
- Go to Agent Settings
- Select "Skills" on the left sidebar
- Select "Add a Skill"
- Enter the URL to the skill manifest, for example `{host-or-tunnel-url}/manifest/echoskill-manifest-1.0.json`
- Click "Save" when validation is complete.
- From any Topic, add a new "Call an Action" node, and select "Echo messages from user"
- Test the agent in Copilot Studio.

## Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading
To learn more about building Bots and Agents, see our [Microsoft Agents Framework on GitHub](https://github.com/microsoft/agents) repo.