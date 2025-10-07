namespace Blockify.Api.Configuration;

public record SpotifyConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}