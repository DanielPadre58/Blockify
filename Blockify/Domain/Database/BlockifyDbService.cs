using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Npgsql;

namespace Blockify.Domain.Database
{
    public class BlockifyDbService : IBlockifyDbService
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly NpgsqlConnection _connection;

        public BlockifyDbService(NpgsqlDataSourceBuilder sourceBuilder)
        {
            _dataSource = sourceBuilder.Build();
            _connection = _dataSource.OpenConnection();
            AddExtensions();
        }

        private static string GetMigrationFilePath(string fileName) =>
            Path.Combine(AppContext.BaseDirectory, "Domain", "Migrations", fileName);

        private static string ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("SQL migration file not found.", path);
            }

            return File.ReadAllText(path);
        }

        public void AddExtensions()
        {
            var sql = ReadFile(GetMigrationFilePath("001_add_extensions.sql"));

            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new InvalidOperationException("SQL migration file is empty.");
            }

            using var command = new NpgsqlCommand(sql, _connection);
            command.ExecuteNonQuery();
        }

        public User ReadUserQuery(NpgsqlDataReader reader)
        {
            reader.Read();

            return new User()
            {
                Id = Convert.ToInt64(reader["id"]),
                Email = reader["email"].ToString()!,
                Spotify = new User.SpotifyData
                {
                    Id = reader["spotify_id"].ToString()!,
                    Url = reader["spotify_profile_url"].ToString()!,
                    Username = reader["spotify_username"].ToString()!,
                    RefreshToken = reader["spotify_refresh_token"].ToString()!,
                    AccessToken = reader["spotify_access_token"].ToString()!,
                },
                CreationDate = Convert.ToDateTime(reader["created_at"]),
                LastRequestDate = Convert.ToDateTime(reader["updated_at"]),
            };
        }

        public void CreateUsersTable()
        {
            var sql = ReadFile(GetMigrationFilePath("001_create_users_table.sql"));

            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new InvalidOperationException("SQL migration file is empty.");
            }

            using var command = new NpgsqlCommand(sql, _connection);
            command.ExecuteNonQuery();
        }

        public bool UsersExists(string spotifyId)
        {
            var sql = ReadFile(GetMigrationFilePath("001_check_users_table_exists.sql"));

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters = { new NpgsqlParameter("spotifyId", spotifyId) },
            };

            var foundUser = command.ExecuteScalar();
            return foundUser != null;
        }

        public User InsertUser(User user)
        {
            var sql = ReadFile(GetMigrationFilePath("001_insert_into_users.sql"));

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("email", user.Email),
                    new NpgsqlParameter("spotify_id", user.Spotify.Id),
                    new NpgsqlParameter("spotify_profile_url", user.Spotify.Url),
                    new NpgsqlParameter("spotify_username", user.Spotify.Username),
                    new NpgsqlParameter("spotify_refresh_token", user.Spotify.RefreshToken),
                    new NpgsqlParameter("spotify_access_token", user.Spotify.AccessToken),
                },
            };

            return ReadUserQuery(command.ExecuteReader());
        }

        public void RefreshAccessToken(long userId, TokenDto token)
        {
            var sql = ReadFile(GetMigrationFilePath("001_refresh_access_token.sql"));

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("userId", userId),
                    new NpgsqlParameter("refreshToken", token.RefreshToken),
                    new NpgsqlParameter("accessToken", token.AccessToken),
                },
            };

            command.ExecuteNonQuery();
        }

        public string? GetAccessTokenById(long userId)
        {
            var sql = ReadFile(GetMigrationFilePath("001_select_access_token_by_id.sql"));

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters = { new NpgsqlParameter("userId", userId) },
            };

            var accessToken = command.ExecuteScalar();
            return accessToken?.ToString();
        }

        public User? SelectUserById(long userId)
        {
            var sql = ReadFile(GetMigrationFilePath("001_select_user_by_id.sql"));

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters = { new NpgsqlParameter("userId", userId) },
            };

            using var reader = command.ExecuteReader();

            return ReadUserQuery(reader);
        }

        public User? SelectUserBySpotifyId(string spotifyId)
        {
            var sql = ReadFile(GetMigrationFilePath("001_select_user_by_spotify_id.sql"));

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters = { new NpgsqlParameter("spotifyId", spotifyId) },
            };

            using var reader = command.ExecuteReader();

            return ReadUserQuery(reader);
        }
    }
}
