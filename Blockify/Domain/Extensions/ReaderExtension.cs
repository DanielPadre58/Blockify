using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Npgsql;

namespace Blockify.Domain.Extensions;

public static class ReaderExtension
{
    public async static Task<UserDto> ReadUserAsync(this NpgsqlDataReader data)
    {
        await data.ReadAsync();

        return new UserDto
        {
            Id = Convert.ToInt64(data["id"]),
            Email = data["email"].ToString() ?? string.Empty,
            Spotify = new SpotifyDto
            {
                Id = data["spotify_id"].ToString() ?? string.Empty,
                Url = data["spotify_profile_url"].ToString() ?? string.Empty,
                Username = data["spotify_username"].ToString() ?? string.Empty,
                Token = new TokenDto
                {
                    RefreshToken = data["spotify_refresh_token"].ToString() ?? string.Empty,
                    AccessToken = data["spotify_access_token"].ToString() ?? string.Empty,
                    ExpiresAt = Convert.ToDateTime(data["spotify_expires_at"]),
                    ExpiresIn = Convert.ToInt32(
                        (Convert.ToDateTime(data["spotify_expires_at"]) - DateTime.Now).TotalSeconds)
                }
            }
        };

    }
    
    public async static Task<PlaylistDto> ReadPlaylistAsync(this NpgsqlDataReader data)
    {
        await data.ReadAsync();

        return new PlaylistDto
        {
            OwnerId = data["owner_id"].ToString() ?? string.Empty,
            SpotifyId = data["spotify_id"].ToString() ?? string.Empty,
        };
    }
}