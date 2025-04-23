# TeamTasker Backend

A .NET 8.0 application built with clean architecture principles.

## Architecture Overview

This solution follows clean architecture principles with the following layers:

- **Domain Layer**: Contains enterprise logic and entities
- **Application Layer**: Contains business logic and use cases
- **Infrastructure Layer**: Contains implementation details and external concerns
- **API Layer**: Contains controllers and API endpoints

## Key Features

- Clean Architecture with clear separation of concerns
- Domain-Driven Design principles
- CQRS pattern with MediatR
- Entity Framework Core 8.0
- Repository pattern
- Fluent Validation
- Unit testing with xUnit
- Swagger documentation

## Project Structure

- **TeamTasker.Domain**: Contains domain entities, value objects, and domain events
- **TeamTasker.Application**: Contains application services, commands, queries, and validators
- **TeamTasker.Infrastructure**: Contains data access, external services, and infrastructure concerns
- **TeamTasker.API**: Contains API controllers and configuration
- **TeamTasker.SharedKernel**: Contains shared abstractions and base classes

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQLite (configured by default) or SQL Server

### Setup

1. Clone the repository
2. The default configuration uses SQLite with a local database file. If you want to use SQL Server, update the connection string in `appsettings.Development.json`
3. Install the EF Core tools if you haven't already: `dotnet tool install --global dotnet-ef`
4. Run migrations: `cd src/Presentation/TeamTasker.API && dotnet ef database update`
5. Run the application: `cd src/Presentation/TeamTasker.API && dotnet run`
6. Access the Swagger UI at: `http://localhost:5220/swagger`

### API Endpoints

The following API endpoints are available:

- **GET /api/projects** - Get all projects
- **GET /api/projects/{id}** - Get a specific project by ID
- **POST /api/projects** - Create a new project
- **PUT /api/projects/{id}** - Update an existing project
- **DELETE /api/projects/{id}** - Delete a project

## Testing

The application includes comprehensive unit tests for all layers:

- Domain layer tests
- Application layer tests
- Infrastructure layer tests
- API layer tests

### Running Tests

To run all tests:

```bash
dotnet test
```

### Code Coverage

To generate code coverage reports:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

To view the coverage report in HTML format, install the ReportGenerator tool:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"tests/*/TestResults/*/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Microservices Readiness

The application is designed to be easily transitioned into a microservices architecture:

- Each bounded context is isolated with clear boundaries
- Domain events for cross-context communication
- Independent data persistence capabilities
- Modular design for easy extraction of services

## Current Status and Future Plans

### Implemented Features

- ✅ Clean Architecture with Domain, Application, Infrastructure, and API layers
- ✅ CQRS pattern with MediatR
- ✅ Entity Framework Core 8.0 with SQLite
- ✅ Project management functionality (CRUD operations)
- ✅ Unit tests for all layers
- ✅ Swagger documentation

### Planned Features

- ⏳ Authentication and Authorization with JWT
- ⏳ Task management functionality
- ⏳ User management functionality
- ⏳ Improved validation with FluentValidation
- ⏳ Global exception handling
- ⏳ Pagination, filtering, and sorting
- ⏳ Improved test coverage (target: 80%)
- ⏳ CI/CD pipeline
