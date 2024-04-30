using Azure.AI.OpenAI;
using System.ComponentModel.DataAnnotations;

namespace OtherEchoBot.Options;

public sealed class AzureOpenAIOptions
{
    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for chat.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: The model deployment name does not necessarily have to be the same as the model name. For example, a model of type `gpt-4` might be called «MyGPT»;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it. Use property <see cref="ChatModelName"/> to set the model name.
    /// </remarks>
    [Required]
    public required string ChatModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for embeddings.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: The model name does not necessarily have to be the same as the model ID. For example, a model of type `text-embedding-ada-002` might be called `MyEmbeddings`;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it. Use property <see cref="EmbeddingsModelName"/> to set the model name.
    /// </remarks>
    [Required]
    public required string EmbeddingsModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the Azure OpenAI API service version.
    /// </summary>
    [Required]
    public OpenAIClientOptions.ServiceVersion ServiceVersion { get; init; } = OpenAIClientOptions.ServiceVersion.V2024_03_01_Preview;

    /// <summary>
    /// Gets the <see cref="Uri "/> for an LLM resource (like OpenAI). This should include protocol and host name.
    /// </summary>
    [Required]
    public required Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the key credential used to authenticate to an LLM resource.
    /// </summary>
    [Required]
    public required string Key { get; init; }
}
