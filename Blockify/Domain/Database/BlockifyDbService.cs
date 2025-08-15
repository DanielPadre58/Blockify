using Npgsql;

namespace Blockify.Domain.Database
{
    public class BlockifyDbService : IBlockifyDbService{
        private readonly NpgsqlConnection _connection;

        public BlockifyDbService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }
    }
}