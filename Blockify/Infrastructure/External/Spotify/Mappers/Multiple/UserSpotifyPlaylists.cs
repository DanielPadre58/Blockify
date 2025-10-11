using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Multiple;

public record UserSpotifyPlaylists
{
    [JsonPropertyName("items")]
    public required List<UserSpotifyPlaylistObject> Items { get; set; }
}