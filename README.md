# Blockify

A .NET 9.0 web application built with ASP.NET Core.

## Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022, VS Code, or any IDE that supports .NET

## Getting Started

1. Clone the repository:
   ```bash
   git clone <your-repository-url>
   cd Blockify
   ```

2. Navigate to the project directory:
   ```bash
   cd Blockify
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to:
   - https://localhost:7001 (HTTPS)
   - http://localhost:5000 (HTTP)

## Development

The application uses:
- .NET 9.0
- ASP.NET Core Web API
- OpenAPI/Swagger for API documentation

## Project Structure

```
Blockify/
├── Blockify.csproj          # Project file with dependencies
├── Startup.cs              # Application startup configuration
├── appsettings.json        # Application configuration
├── appsettings.Development.json  # Development-specific settings
└── Properties/
    └── launchSettings.json # Launch configuration
```

## API Documentation

When running in development mode, you can access the OpenAPI documentation at:
- https://localhost:7001/swagger

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

[Add your license information here]
