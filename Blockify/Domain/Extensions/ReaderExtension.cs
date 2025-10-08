using System.Security.Cryptography;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Infrastructure.Exceptions.Blockify;
using Npgsql;

namespace Blockify.Domain.Extensions;

public static class ReaderExtension
{
    public static async Task<UserDto?> ReadUserAsync(this NpgsqlDataReader data)
    {
        try
        {
            await data.ReadAsync();

            if (!data.HasRows)
                return null;

            var user = new UserDto
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

            return user;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidColumnOperationException(ex.Message);
        }
    }

    public static async Task<TokenDto?> ReadTokenAsync(this NpgsqlDataReader data)
    {
        await data.ReadAsync();

        if (!data.HasRows)
            return null!;

        return new TokenDto
        {
            AccessToken = data["spotify_access_token"].ToString()!,
            ExpiresAt = Convert.ToDateTime(data["spotify_expires_at"]),
            ExpiresIn = Convert.ToInt32(
                (Convert.ToDateTime(data["spotify_expires_at"]) - DateTime.Now).TotalSeconds),
            RefreshToken = data["spotify_refresh_token"].ToString()!
        };
    }
    
    public static async Task<PlaylistDto> ReadPlaylistAsync(this NpgsqlDataReader data)
    {
        await data.ReadAsync();

        return new PlaylistDto
        {
            OwnerId = data["owner_id"].ToString() ?? string.Empty,
            SpotifyId = data["spotify_id"].ToString() ?? string.Empty,
            Name = data["keyword"].ToString() ?? string.Empty,
        };
    }
    
    public static async Task<List<PlaylistDto>> ReadPlaylistsAsync(this NpgsqlDataReader data)
    {
        var playlists = new List<PlaylistDto>();

        while (await data.ReadAsync())
        {
            playlists.Add(new PlaylistDto
            {
                OwnerId = data["owner_id"].ToString() ?? string.Empty,
                SpotifyId = data["spotify_id"].ToString() ?? string.Empty,
                Name = data["keyword"].ToString() ?? string.Empty,
            });
        }

        return playlists;
    }
}