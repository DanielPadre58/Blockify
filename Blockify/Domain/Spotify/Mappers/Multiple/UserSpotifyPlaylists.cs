using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Multiple;

public class UserSpotifyPlaylists
{
    [JsonPropertyName("items")]
    public required List<UserSpotifyPlaylistObject> Items { get; set; }
}