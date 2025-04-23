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
- SQL Server (or other database provider)

### Setup

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update --project src/Infrastructure/TeamTasker.Infrastructure --startup-project src/Presentation/TeamTasker.API`
4. Run the application: `dotnet run --project src/Presentation/TeamTasker.API`

## Microservices Readiness

The application is designed to be easily transitioned into a microservices architecture:

- Each bounded context is isolated with clear boundaries
- Domain events for cross-context communication
- Independent data persistence capabilities
- Modular design for easy extraction of services
