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

    private User ReadUserQuery(NpgsqlDataReader reader)
    {
        reader.Read();

        return new()
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
    }

    public bool UsersExists(string spotifyId)
    {
        var sql = ReadFile(GetQueryPath("check_users_table_exists.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("spotifyId", spotifyId));

        var foundUser = command.ExecuteScalar();
        return foundUser != null;
    }

    public User InsertUser(User user)
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

        return ReadUserQuery(command.ExecuteReader());
    }

    public void RefreshAccessToken(long userId, TokenDto token)
    {
        var sql = ReadFile(GetQueryPath("refresh_access_token.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));
        command.Parameters.Add(new("refreshToken", token.RefreshToken));
        command.Parameters.Add(new("accessToken", token.AccessToken));
        command.Parameters.Add(new("expiresAt", token.ExpiresAt));

        command.ExecuteNonQuery();
    }

    public string? GetAccessTokenById(long userId)
    {
        var sql = ReadFile(GetQueryPath("select_access_token_by_id.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));

        var accessToken = command.ExecuteScalar();
        return accessToken?.ToString();
    }

    public User SelectUserById(long userId)
    {
        var sql = ReadFile(GetQueryPath("select_user_by_id.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("userId", userId));

        using var reader = command.ExecuteReader();

        return ReadUserQuery(reader);
    }

    public User SelectUserBySpotifyId(string spotifyId)
    {
        var sql = ReadFile(GetQueryPath("select_user_by_spotify_id.sql"));

        using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.Add(new("spotifyId", spotifyId));

        using var reader = command.ExecuteReader();

        return ReadUserQuery(reader);
    }
}