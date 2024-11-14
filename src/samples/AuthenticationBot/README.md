# OAuth Authentication

This Agent has been created using [Agents Framework](https://github.com/microsoft/agents-for-net), it shows how to use authentication in your Agent using OAuth.

- The sample uses the bot authentication capabilities in [Azure Bot Service](https://docs.botframework.com), providing features to make it easier to develop a bot that authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, etc.
- The samples demonstrates performing OAuth without using the Dialogs package.

## To try this sample

- Register a bot in Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment)

- [Add Authentication to your bot via Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=csharp)

- Setup token connections in the bot appsettings

- Start devtunnel

- Update your Azure Bot ``Messaging endpoint`` with the tunnel Url:  `{tunnel-url}/api/messages`

- Update `appsettings.json` 

  | Property             | Value Description     | 
  |----------------------|-----------|
  | ConnectionName       | Set the configured bot's OAuth connection name.      |
    
- Run the bot from a terminal or from Visual Studio

- Test via "Test in WebChat"" on your Azure Bot in the Azure Portal.

## Using this Agent in Teams

1. The Azure Bot must have the Teams Channel added.
1. Manually update the manifest.json
   - Edit the `manifest.json` contained in the  `/appManifest` folder to replace with your AppId (that was created above) *everywhere* you see the place holder string `<<AAD_APP_CLIENT_ID>>`
   - Zip up the contents of the `/appManifest` folder to create a `manifest.zip`
1. Upload the `manifest.zip` to Teams
   - Select **Developer Portal** in the Teams left sidebar
   - Select **Apps** (top row)
   - Select **Import app**, and select the manifest.zip
1. Start the Agent, and select **Preview in Teams** in the upper right corner

## Interacting with the Agent

Type anything to sign-in, or `logout` to sign-out.  

