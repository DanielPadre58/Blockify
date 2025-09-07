using Blockify.Application.DTOs.Authentication;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;

namespace Blockify.Application.Services.Spotify.Client;

public interface ISpotifyClient
{
    public Task<TokenDto> RefreshTokenAsync(string refreshToken);

    public Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken);

    public Task<IEnumerable<Playlist>> GetUserPlaylists(string accessToken);
}