using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Multiple;

public class UserSpotifyPlaylistObject
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("tracks")]
    public required UserSpotifyTracks Tracks { get; set; }
}