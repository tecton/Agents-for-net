// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.Memory;
using Microsoft.Agents.Protocols.Adapter;
using Microsoft.Agents.Protocols.Connector;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Agents.Samples.Bots
{
    public class AuthBot : ActivityHandler
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly OAuthFlow _flow;
        private readonly IStorage _storage;

        public AuthBot(IConfiguration configuration, IStorage storage, ILogger<AuthBot> logger)
        {
            _logger = logger ?? NullLogger<AuthBot>.Instance;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _flow = new OAuthFlow("Sign In", "Please sign in", _configuration["ConnectionName"], 30000, null);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Welcome to AuthenticationBot. Type anything to get logged in. Type 'logout' to sign-out."), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            if (string.Equals("logout", turnContext.Activity.Text, StringComparison.OrdinalIgnoreCase))
            {
                await _flow.SignOutUserAsync(turnContext, cancellationToken);
                await turnContext.SendActivityAsync(MessageFactory.Text("You have been signed out."), cancellationToken);
            }
            else
            {
                TokenResponse tokenResponse = null;

                // Read flow state for this conversation
                var stateKey = GetStorageKey(turnContext);
                var items = await _storage.ReadAsync([stateKey], cancellationToken);
                FlowState state = items.TryGetValue(stateKey, out object value) ? (FlowState)value : new FlowState();

                if (!state.FlowStarted)
                {
                    tokenResponse = await _flow.BeginFlowAsync(turnContext, null, cancellationToken);

                    // If a TokenResponse is returned, there was a cached token already.  Otherwise, start the process of getting a new token.
                    if (tokenResponse == null)
                    {
                        var expires = DateTime.UtcNow.AddMilliseconds(_flow.Timeout ?? TimeSpan.FromMinutes(15).TotalMilliseconds);

                        state.FlowStarted = true;
                        state.FlowExpires = expires;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("You are still logged in."), cancellationToken);
                    }
                }
                else
                {
                    try
                    {
                        tokenResponse = await _flow.ContinueFlowAsync(turnContext, state.FlowExpires, cancellationToken);
                        if (tokenResponse != null)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("You are now logged in."), cancellationToken);
                        }
                        else
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("Login was not successful please try again."), cancellationToken);
                        }
                    }
                    catch (TimeoutException)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("You did not respond in time.  Please try again."), cancellationToken);
                    }

                    state.FlowStarted = false;
                }

                // Store flow state
                items[stateKey] = state;
                await _storage.WriteAsync(items, cancellationToken);


                if (tokenResponse != null)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Here is your token {tokenResponse.Token}"), cancellationToken);
                }
            }
        }

        protected override Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Response Event Activity (Not handled).");
            return Task.CompletedTask;
        }

        private static string GetStorageKey(ITurnContext turnContext)
        {
            var channelId = turnContext.Activity.ChannelId ?? throw new InvalidOperationException("invalid activity-missing channelId");
            var conversationId = turnContext.Activity.Conversation?.Id ?? throw new InvalidOperationException("invalid activity-missing Conversation.Id");
            return $"{channelId}/conversations/{conversationId}/flowState";
        }
    }

    class FlowState
    {
        public bool FlowStarted = false;
        public DateTime FlowExpires = DateTime.MinValue;
    }
}
