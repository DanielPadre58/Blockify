using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Multiple;

public abstract class UserSpotifyPlaylists
{
    [JsonPropertyName("items")]
    public required List<UserSpotifyPlaylistObject> Items { get; set; }
}