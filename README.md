# Blockify

Blockify is a modern ASP.NET Core Web API application built on .NET 9.0, designed to provide secure authentication and integration with Spotify. The project follows best practices for scalability, maintainability, and containerization.

## Features

- **Spotify OAuth Authentication**: Secure user authentication via Spotify.
- **RESTful API**: Clean, versioned endpoints for client integration.
- **OpenAPI/Scalar**: Interactive API documentation for easy exploration.
- **Docker Support**: Ready-to-use Docker and Docker Compose configuration for local development and deployment.
- **Extensible Architecture**: Modular structure for easy feature expansion.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for containerized development)
- Visual Studio 2022, VS Code, or any compatible IDE

## Getting Started

### 1. Clone the Repository

```sh
git clone <your-repository-url>
cd Blockify
```

### 2. Restore Dependencies

```sh
dotnet restore
```

### 3. Run the Application (Development)

```sh
dotnet run
```

Access the API at:

- http://localhost:5000
- https://localhost:7155

### 4. Run with Docker

```sh
docker-compose up --build
```

Access the API at:

- http://localhost:5000
- https://localhost:5001

## API Documentation

OpenAPI/Swagger documentation is available at:

- http://localhost:5000/swagger

## Project Structure

- **Api/**: Controllers and configuration for API endpoints.
- **Application/**: DTOs and service logic.
- **Domain/**: Database interfaces, entities, and SQL logic.
- **Shared/**: Common exceptions and utilities.
- **Properties/**: Launch settings and configuration.
- **wwwroot/**: Static files.

## Configuration

Application settings are managed via `appsettings.json` and environment-specific files. Sensitive credentials (e.g., Spotify OAuth keys) should be provided via environment variables or secure configuration providers.

## Docker & Deployment

- See [README-Docker.md](../../README-Docker.md) for detailed Docker usage and deployment instructions.
- Supports hot reload and volume mounting for development.

## Contributing

1. Fork the repository.
2. Create a feature branch.
3. Make your changes and add tests.
4. Submit a pull request.

## License

This project is licensed under the MIT License.