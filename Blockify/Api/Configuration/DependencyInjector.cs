using Blockify.Domain.Database;

namespace Blockify.Api.Configuration
{
    public static class DependencyInjector{
        public static void Inject(IServiceCollection services, IConfiguration configuration){
            services.AddScoped<IBlockifyDbService, BlockifyDbService>(
                options => new BlockifyDbService(configuration.GetConnectionString("BlockifyDb")!)
            );
        }
    }
}