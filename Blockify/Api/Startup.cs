using Blockify.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

DependencyInjector.Inject(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
