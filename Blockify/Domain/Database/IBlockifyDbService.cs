using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;

namespace Blockify.Domain.Database;

public interface IBlockifyDbService
{
    public Task<bool> UsersExistsAsync(string spotifyId);
    public Task<User> InsertUserAsync(User user);
    public Task RefreshAccessTokenAsync(long userId, TokenDto token);
    public Task<string?> GetAccessTokenByIdAsync(long userId);
    public Task<User?> SelectUserByIdAsync(long userId);
    public Task<User?> SelectUserBySpotifyIdAsync(string userId);
}