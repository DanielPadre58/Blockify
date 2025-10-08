using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Multiple;

public class UserSpotifyTracks
{
    [JsonPropertyName("href")]
    public required string Href { get; set; }

    [JsonPropertyName("total")]
    public required int Total { get; set; }
}