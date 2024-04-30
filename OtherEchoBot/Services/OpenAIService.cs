using Azure;
using Azure.AI.OpenAI;

using OtherEchoBot.Abstractions;
using Microsoft.Extensions.Options;
using OtherEchoBot.Options;

namespace OtherEchoBot.Services;

internal sealed class OpenAIService : IOpenAIService
{
    private AzureAISearchOptions optionsAzureAISearch;
    private AzureOpenAIOptions optionsAzureOpenAIOptions;

    public OpenAIService(IOptionsMonitor<AzureAISearchOptions> optionsMonitorAzureAISearch, IOptionsMonitor<AzureOpenAIOptions> optionsMonitorAzureOpenAI)
    {
        optionsAzureAISearch = optionsMonitorAzureAISearch.CurrentValue;
        optionsAzureOpenAIOptions = optionsMonitorAzureOpenAI.CurrentValue;

        optionsMonitorAzureAISearch.OnChange(newOptions => optionsAzureAISearch = newOptions);
        optionsMonitorAzureOpenAI.OnChange(newOptions => optionsAzureOpenAIOptions = newOptions);
    }

    /// <inheritdoc/>
    public async Task<string> GetResponseAsync(string message, CancellationToken cancellationToken)
    {
        var response = await BuildOpenAIClient().GetChatCompletionsAsync(BuildChatCompletionsOptions(message), cancellationToken);

        return response.Value.Choices[0].Message.Content;
    }

    /// <inheritdoc/>
    public async Task<IAsyncEnumerable<string>> GetResponseStreamingAsync(string message, CancellationToken cancellationToken)
    {
        var response = await BuildOpenAIClient().GetChatCompletionsStreamingAsync(BuildChatCompletionsOptions(message), cancellationToken);

        return response.Select(r => r.ContentUpdate);
    }

    private ChatCompletionsOptions BuildChatCompletionsOptions(string message)
    {
        return new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatRequestUserMessage(message),
            },
            AzureExtensionsOptions = new AzureChatExtensionsOptions()
            {
                Extensions =
                {
                    new AzureSearchChatExtensionConfiguration
                    {
                        SearchEndpoint = optionsAzureAISearch.Endpoint,
                        IndexName = optionsAzureAISearch.IndexName,
                        Authentication = new OnYourDataApiKeyAuthenticationOptions(optionsAzureAISearch.Key),
                        SemanticConfiguration = optionsAzureAISearch.SemanticConfiguration,
                        QueryType = string.IsNullOrWhiteSpace( optionsAzureAISearch.SemanticConfiguration) ? AzureSearchQueryType.VectorSimpleHybrid : AzureSearchQueryType.VectorSemanticHybrid,
                        Strictness = 3,
                        DocumentCount = 5,
                        VectorizationSource = new OnYourDataDeploymentNameVectorizationSource(optionsAzureOpenAIOptions.EmbeddingsModelDeploymentName),
                    },
                }
            },
            DeploymentName = optionsAzureOpenAIOptions.ChatModelDeploymentName,
        };
    }

    private OpenAIClient BuildOpenAIClient()
    {
        

        return new OpenAIClient(optionsAzureOpenAIOptions.Endpoint, new AzureKeyCredential(optionsAzureOpenAIOptions.Key), new OpenAIClientOptions(optionsAzureOpenAIOptions.ServiceVersion));
    }
}
