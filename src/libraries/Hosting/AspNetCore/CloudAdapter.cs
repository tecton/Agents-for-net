// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Agents.Protocols.Adapter;
using Microsoft.Agents.Protocols.Connector;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Extensions.Logging;

namespace Microsoft.Agents.Hosting.AspNetCore
{
    /// <summary>
    /// An adapter that implements the Activity Protocol. It can be hosted in different cloud environments, both public and private.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CloudAdapter"/> class.
    /// </remarks>
    public class CloudAdapter : CloudAdapterBase, IBotHttpAdapter
    {
        /// <param name="channelServiceClientFactory">The <see cref="IChannelServiceClientFactory"/> this adapter should use.</param>
        /// <param name="logger">The <see cref="ILogger"/> implementation this adapter should use.</param>
        public CloudAdapter(
            IChannelServiceClientFactory channelServiceClientFactory,
            ILogger logger = null) : base(channelServiceClientFactory, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                StringBuilder sbError = new StringBuilder(1024);
                sbError.Append(exception.Message);
                if (exception is ErrorResponseException errorResponse)
                {
                    sbError.Append(Environment.NewLine);
                    sbError.Append(errorResponse.Body.ToString());
                }
                string resolvedErrorMessage = sbError.ToString();
                logger.LogError(exception, "Exception caught : {ExceptionMessage}", resolvedErrorMessage);  
                
                await turnContext.SendActivityAsync(MessageFactory.Text(resolvedErrorMessage));

                // Send a trace activity
                await turnContext.TraceActivityAsync("OnTurnError Trace", resolvedErrorMessage, "https://www.botframework.com/schemas/error", "TurnError");
                sbError.Clear(); 
            };
        }

        /// <summary>
        /// Process the inbound HTTP request with the agent resulting in the outbound HTTP response. This method can be called directly from a Controller.
        /// If the HTTP method is a POST, the body will contain the <see cref="Activity"/> to process. 
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <param name="bot">The <see cref="IBot"/> implementation to use for this request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public virtual async Task ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(httpRequest);
            ArgumentNullException.ThrowIfNull(httpResponse);
            ArgumentNullException.ThrowIfNull(bot);

            try
            {
                ClaimsIdentity claimsIdentity = httpRequest.HttpContext.User.Identity as ClaimsIdentity;

                // Only POST requests are handled
                if (httpRequest.Method == HttpMethods.Post)
                {
                    // Deserialize the incoming Activity
                    var activity = await HttpHelper.ReadRequestAsync<Activity>(httpRequest).ConfigureAwait(false);

                    // A request must contain an Activity body
                    if (string.IsNullOrEmpty(activity?.Type))
                    {
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        Logger.LogWarning("BadRequest: Missing activity or activity type.");
                        return;
                    }

                    // Process the inbound activity with the bot
                    var invokeResponse = await ProcessActivityAsync(claimsIdentity, activity, bot.OnTurnAsync, cancellationToken).ConfigureAwait(false);

                    // Write the response, potentially serializing the InvokeResponse
                    await HttpHelper.WriteResponseAsync(httpResponse, invokeResponse).ConfigureAwait(false);
                }
                else
                {
                    httpResponse.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // handle unauthorized here as this layer creates the http response
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;

                Logger.LogError(ex, "Unauthorized: {ExceptionMessage}", ex.ToString());
            }
        }
    }
}
