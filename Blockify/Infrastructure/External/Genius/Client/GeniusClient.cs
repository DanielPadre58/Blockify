using Blockify.Infrastructure.External.Configuration;
using Microsoft.Extensions.Options;

namespace Blockify.Infrastructure.External.Genius.Client;

public class GeniusClient : ExternalClient, IGeniusClient
{
    private readonly GeniusConfiguration _configuration;

    public GeniusClient(HttpClient client, IOptions<GeniusConfiguration> configuration) : base(client)
    {
        _configuration = configuration.Value;
    }

    public async Task<HttpResponseMessage> SearchSongId(string name, string artist)
    {
        var requestUri = $"https://api.genius.com/search?q={Uri.EscapeDataString(name)}%20{Uri.EscapeDataString(artist)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("Authorization", $"Bearer {_configuration.ClientSecret}");

        var response = await HttpClient.SendAsync(request);
        await VerifyResponseAsync(response, request);

        return response;
    }
}