using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;

namespace Blockify.Infrastructure.Blockify.Repositories;

public interface IBlockifyRepository
{
    public Task<UserDto?> InsertUserAsync(User user);
    public Task RefreshAccessTokenAsync(long userId, TokenDto token);
    public Task<TokenDto?> GetTokenByIdAsync(long userId);
    public Task<UserDto?> SelectUserByIdAsync(long userId);
    public Task<UserDto?> SelectUserBySpotifyIdAsync(string userId);
    public Task<PlaylistDto?> InsertPlaylistAsync(PlaylistDto playlist);
    public Task<List<PlaylistDto>> SelectPlaylistsAsync(string userId);
}