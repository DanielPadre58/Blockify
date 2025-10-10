namespace Blockify.Infrastructure.External.Gemini.Client;

public interface IGeminiClient
{
    Task<HttpResponseMessage> GenerateTextAsync(string prompt);
}