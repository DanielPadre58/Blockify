using Npgsql;

namespace Blockify.Domain;

public class BlockifyDbService : IBlockifyDbService{
    private readonly NpgsqlConnection _connection;

    public BlockifyDbService(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }
}