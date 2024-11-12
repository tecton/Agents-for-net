// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Agents.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Hosting;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Protocols.Connector;
using Microsoft.Agents.Authentication.Msal;
using Microsoft.Agents.BotBuilder;
using Microsoft.Agents.Memory;
using Microsoft.Agents.Samples.Bots;

namespace Microsoft.Agents.Hosting.Setup
{
    // This class sets up basic validation for tokens from Bot Service.
    // If you choose to use aspnet's built in authentication process, System.Identity.Web, you would need to configure it to support tokens sent from the Azure Bot Service. 
    public static class SampleServiceCollectionExtensions
    {

        public static IHostApplicationBuilder AddBotWithMsalAuth<T, TImpl>(this IHostApplicationBuilder builder)
            where T : IBot
            where TImpl : class, T
        {
            builder.Services.AddBotAspNetAuthentication(builder.Configuration);

            // Add default bot MsalAuth support
            builder.Services.AddDefaultMsalAuth(builder.Configuration);

            // Add Connections object to access configured token connections.
            builder.Services.AddSingleton<IConnections, ConfigurationConnections>();

            // Add factory for ConnectorClient and UserTokenClient creation
            builder.Services.AddSingleton<IChannelServiceClientFactory, RestChannelServiceClientFactory>();

            // Add the BotAdapter, this is the default adapter that works with Azure Bot Service and Activity Protocol.
            builder.Services.AddCloudAdapter();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            builder.Services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            builder.Services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            builder.Services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            builder.Services.AddSingleton<MainDialog>();

            // Add the Bot,  this is the primary worker for the bot. 
            builder.Services.AddTransient<IBot, TImpl>();

            return builder;
        }


        /// <summary>
        /// Adds default token validation typical for ABS/SMBA.  If config settings are not supplied, this will
        /// default to Azure Public Cloud.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="rootConnectionConfig"></param>
        /// <remarks>
        /// Example config:
        ///     {
        ///        "TokenValidation": {
        ///           "ValidIssuers": [
        ///              "{default:Public-Azure}"
        ///           ],
        ///           "AllowedCallers": [
        ///              "{default:*}"
        ///           ]
        ///        }
        ///     }
        /// </remarks>
        public static void AddBotAspNetAuthentication(this IServiceCollection services, IConfiguration configuration, string botConnectionConfig = "Connections:BotServiceConnection:Settings")
        {
            IConfigurationSection tokenValidationSection = configuration.GetSection("TokenValidation");

            List<string> validTokenIssuers = tokenValidationSection.GetSection("ValidIssuers").Get<List<string>>();

            // If ValidIssuers is empty, default for ABS Public Cloud
            if (validTokenIssuers == null || validTokenIssuers.Count == 0)
            {
                validTokenIssuers =
                [
                    "https://api.botframework.com",
                    "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",
                    "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",
                    "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",
                    "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0",
                ];

                string tenantId = configuration[$"{botConnectionConfig}:TenantId"];
                if (!string.IsNullOrEmpty(tenantId))
                {
                    validTokenIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidTokenIssuerUrlTemplateV1, tenantId));
                    validTokenIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidTokenIssuerUrlTemplateV2, tenantId));
                }
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuers = validTokenIssuers,
                    ValidAudience = configuration[$"{botConnectionConfig}:ClientId"],
                    RequireSignedTokens = true,
                    SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
                };
            });
        }
    }
}
