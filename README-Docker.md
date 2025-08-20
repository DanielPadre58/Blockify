# Blockify Docker Setup

This document provides instructions for running the Blockify application using Docker and Docker Compose.

## Prerequisites

- Docker Desktop installed and running
- Docker Compose (usually included with Docker Desktop)

## Quick Start

### 1. Build and Run the Application

```bash
# Build and start all services
docker-compose up --build

# Run in detached mode (background)
docker-compose up -d --build
```

### 2. Access the Application

- **Main Application**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **OpenAPI/Swagger**: http://localhost:5000/swagger (in development mode)

## Development Mode

The application includes a development override configuration that enables:

- Hot reload with `dotnet watch`
- Volume mounting for live code changes
- Development environment settings

```bash
# Start in development mode (uses docker-compose.override.yml)
docker-compose up --build
```

## Production Mode

For production deployment, use the base docker-compose.yml without the override:

```bash
# Remove override and run production build
docker-compose -f docker-compose.yml up --build
```

## Useful Commands

```bash
# View logs
docker-compose logs -f blockify-app

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Rebuild a specific service
docker-compose build blockify-app

# Execute commands in running container
docker-compose exec blockify-app dotnet --version

# View running containers
docker-compose ps
```

## Service Configuration

### Main Application (blockify-app)
- **Ports**: 5000 (HTTP), 5001 (HTTPS)
- **Environment**: Development
- **Hot Reload**: Enabled in development mode

### PostgreSQL Database (blockify-db)
- **Port**: 5432
- **Database**: blockify
- **Credentials**: blockify_user / blockify_password
- **Data Persistence**: postgres_data volume

### Redis Cache (blockify-redis)
- **Port**: 6379
- **Data Persistence**: redis_data volume

## Customization

### Environment Variables

You can customize the application by creating a `.env` file:

```env
POSTGRES_DB=your_database_name
POSTGRES_USER=your_username
POSTGRES_PASSWORD=your_password
ASPNETCORE_ENVIRONMENT=Development
```

### Port Configuration

To change the exposed ports, modify the `ports` section in `docker-compose.yml`:

```yaml
ports:
  - "8080:80"    # Change 5000 to 8080
  - "8443:443"   # Change 5001 to 8443
```

## Troubleshooting

### Common Issues

1. **Port already in use**: Change the port mappings in docker-compose.yml
2. **Permission denied**: Ensure Docker has proper permissions
3. **Build fails**: Check that all required files are present

### Logs and Debugging

```bash
# View all logs
docker-compose logs

# View specific service logs
docker-compose logs blockify-app

# Follow logs in real-time
docker-compose logs -f
```

### Cleanup

```bash
# Remove all containers, networks, and volumes
docker-compose down -v --remove-orphans

# Remove all images
docker system prune -a
```

## Security Notes

- Default passwords are used for development only
- Change default credentials for production use
- Consider using Docker secrets for sensitive data in production
- Enable HTTPS in production environments
