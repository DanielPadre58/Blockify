namespace Blockify.Infrastructure.Blockify.Repositories;

public interface IBlockifyMigrationsManager
{
    public Task ApplyMigrationsAsync();
}