using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Multiple;

public abstract class UserSpotifyPlaylistObject
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("tracks")]
    public required UserSpotifyTracks Tracks { get; set; }
}