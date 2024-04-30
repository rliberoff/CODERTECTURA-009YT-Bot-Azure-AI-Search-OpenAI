using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace OtherEchoBot.Adapters;

internal record BotCloudAdapterWithErrorHandlerServices(
    BotFrameworkAuthentication BotFrameworkAuthentication,
    ConversationState ConversationState,
    IBotTelemetryClient BotTelemetryClient, 
    IEnumerable<IMiddleware> BotMiddlewares, 
    ILogger<BotCloudAdapterWithErrorHandler> Logger
    )
{
}
