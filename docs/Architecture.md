# TeamTasker Architecture

## Overview

TeamTasker is built following Clean Architecture principles, which promotes separation of concerns and dependency inversion. The application is divided into several layers, each with its own responsibility and dependencies.

## Layers

### Domain Layer

The Domain layer is the core of the application and contains:

- Domain Entities
- Value Objects
- Domain Events
- Repository Interfaces
- Domain Services

This layer has no dependencies on other layers or external frameworks. It encapsulates the business rules and logic of the application.

### Application Layer

The Application layer coordinates the application's tasks and delegates work to the domain. It contains:

- Commands and Queries (CQRS pattern)
- Command and Query Handlers
- Validators
- DTOs (Data Transfer Objects)
- Interfaces for Infrastructure services

This layer depends only on the Domain layer and defines interfaces that are implemented by the outer layers.

### Infrastructure Layer

The Infrastructure layer provides implementations for the interfaces defined in the Application and Domain layers. It contains:

- Database Context
- Repository Implementations
- External Service Implementations
- Data Access Logic
- Migrations

This layer depends on the Application and Domain layers.

### API Layer

The API layer is the entry point to the application and contains:

- Controllers
- Middleware
- Filters
- Configuration

This layer depends on the Application layer and sometimes on the Infrastructure layer for dependency injection.

## CQRS Pattern

The application uses the Command Query Responsibility Segregation (CQRS) pattern, which separates read and write operations:

- Commands: Used for operations that change state (create, update, delete)
- Queries: Used for operations that read state

This separation allows for optimizing each type of operation independently.

## Dependency Injection

The application uses the built-in dependency injection container in ASP.NET Core. Dependencies are registered in the `DependencyInjection.cs` files in each layer.

## Entity Framework Core

Entity Framework Core is used as the ORM (Object-Relational Mapper) for data access. The application follows a code-first approach with migrations.

## Microservices Readiness

The application is designed to be easily transitioned into a microservices architecture:

1. **Bounded Contexts**: The domain is divided into bounded contexts that can be extracted as separate microservices.
2. **Domain Events**: Domain events are used for communication between bounded contexts, which can be replaced with message brokers in a microservices architecture.
3. **Independent Data Access**: Each potential microservice has its own data access logic that can be separated into its own database.
4. **API Gateway**: The API layer can be transformed into an API gateway that routes requests to the appropriate microservice.

## Folder Structure

```
TeamTasker/
├── src/
│   ├── Core/
│   │   ├── TeamTasker.Domain/
│   │   ├── TeamTasker.Application/
│   │   └── TeamTasker.SharedKernel/
│   ├── Infrastructure/
│   │   └── TeamTasker.Infrastructure/
│   └── Presentation/
│       └── TeamTasker.API/
├── tests/
│   ├── TeamTasker.Domain.UnitTests/
│   ├── TeamTasker.Application.UnitTests/
│   ├── TeamTasker.Infrastructure.UnitTests/
│   └── TeamTasker.API.UnitTests/
├── docs/
└── docker-compose.yml
```
