using Blockify.Application.DTOs.Authentication;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;

namespace Blockify.Application.Services.Spotify.Client;

public interface ISpotifyClient
{
    public Task<HttpResponseMessage> RefreshTokenAsync(string refreshToken);

    public Task<HttpResponseMessage> GetPlaylistAsync(string playlistId, string accessToken);

    public Task<HttpResponseMessage> GetUserPlaylists(string accessToken);

    public Task<HttpResponseMessage> CreateKeywordPlaylist(string accessToken, string userId, string keyword);
}