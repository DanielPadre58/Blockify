using System.Text;
using System.Text.Json;
using Blockify.Infrastructure.External.Configuration;
using Microsoft.Extensions.Options;

namespace Blockify.Infrastructure.External.Gemini.Client;

public class GeminiClient : ExternalClient, IGeminiClient
{
    private readonly GeminiConfiguration _geminiConfiguration;

    public GeminiClient(HttpClient client, IOptions<GeminiConfiguration> geminiConfiguration): base(client)
    {
        _geminiConfiguration = geminiConfiguration.Value;
    }

    private new static object Prompt(string prompt) => 
        new
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
    
    public async Task<HttpResponseMessage> GenerateTextAsync(string prompt)
    {
        var requestUri = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiConfiguration.Model}:generateContent";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add("x-goog-api-key", _geminiConfiguration.ApiKey);

        var requestContent = Prompt(prompt);

        var jsonBody = JsonSerializer.Serialize(requestContent);
        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await HttpClient.SendAsync(request);
        await VerifyResponseAsync(response, request);

        return response;
    }
}