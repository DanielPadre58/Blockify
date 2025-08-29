using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;

namespace Blockify.Domain.Database
{
    public interface IBlockifyDbService
    {
        public void CreateUsersTable();
        public bool UsersExists(string spotifyId);
        public User InsertUser(User user);
        public void RefreshAccessToken(long userId, TokenDto token);
        public string? GetAccessTokenById(long userId);
        public User? SelectUserById(long userId);
        public User? SelectUserBySpotifyId(string userId);
    }
}
