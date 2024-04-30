namespace OtherEchoBot.Abstractions;

public interface IOpenAIService
{
    Task<string> GetResponseAsync(string message, CancellationToken cancellationToken);

    Task<IAsyncEnumerable<string>> GetResponseStreamingAsync(string message, CancellationToken cancellationToken);
}
