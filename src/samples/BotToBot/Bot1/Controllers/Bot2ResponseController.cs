// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.Client;
using Microsoft.Agents.Hosting.AspNetCore;

namespace Microsoft.Agents.Samples.Bots
{
    // A controller that handles channel replies to the bot.
    [Authorize]
    [ApiController]
    [Route("api/botresponse")]
    public class Bot2ResponseController(IChannelResponseHandler handler) : ChannelServiceController(handler)
    {
    }
}
