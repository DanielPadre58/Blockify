using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Multiple;

public record UserSpotifyPlaylistObject
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("tracks")]
    public required UserSpotifyTracks Tracks { get; set; }
}