using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;

namespace OtherEchoBot.Adapters;

internal sealed class BotCloudAdapterWithErrorHandler : CloudAdapter
{
    private readonly BotCloudAdapterWithErrorHandlerServices services;

    public BotCloudAdapterWithErrorHandler(BotCloudAdapterWithErrorHandlerServices services) : base(services.BotFrameworkAuthentication, services.Logger)
    {
        this.services = services;

        foreach (var middleware in services.BotMiddlewares)
        {
            Use(middleware);
        }

        OnTurnError = ErrorHandlerAsync;
    }

    private async Task ErrorHandlerAsync(ITurnContext turnContext, Exception exception)
    {
        // Log any leaked exception from the application.
        services.Logger.LogError(exception, @"[{EventName}] Unhandled error: {ErrorMessage}", nameof(OnTurnError), exception.Message);

        // Send a message to the user
        await turnContext.SendActivityAsync(@"The bot encountered an error or bug.");
        await turnContext.SendActivityAsync(@"To continue to run this bot, please fix the bot source code.");

        services.BotTelemetryClient.TrackException(exception);

        try
        {
            await services.ConversationState.DeleteAsync(turnContext);
        }
        catch (Exception ex)
        {
            services.Logger.LogError(ex, @"Exception caught on attempting to `Delete` the conversation state: {ErrorMessage}.", ex.Message);
        }

        // Send a trace activity, which will be displayed in the Bot Framework Emulator
        await turnContext.TraceActivityAsync($"{nameof(OnTurnError)} Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
    }
}
