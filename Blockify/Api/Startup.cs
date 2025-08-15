using Blockify.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
SwaggerConfiguration.ConfigureSwagger(builder.Services);

DependencyInjector.Inject(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    SwaggerConfiguration.ConfigureSwaggerUI(app);
}

app.UseAuthorization();

app.MapControllers();

app.Run();
