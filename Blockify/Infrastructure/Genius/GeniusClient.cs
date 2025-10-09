using Blockify.Api.Configuration.External_Services_Configuration;

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