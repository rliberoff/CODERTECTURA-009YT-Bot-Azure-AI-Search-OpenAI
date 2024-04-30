﻿using Microsoft.AspNetCore.Mvc;

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;

namespace OtherEchoBot.Controllers;

/// <summary>
/// Controller to handle bot requests.
/// </summary>
[ApiController]
[Route(@"api/messages")]
public class BotController : ControllerBase
{
    private readonly IBot bot;
    private readonly IBotFrameworkHttpAdapter adapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotController"/> class.
    /// </summary>
    /// <param name="adapter">A bot adapter to use.</param>
    /// <param name="bot">A bot that can operate on incoming activities.</param>
    public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
    {
        this.adapter = adapter;
        this.bot = bot;
    }

    /// <summary>
    /// Handles a request for the bot.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation of handling the request for the bot.
    /// </returns>
    [HttpGet]
    [HttpPost]
    public Task HandleAsync()
    {
        return adapter.ProcessAsync(Request, Response, bot);
    }
}
