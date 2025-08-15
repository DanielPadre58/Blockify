using Blockify.Domain;

namespace Blockify.Api;

public static class DependencyInjector{
    public static void Inject(IServiceCollection services, IConfiguration configuration){
        services.AddScoped<IBlockifyDbService, BlockifyDbService>(
            _ => new BlockifyDbService(configuration.GetConnectionString("BlockifyDb")!)
        );
    }
}