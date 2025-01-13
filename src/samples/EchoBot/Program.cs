﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using EchoBot;
using Microsoft.Agents.Hosting.Setup;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.AddBot<IBot, MyBot>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("TestTool"))
{
    app.MapGet("/", () => "Microsoft Copilot SDK Sample");
    app.UseDeveloperExceptionPage();
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}
app.Run();

