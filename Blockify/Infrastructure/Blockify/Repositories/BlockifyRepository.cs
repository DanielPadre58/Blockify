using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Blockify.Domain.Extensions;

using Npgsql;

using static Blockify.Infrastructure.Tools.SqlScriptsHelper;

namespace Blockify.Infrastructure.Blockify.Repositories;

public class BlockifyRepository : IBlockifyRepository
{
    private readonly NpgsqlConnection _connection;

    public BlockifyRepository(NpgsqlDataSourceBuilder sourceBuilder)
    {
        _connection =
            sourceBuilder.Build()
                .OpenConnection();
    }

    public async Task<UserDto?> InsertUserAsync(User user)
    {
        var sql = await ReadFileAsync(
            GetPath("insert_into_users.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("email", user.Email));
        command.Parameters.Add(new NpgsqlParameter("spotify_id", user.Spotify.Id));
        command.Parameters.Add(new NpgsqlParameter("spotify_profile_url", user.Spotify.Url));
        command.Parameters.Add(new NpgsqlParameter("spotify_username", user.Spotify.Username));
        command.Parameters.Add(new NpgsqlParameter("spotify_refresh_token", user.Spotify.Token.RefreshToken));
        command.Parameters.Add(new NpgsqlParameter("spotify_access_token", user.Spotify.Token.AccessToken));
        command.Parameters.Add(new NpgsqlParameter("spotify_expires_at", user.Spotify.Token.ExpiresAt));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadUserAsync();
    }

    public async Task RefreshAccessTokenAsync(long userId, TokenDto token)
    {
        var sql = await ReadFileAsync(
            GetPath("refresh_access_token.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("userId", userId));
        command.Parameters.Add(new NpgsqlParameter("refreshToken", token.RefreshToken));
        command.Parameters.Add(new NpgsqlParameter("accessToken", token.AccessToken));
        command.Parameters.Add(new NpgsqlParameter("expiresAt", token.ExpiresAt));

        await command.ExecuteNonQueryAsync();
    }

    public async Task<TokenDto?> GetTokenByIdAsync(long userId)
    {
        var sql = await ReadFileAsync(
            GetPath("select_token_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("userId", userId));

        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        if (!reader.HasRows)
            throw new Exception("User not found.");

        return new TokenDto
        {
            AccessToken = reader["spotify_access_token"].ToString()!,
            ExpiresAt = Convert.ToDateTime(reader["spotify_expires_at"]),
            ExpiresIn = Convert.ToInt32(
                (Convert.ToDateTime(reader["spotify_expires_at"]) - DateTime.Now).TotalSeconds),
            RefreshToken = reader["spotify_refresh_token"].ToString()!
        };
    }

    public async Task<UserDto?> SelectUserByIdAsync(long userId)
    {
        var sql = await ReadFileAsync(
            GetPath("select_user_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("userId", userId));

        await using var reader = command.ExecuteReader();

        return await reader.ReadUserAsync();
    }

    public async Task<UserDto?> SelectUserBySpotifyIdAsync(string spotifyId)
    {
        var sql = await ReadFileAsync(
            GetPath("select_user_by_spotify_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("spotifyId", spotifyId));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadUserAsync();
    }

    public async Task<PlaylistDto?> InsertPlaylistAsync(PlaylistDto playlist)
    {
        var sql = await ReadFileAsync(
            GetPath("insert_into_playlists.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("id", $"{playlist.OwnerId}:{playlist.SpotifyId}"));
        command.Parameters.Add(new NpgsqlParameter("userId", playlist.OwnerId));
        command.Parameters.Add(new NpgsqlParameter("spotifyId", playlist.SpotifyId));
        command.Parameters.Add(new NpgsqlParameter("keyword", playlist.Name));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadPlaylistAsync();
    }

    public async Task<List<PlaylistDto>> SelectPlaylistsAsync(string userId)
    {
        var sql = await ReadFileAsync(
            GetPath("select_all_users_playlists_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new NpgsqlParameter("userId", userId));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadPlaylistsAsync();
    }
}