namespace Blockify.Api.Configuration;

public record SpotifyConfiguration
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}