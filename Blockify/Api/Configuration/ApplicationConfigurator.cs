using System.Security.Claims;
using AspNet.Security.OAuth.Spotify;
using Blockify.Infrastructure.External.Configuration;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Blockify.Api.Configuration;

public class ApplicationConfigurator
{
    public readonly IServiceCollection Services;
    public readonly IConfiguration Configuration;

    public ApplicationConfigurator(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    public void ConfigureServices()
    {
        Services.AddControllers();
        Services.AddEndpointsApiExplorer();
        Services.AddOpenApi();
        Services
            .AddAuthentication("default_cookie")
            .AddCookie("default_cookie", CookieOptions())
            .AddSpotify("spotify", SpotifyOptions());

        Services.Configure<SpotifyConfiguration>(
            Configuration.GetSection("Api:OAuth:Spotify")
        );
        Services.Configure<GeminiConfiguration>(
            Configuration.GetSection("Api:AI:Gemini")
        );
        Services.Configure<GeniusConfiguration>(
            Configuration.GetSection("Api:Lyrics:Genius")
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
                Configuration["Api:OAuth:Spotify:ClientId"]
                ?? throw new MissingConfigurationException("Spotify Client Id");

            options.ClientSecret =
                Configuration["Api:OAuth:Spotify:ClientSecret"]
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
        DependencyInjector.Inject(Services, Configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}