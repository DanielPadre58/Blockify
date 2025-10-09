namespace Blockify.Api.Configuration.External_Services_Configuration;

public record SpotifyConfiguration
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}