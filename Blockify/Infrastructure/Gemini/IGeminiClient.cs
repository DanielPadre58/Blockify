namespace Blockify.Infrastructure.Gemini;

public interface IGeminiClient
{
    Task<HttpResponseMessage> GenerateTextAsync(string prompt);
}