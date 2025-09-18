using System.Text;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Blockify.Domain.Extensions;
using Blockify.Infrastructure.Tools;
using Npgsql;

namespace Blockify.Infrastructure.Blockify.Repositories;

public class BlockifyDbService : IBlockifyDbService
{
    private readonly NpgsqlConnection _connection;

    public BlockifyDbService(NpgsqlDataSourceBuilder sourceBuilder)
    {
        _connection =
            sourceBuilder.Build()
                .OpenConnection();
        Task.WaitAll(ApplyMigrations());
    }

    private async Task ApplyMigrations()
    {
        int version;
        try
        {
            var versionSql = SqlScriptsHelper.ReadFile(
                SqlScriptsHelper.GetQueryPath("select_current_migration.sql"));

            if (string.IsNullOrWhiteSpace(versionSql))
            {
                throw new InvalidOperationException("SQL migration file is empty.");
            }

            await using var command = new NpgsqlCommand(versionSql, _connection);
            version = Convert.ToInt32(await command.ExecuteScalarAsync() ?? "-1");
        }
        catch (Exception)
        {
            var createMigrationsTableSql = SqlScriptsHelper.ReadFile(
                SqlScriptsHelper.GetMigrationPath("0_create_migrations_table.sql")
            );

            if (string.IsNullOrWhiteSpace(createMigrationsTableSql))
            {
                throw new InvalidOperationException("SQL migration file is empty.");
            }

            var initMigrationCmd = new NpgsqlBatch(_connection);
            await using var initMigrationTransaction = await _connection.BeginTransactionAsync();
            initMigrationCmd.Transaction = initMigrationTransaction;

            initMigrationCmd.BatchCommands.Add(new(createMigrationsTableSql));
            initMigrationCmd.BatchCommands.Add(
                new(
                    SqlScriptsHelper.ReadFile(
                        SqlScriptsHelper.GetQueryPath("insert_into_migrations.sql")))
                {
                    Parameters =
                    {
                        new("version", int.Parse("0")),
                        new("filename", "0_create_migrations_table.sql")
                    }
                }
            );

            await initMigrationCmd.ExecuteNonQueryAsync();
            await initMigrationTransaction.CommitAsync();

            version = 0;
        }

        var migrations = Directory
            .GetFiles(
                Path.Combine(AppContext.BaseDirectory, "Domain", "Database", "Migrations"),
                "*.sql"
            )
            .OrderBy(m => m)
            .ToList();

        migrations.RemoveAll(m =>
        {
            var migrationVersion = SqlScriptsHelper.GetMigrationVersion(m);

            return migrationVersion <= version;
        });

        if (migrations.Count == 0)
            return;

        var commandSql = new StringBuilder();
        var migrationsBatch = new NpgsqlBatch(_connection);
        foreach (var file in migrations)
        {
            commandSql.Append(SqlScriptsHelper.ReadFile(file));
            migrationsBatch.BatchCommands
                .Add(new(
                    SqlScriptsHelper.ReadFile(
                        SqlScriptsHelper.GetQueryPath("insert_into_migrations.sql")))
                {
                    Parameters =
                    {
                        new("version", SqlScriptsHelper.GetMigrationVersion(Path.GetFileName(file))),
                        new("filename", Path.GetFileName(file))
                    }
                });
        }

        var batch = new NpgsqlCommand(commandSql.ToString(), _connection);
        await batch.ExecuteNonQueryAsync();
        await migrationsBatch.ExecuteNonQueryAsync();
    }

    public async Task<bool> UsersExistsAsync(string spotifyId)
    {
        var sql = SqlScriptsHelper.ReadFile
            (SqlScriptsHelper.GetQueryPath("check_users_table_exists.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("spotifyId", spotifyId));

        var userExists = (bool)(await command.ExecuteScalarAsync() ?? false);

        return userExists;
    }

    public async Task<UserDto?> InsertUserAsync(User user)
    {
        var sql = SqlScriptsHelper.ReadFile(
            SqlScriptsHelper.GetQueryPath("insert_into_users.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("email", user.Email));
        command.Parameters.Add(new("spotify_id", user.Spotify.Id));
        command.Parameters.Add(new("spotify_profile_url", user.Spotify.Url));
        command.Parameters.Add(new("spotify_username", user.Spotify.Username));
        command.Parameters.Add(new("spotify_refresh_token", user.Spotify.Token.RefreshToken));
        command.Parameters.Add(new("spotify_access_token", user.Spotify.Token.AccessToken));
        command.Parameters.Add(new("spotify_expires_at", user.Spotify.Token.ExpiresAt));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadUserAsync();
    }

    public async Task RefreshAccessTokenAsync(long userId, TokenDto token)
    {
        var sql = SqlScriptsHelper.ReadFile(
            SqlScriptsHelper.GetQueryPath("refresh_access_token.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));
        command.Parameters.Add(new("refreshToken", token.RefreshToken));
        command.Parameters.Add(new("accessToken", token.AccessToken));
        command.Parameters.Add(new("expiresAt", token.ExpiresAt));

        await command.ExecuteNonQueryAsync();
    }

    public async Task<TokenDto> GetTokenByIdAsync(long userId)
    {
        var sql = SqlScriptsHelper.ReadFile(
            SqlScriptsHelper.GetQueryPath("select_token_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));

        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        if (!reader.HasRows)
            throw new Exception("User not found.");

        return new TokenDto
        {
            AccessToken = reader["spotify_access_token"].ToString()!,
            ExpiresAt = Convert.ToDateTime(reader["spotify_expires_at"]),
            RefreshToken = reader["spotify_refresh_token"].ToString()!
        };
    }

    public async Task<UserDto?> SelectUserByIdAsync(long userId)
    {
        var sql = SqlScriptsHelper.ReadFile(
            SqlScriptsHelper.GetQueryPath("select_user_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));

        await using var reader = command.ExecuteReader();

        return await reader.ReadUserAsync();
    }

    public async Task<UserDto?> SelectUserBySpotifyIdAsync(string spotifyId)
    {
        var sql = SqlScriptsHelper.ReadFile(
            SqlScriptsHelper.GetQueryPath("select_user_by_spotify_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("spotifyId", spotifyId));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadUserAsync();
    }

    public async Task<PlaylistDto?> InsertPlaylistAsync(PlaylistDto playlist)
    {
        var sql = SqlScriptsHelper.ReadFile(
            SqlScriptsHelper.GetQueryPath("insert_into_playlists.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("id", $"{playlist.OwnerId}:{playlist.SpotifyId}"));
        command.Parameters.Add(new("userId", playlist.OwnerId));
        command.Parameters.Add(new("spotifyId", playlist.SpotifyId));

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadPlaylistAsync();
    }
}