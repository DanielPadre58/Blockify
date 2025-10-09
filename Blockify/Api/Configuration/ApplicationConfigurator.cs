using System.Security.Claims;
using AspNet.Security.OAuth.Spotify;
using Blockify.Api.Configuration.External_Services_Configuration;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Blockify.Api.Configuration;

public class ApplicationConfigurator
{
    public readonly IServiceCollection _services;
    public readonly IConfiguration _configuration;

    public ApplicationConfigurator(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public void ConfigureServices()
    {
        _services.AddControllers();
        _services.AddEndpointsApiExplorer();
        _services.AddOpenApi();
        _services
            .AddAuthentication("default_cookie")
            .AddCookie("default_cookie", CookieOptions())
            .AddSpotify("spotify", SpotifyOptions());

        _services.Configure<SpotifyConfiguration>(
            _configuration.GetSection("Api:OAuth:Spotify")
        );
        _services.Configure<GeminiConfiguration>(
            _configuration.GetSection("Api:AI:Gemini")
        );
        _services.Configure<GeniusConfiguration>(
            _configuration.GetSection("Api:Lyrics:Genius")
        );
    }

    private static Action<CookieAuthenticationOptions> CookieOptions()
    {
        return options =>
        {
            options.Cookie.Name = "blockifyCookie";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);

            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/logout";
            options.AccessDeniedPath = "/account/denied";
        };
    }

    private Action<SpotifyAuthenticationOptions> SpotifyOptions()
    {
        return options =>
        {
            options.ClientId =
                _configuration["Api:OAuth:Spotify:ClientId"]
                ?? throw new MissingConfigurationException("Spotify Client Id");

            options.ClientSecret =
                _configuration["Api:OAuth:Spotify:ClientSecret"]
                ?? throw new MissingConfigurationException("Spotify Client Secret");

            options.SaveTokens = true;

            options.Scope.Add("user-read-private");
            options.Scope.Add("playlist-modify-public");
            options.Scope.Add("user-read-email");
            options.Scope.Add("playlist-read-private");

            options.Events.OnCreatingTicket = async context =>
            {
                context.Identity?.AddClaim(new Claim("urn:provider", "spotify"));

                context.Identity?.AddClaim(
                    new Claim("iat", DateTimeOffset.Now.ToUnixTimeSeconds().ToString())
                );

                context.Identity?.AddClaim(
                    new Claim(
                        "exp",
                        DateTimeOffset.Now.AddDays(30).ToUnixTimeSeconds().ToString()
                    )
                );

                await Task.CompletedTask;
            };
        };
    }

    public void AddDependencies()
    {
        DependencyInjector.Inject(_services, _configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}