using Blockify.Api.Configuration;
using Microsoft.Extensions.Options;

namespace Blockify.Infrastructure.Gemini;

public class GeminiClient : IGeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly GeminiConfiguration _geminiConfiguration;

    public GeminiClient(HttpClient httpClient, IOptions<GeminiConfiguration> geminiConfiguration)
    {
        _httpClient = httpClient;
        _geminiConfiguration = geminiConfiguration.Value;
    }
}