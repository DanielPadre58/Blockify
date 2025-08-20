using Npgsql;

namespace Blockify.Domain.Database
{
    public class BlockifyDbService : IBlockifyDbService{
        private readonly NpgsqlDataSource _dataSource;
        private readonly NpgsqlConnection _connection;

        public BlockifyDbService(NpgsqlDataSourceBuilder sourceBuilder)
        {
            _dataSource = sourceBuilder.Build();
            _connection = _dataSource.OpenConnection();
        }
    }
}