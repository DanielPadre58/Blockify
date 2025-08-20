using Blockify.Domain.Database;
using Blockify.Shared.Exceptions;
using Npgsql;

namespace Blockify.Api.Configuration
{
    public static class DependencyInjector{
        public static void Inject(IServiceCollection services, IConfiguration configuration){
            services.AddScoped<IBlockifyDbService, BlockifyDbService>(
                options =>
                {
                    var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Blockify"));
                    return new BlockifyDbService(npgsqlDataSourceBuilder);
                }
            );
        }
    }
}