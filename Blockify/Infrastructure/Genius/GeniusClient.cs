using Blockify.Api.Configuration;

namespace Blockify.Infrastructure.Genius;

public class GeniusClient : IGeniusClient
{
    private readonly HttpClient _client;
    private readonly GeniusConfiguration _configuration;

    public GeniusClient(HttpClient client, GeniusConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }
}