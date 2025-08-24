using Blockify.Application.Services.Spotify;
using Blockify.Shared.Exceptions;

namespace Blockify.Api.Configuration
{
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
                .AddCookie(
                    "default_cookie",
                    options =>
                    {
                        options.Cookie.Name = "blockifyCookie";
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);

                        options.LoginPath = "/account/login";
                        options.LogoutPath = "/account/logout";
                        options.AccessDeniedPath = "/account/denied";
                    }
                )
                .AddSpotify(
                    "spotify",
                    options =>
                    {
                        options.ClientId =
                            _configuration["Api:OAuth:Spotify:ClientId"]
                            ?? throw new MissingConfigurationException("Spotify Client Id");

                        options.ClientSecret =
                            _configuration["Api:OAuth:Spotify:ClientSecret"]
                            ?? throw new MissingConfigurationException("Spotify Client Secret");

                        options.SaveTokens = true;
                    }
                );

            _services.Configure<SpotifyConfiguration>(
                _configuration.GetSection("Api:OAuth:Spotify")
            );
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
}
