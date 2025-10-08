using System.Text;
using System.Text.Json;
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


    public async Task<HttpResponseMessage> GenerateTextAsync(string prompt)
    {
        var requestUri = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiConfiguration.Model}:generateContent";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add("x-goog-api-key", _geminiConfiguration.ApiKey);

        var requestContent = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var jsonBody = JsonSerializer.Serialize(requestContent);
        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        return response;
    }
}