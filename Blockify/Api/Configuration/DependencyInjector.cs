using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.Gemini;
using Blockify.Infrastructure.Spotify.Client;
using Blockify.Shared.Exceptions;
using Npgsql;

namespace Blockify.Api.Configuration;

public static class DependencyInjector
{
    public static void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBlockifyRepository, BlockifyRepository>(options =>
        {
            var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(
                configuration.GetConnectionString("BlockifyDb")
                ?? throw new MissingConfigurationException(
                    "\"BlockifyDb\" connection string"
                )
            );
            return new BlockifyRepository(npgsqlDataSourceBuilder);
        });
        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        services.AddHttpClient<ISpotifyClient, SpotifyClient>();
        services.AddScoped<ISpotifyService, SpotifyService>();
        services.AddScoped<IBlockifyMigrationsManager, BlockifyMigrationsManager>(options =>
        {
            var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(
                configuration.GetConnectionString("BlockifyDb")
                ?? throw new MissingConfigurationException(
                    "\"BlockifyDb\" connection string"
                )
            );
            return new BlockifyMigrationsManager(npgsqlDataSourceBuilder);
        });
        services.AddHttpClient<IGeminiClient, GeminiClient>();
    }
}