using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

using OtherEchoBot.Abstractions;

using System.Text;

namespace OtherEchoBot.Bots;

internal sealed class EchoBot : TeamsActivityHandler
{
    private const string WelcomeText = @"¡Hola! Respondo preguntas sobre el libro Drácula de Bram Stoker ¡me lo he leído entero!";

    private readonly IOpenAIService serviceOpenAI;

    public EchoBot(IOpenAIService serviceOpenAI)
    {
        this.serviceOpenAI = serviceOpenAI;
    }

    /// <inheritdoc/>
    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        if (turnContext.Activity.ChannelId == Channels.Msteams)
        {
            turnContext.Activity.RemoveRecipientMention();
        }

        // There is an open bug about the typing indicator not being shown in Teams.
        // So, We need to explicitly send a typing activity to the user to show the typing indicator.
        // References:
        //  - https://github.com/microsoft/BotFramework-Composer/issues/9679
        //  - https://github.com/microsoft/botbuilder-dotnet/issues/6752
        await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);

        await base.OnMessageActivityAsync(turnContext, cancellationToken);

        ////await SendResponseAsync(turnContext, cancellationToken);
        
        await SendResponseStreamingAsync(turnContext, cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeText, WelcomeText), cancellationToken);
            }
        }
    }

    private async Task SendResponseAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        var response = await serviceOpenAI.GetResponseAsync(turnContext.Activity.Text, cancellationToken);

        await turnContext.SendActivityAsync(MessageFactory.Text(response, response), cancellationToken);
    }


    private async Task SendResponseStreamingAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        // In some channels, like Microsoft Teams, it is possible to stream the response to the user,
        // providing a better user experience (kind of ChatGPT).
        // Sadly, the web chat does not currently support streaming responses. Reference: https://github.com/microsoft/BotFramework-WebChat/issues/4876

        var responseStream = await serviceOpenAI.GetResponseStreamingAsync(turnContext.Activity.Text, cancellationToken);

        var isFirstTime = true;
        string resourceResponseId = string.Empty;
        var responseMessageBuilder = new StringBuilder();

        await foreach (var response in responseStream)
        {
            if (!string.IsNullOrEmpty(response))
            {
                responseMessageBuilder.Append(response);
            }

            if (responseMessageBuilder.Length > 0)
            {
                if (isFirstTime)
                {
                    var resourceResponse = await turnContext.SendActivityAsync(MessageFactory.Text(responseMessageBuilder.ToString()), cancellationToken);
                    resourceResponseId = resourceResponse.Id;
                    isFirstTime = false;
                }
                else
                {
                    var responseActivity = MessageFactory.Text(responseMessageBuilder.ToString());
                    responseActivity.Id = resourceResponseId;
                    await turnContext.UpdateActivityAsync(responseActivity, cancellationToken);
                }
            }
        }
    }
}
