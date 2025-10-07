using System.Data.Common;
using Blockify.Domain.Entities;
using Blockify.Infrastructure.Blockify.Migrations;
using Blockify.Infrastructure.Exceptions.Blockify;
using Npgsql;
using static Blockify.Infrastructure.Tools.SqlScriptsHelper;

namespace Blockify.Infrastructure.Blockify.Repositories
{
    public class BlockifyMigrationsManager : IBlockifyMigrationsManager
    {
        private readonly NpgsqlConnection _connection;

        private static string MigrationsPath => Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Blockify", "Migrations");

        public BlockifyMigrationsManager(NpgsqlDataSourceBuilder sourceBuilder)
        {
            _connection =
                sourceBuilder.Build()
                    .OpenConnection();
        }

        private static Migration GetMigrationByJsonFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("migration file path cannot be null nor empty");

            var json = File.ReadAllText(filePath);

            var migration = JsonMapper<Migration>.FromJson(json);

            return migration;
        }

        private async Task<int> GetCurrentMigrationVersion()
        {
            try
            {
                var path = GetPath("select_current_migration.sql");
                var versionSql = await ReadFileAsync(path);

                if (string.IsNullOrWhiteSpace(versionSql))
                    throw new EmptyTextFileException("SQL query file is empty.", path);

                await using var command = new NpgsqlCommand(versionSql, _connection);
                var version = Convert.ToInt32(await command.ExecuteScalarAsync());

                return version;
            }
            catch (DbException) { return -1; }
        }

        public async Task ApplyMigrationsAsync()
        {
            var version = await GetCurrentMigrationVersion();

            var migrationsAsJson = Directory
                .GetFiles(MigrationsPath, "*.json");

            var migrations = migrationsAsJson
                .Select(GetMigrationByJsonFile)
                .Where(m => m.Version > version)
                .OrderBy(m => m.Version)
                .ToList();

            if (migrations.Count == 0)
                return;

            foreach (var migration in migrations)
                await ApplyMigration(migration);
        }

        private async Task ApplyMigration(Migration migration)
        {
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var applyMigration = new NpgsqlCommand(migration.Sql, _connection, transaction);

                await applyMigration.ExecuteNonQueryAsync();
                await LogMigrationAsync(migration, transaction);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task LogMigrationAsync(Migration migration, NpgsqlTransaction transaction)
        {
            var insertSql = await ReadFileAsync(GetPath("insert_into_migrations.sql"));

            using var logCmd = new NpgsqlCommand(insertSql, _connection, transaction);
            logCmd.Parameters.AddWithValue("version", migration.Version);
            logCmd.Parameters.AddWithValue("name", migration.Name);
            await logCmd.ExecuteNonQueryAsync();
        }
    }
}
