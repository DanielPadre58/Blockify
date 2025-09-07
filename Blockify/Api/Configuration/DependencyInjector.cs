using Blockify.Application.Services;
using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify;
using Blockify.Application.Services.Spotify.Client;
using Blockify.Domain.Database;
using Blockify.Shared.Exceptions;
using Npgsql;

namespace Blockify.Api.Configuration;

public static class DependencyInjector
{
    public static void Inject(IServiceCollection services, IConfiguration configuration)
    {
        var test = configuration.GetConnectionString("BlockifyDb");
        services.AddScoped<IBlockifyDbService, BlockifyDbService>(options =>
        {
            var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(
                configuration.GetConnectionString("BlockifyDb")
                ?? throw new MissingConfigurationException(
                    "\"BlockifyDb\" connection string"
                )
            );
            return new BlockifyDbService(npgsqlDataSourceBuilder);
        });
        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        services.AddHttpClient<ISpotifyClient, SpotifyClient>();
        services.AddScoped<ISpotifyService, SpotifyService>();
    }
}