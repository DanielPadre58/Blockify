using Blockify.Api.Configuration;
using Blockify.Infrastructure.Blockify.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile(
        "appsettings.Development.Local.json",
        optional: true,
        reloadOnChange: true
    );
}

var configurator = new ApplicationConfigurator(builder.Services, builder.Configuration);

configurator.ConfigureServices();
configurator.AddDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Blockify")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

configurator.Configure(app);

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var migrationManager = scope.ServiceProvider.GetRequiredService<IBlockifyMigrationsManager>();
    await migrationManager.ApplyMigrationsAsync();
}

app.Run();
