using System.Text;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Npgsql;

namespace Blockify.Domain.Database;

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

    private static string GetMigrationPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "Domain", "Database", "Migrations", fileName);

    private static int GetMigrationVersion(string fileName) =>
        int.Parse(Path.GetFileName(fileName).Split('_').First());

    private static string GetQueryPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "Domain", "Database", "Queries", fileName);

    private static string ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("SQL migration file not found.", path);
        }

        return File.ReadAllText(path);
    }

    private async Task ApplyMigrations()
    {
        int version;
        try
        {
            var versionSql = ReadFile(GetQueryPath("select_current_migration.sql"));

            if (string.IsNullOrWhiteSpace(versionSql))
            {
                throw new InvalidOperationException("SQL migration file is empty.");
            }

            await using var command = new NpgsqlCommand(versionSql, _connection);
            version = Convert.ToInt32(await command.ExecuteScalarAsync() ?? "-1");
        }
        catch (Exception)
        {
            var createMigrationsTableSql = ReadFile(
                GetMigrationPath("0_create_migrations_table.sql")
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
                new(ReadFile(GetQueryPath("insert_into_migrations.sql")))
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
            var migrationVersion = GetMigrationVersion(m);

            return migrationVersion <= version;
        });

        if (migrations.Count == 0)
            return;

        var commandSql = new StringBuilder();
        var migrationsBatch = new NpgsqlBatch(_connection);
        foreach (var file in migrations)
        {
            commandSql.Append(ReadFile(file));
            migrationsBatch.BatchCommands
                .Add(new(ReadFile(GetQueryPath("insert_into_migrations.sql")))
                {
                    Parameters =
                    {
                        new("version", GetMigrationVersion(Path.GetFileName(file))),
                        new("filename", Path.GetFileName(file))
                    }
                });
        }

        var batch = new NpgsqlCommand(commandSql.ToString(), _connection);
        await batch.ExecuteNonQueryAsync();
        await migrationsBatch.ExecuteNonQueryAsync();
    }

    private async Task<User> ReadUserQueryAsync(NpgsqlDataReader reader)
    {
        await reader.ReadAsync();
        
        var user = new User
        {
            Id = Convert.ToInt64(reader["id"]),
            Email = reader["email"].ToString()!,
            Spotify = new SpotifyDto
            {
                Id = reader["spotify_id"].ToString()!,
                Url = reader["spotify_profile_url"].ToString()!,
                Username = reader["spotify_username"].ToString()!,
                Token = new TokenDto
                {
                    RefreshToken = reader["spotify_refresh_token"].ToString()!,
                    AccessToken = reader["spotify_access_token"].ToString()!,
                    ExpiresAt = Convert.ToDateTime(reader["spotify_expires_at"])
                }
            },
            CreationDate = Convert.ToDateTime(reader["created_at"]),
            LastRequestDate = Convert.ToDateTime(reader["updated_at"])
        };

        return user;
    }

    public async Task<bool> UsersExistsAsync(string spotifyId)
    {
        var sql = ReadFile(GetQueryPath("check_users_table_exists.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("spotifyId", spotifyId));

        var userExists = (bool)(await command.ExecuteScalarAsync() ?? false);
        
        return userExists;
    }

    public async Task<User> InsertUserAsync(User user)
    {
        var sql = ReadFile(GetQueryPath("insert_into_users.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("email", user.Email));
        command.Parameters.Add(new("spotify_id", user.Spotify.Id));
        command.Parameters.Add(new("spotify_profile_url", user.Spotify.Url));
        command.Parameters.Add(new("spotify_username", user.Spotify.Username));
        command.Parameters.Add(new("spotify_refresh_token", user.Spotify.Token.RefreshToken));
        command.Parameters.Add(new("spotify_access_token", user.Spotify.Token.AccessToken));
        command.Parameters.Add(new("spotify_expires_at", user.Spotify.Token.ExpiresAt));

        await using var reader = await command.ExecuteReaderAsync();
        
        return await ReadUserQueryAsync(reader);
    }

    public async Task RefreshAccessTokenAsync(long userId, TokenDto token)
    {
        var sql = ReadFile(GetQueryPath("refresh_access_token.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));
        command.Parameters.Add(new("refreshToken", token.RefreshToken));
        command.Parameters.Add(new("accessToken", token.AccessToken));
        command.Parameters.Add(new("expiresAt", token.ExpiresAt));

        await command.ExecuteNonQueryAsync();
    }

    public async Task<string?> GetAccessTokenByIdAsync(long userId)
    {
        var sql = ReadFile(GetQueryPath("select_access_token_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));

        var accessToken = await command.ExecuteScalarAsync();
        return accessToken?.ToString();
    }

    public async Task<User?> SelectUserByIdAsync(long userId)
    {
        var sql = ReadFile(GetQueryPath("select_user_by_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));

        await using var reader = command.ExecuteReader();

        return await ReadUserQueryAsync(reader);
    }

    public async Task<User?> SelectUserBySpotifyIdAsync(string spotifyId)
    {
        var sql = ReadFile(GetQueryPath("select_user_by_spotify_id.sql"));

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("spotifyId", spotifyId));

        await using var reader = await command.ExecuteReaderAsync();

        return await ReadUserQueryAsync(reader);
    }
}