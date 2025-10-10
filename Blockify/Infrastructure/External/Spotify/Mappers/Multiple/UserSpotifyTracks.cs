using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Multiple;

public abstract class UserSpotifyTracks
{
    [JsonPropertyName("href")]
    public required string Href { get; set; }

    [JsonPropertyName("total")]
    public required int Total { get; set; }
}