using Microsoft.OpenApi.Models;

namespace Blockify.Api;

public static class SwaggerConfiguration{
    public static void ConfigureSwagger(IServiceCollection services){
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Blockify API", Version = "v1" });
        });
    }

    public static void ConfigureSwaggerUI(WebApplication app){
        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blockify API v1");
        });
    }
}