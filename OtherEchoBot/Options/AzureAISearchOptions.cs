using System.ComponentModel.DataAnnotations;

namespace OtherEchoBot.Options;

internal sealed class AzureAISearchOptions
{
    /// <summary>
    /// Gets the <see cref="Uri "/> for an Azure AI Search service instance. This should include protocol and host name.
    /// </summary>
    [Required]
    public required Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the name of the index to use in the Azure AI Search service instance.
    /// </summary>
    public required string IndexName { get; init; }

    /// <summary>
    /// Gets the key credential used to authenticate to an Azure AI Search service instance.
    /// </summary>
    [Required]
    public required string Key { get; init; }

    /// <summary>
    /// Gets the name of a semantic ranker configuration to use in the Azure AI Search service instance.
    /// </summary>
    /// <remarks>
    /// If the value of this options is <see langword="null"/> a semantic ranker won't be used, otherwise a semantic search will be performed. 
    /// </remarks>
    public string? SemanticConfiguration { get; init; }
}
