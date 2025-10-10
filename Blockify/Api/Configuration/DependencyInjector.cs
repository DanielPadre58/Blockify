using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Blockify;
using Blockify.Application.Services.Spotify;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.External.Gemini.Client;
using Blockify.Infrastructure.External.Genius.Client;
using Blockify.Infrastructure.External.Spotify.Client;
using Blockify.Shared.Exceptions;
using Npgsql;

namespace Blockify.Api.Configuration;

public static class DependencyInjector
{
    public static void Inject(IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        services.AddScoped<ISpotifyService, SpotifyService>();
        services.AddScoped<IBlockifyService, BlockifyService>();
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
        services.AddHttpClient<ISpotifyClient, SpotifyClient>();
        services.AddHttpClient<IGeminiClient, GeminiClient>();
        services.AddHttpClient<IGeniusClient, GeniusClient>();
    }
}