# TeamTasker Architecture Diagram

## Clean Architecture Overview

TeamTasker follows the Clean Architecture principles, which organizes the application into concentric layers with dependencies pointing inward. This ensures separation of concerns and makes the system more maintainable, testable, and adaptable to change.

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                                                         │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │                                                 │   │   │
│  │  │  ┌─────────────────────────────────────────┐   │   │   │
│  │  │  │                                         │   │   │   │
│  │  │  │           Domain Layer                  │   │   │   │
│  │  │  │                                         │   │   │   │
│  │  │  └─────────────────────────────────────────┘   │   │   │
│  │  │                Application Layer               │   │   │
│  │  │                                                 │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  │                  Infrastructure Layer                   │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                        Presentation Layer                       │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Component Diagram

The following diagram illustrates the main components of the TeamTasker application and their interactions:

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                         │
│                                  Presentation Layer                                     │
│                                                                                         │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────┐ │
│  │     Auth      │  │    Tasks      │  │   Projects    │  │     Teams     │  │ Users │ │
│  │  Controller   │  │  Controller   │  │  Controller   │  │  Controller   │  │  API  │ │
│  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘  └───┬───┘ │
│          │                  │                  │                  │              │     │
└──────────┼──────────────────┼──────────────────┼──────────────────┼──────────────┼─────┘
           │                  │                  │                  │              │
┌──────────┼──────────────────┼──────────────────┼──────────────────┼──────────────┼─────┐
│          │                  │                  │                  │              │     │
│          ▼                  ▼                  ▼                  ▼              ▼     │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────┐ │
│  │     Auth      │  │     Task      │  │    Project    │  │     Team      │  │ User  │ │
│  │   Services    │  │   Services    │  │   Services    │  │   Services    │  │Service│ │
│  └───────────────┘  └───────────────┘  └───────────────┘  └───────────────┘  └───────┘ │
│                                                                                         │
│                                  Application Layer                                      │
│                                                                                         │
└─────────────────────────────────────────────────────────────────────────────────────────┘
           │                  │                  │                  │              │
┌──────────┼──────────────────┼──────────────────┼──────────────────┼──────────────┼─────┐
│          │                  │                  │                  │              │     │
│          ▼                  ▼                  ▼                  ▼              ▼     │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────┐ │
│  │     Auth      │  │     Task      │  │    Project    │  │     Team      │  │ User  │ │
│  │ Repositories  │  │ Repositories  │  │ Repositories  │  │ Repositories  │  │  Repo │ │
│  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘  └───┬───┘ │
│          │                  │                  │                  │              │     │
│          └──────────────────┴──────────────────┴──────────────────┴──────────────┘     │
│                                         │                                              │
│                                         ▼                                              │
│                              ┌─────────────────────┐                                   │
│                              │  Entity Framework   │                                   │
│                              │       Core         │                                   │
│                              └─────────┬───────────┘                                   │
│                                        │                                               │
│                                        ▼                                               │
│                              ┌─────────────────────┐                                   │
│                              │     SQLite DB       │                                   │
│                              └─────────────────────┘                                   │
│                                                                                         │
│                                 Infrastructure Layer                                    │
│                                                                                         │
└─────────────────────────────────────────────────────────────────────────────────────────┘
           │                  │                  │                  │              │
┌──────────┼──────────────────┼──────────────────┼──────────────────┼──────────────┼─────┐
│          │                  │                  │                  │              │     │
│          ▼                  ▼                  ▼                  ▼              ▼     │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────┐ │
│  │     User      │  │     Task      │  │    Project    │  │     Team      │  │TeamMem│ │
│  │    Entity     │  │    Entity     │  │    Entity     │  │    Entity     │  │Entity │ │
│  └───────────────┘  └───────────────┘  └───────────────┘  └───────────────┘  └───────┘ │
│                                                                                         │
│  ┌───────────────┐  ┌───────────────┐                                                   │
│  │     Tag       │  │   TaskTag     │                                                   │
│  │    Entity     │  │    Entity     │                                                   │
│  └───────────────┘  └───────────────┘                                                   │
│                                                                                         │
│                                    Domain Layer                                         │
│                                                                                         │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Domain Layer

The Domain Layer is the core of the application and contains:

- **Entities**: User, Task, Project, Team, TeamMember, Skill, UserSkill, Tag, TaskTag
- **Value Objects**: Address
- **Domain Events**: TaskCreatedEvent, TaskUpdatedEvent, etc.
- **Repository Interfaces**: IUserRepository, ITaskRepository, ITagRepository, etc.
- **Domain Services**: Business logic that doesn't fit into entities

This layer has no dependencies on other layers or external frameworks.

### Application Layer

The Application Layer coordinates the application's tasks and delegates work to the domain:

- **Commands and Queries**: CreateTaskCommand, GetTasksQuery, etc.
- **Command and Query Handlers**: CreateTaskCommandHandler, GetTasksQueryHandler, etc.
- **DTOs**: TaskDto, UserDto, etc.
- **Interfaces for Infrastructure Services**: ICurrentUserService, IApplicationDbContext, etc.

This layer depends only on the Domain Layer.

### Infrastructure Layer

The Infrastructure Layer provides implementations for the interfaces defined in the Application and Domain layers:

- **Database Context**: ApplicationDbContext
- **Repository Implementations**: TaskRepository, UserRepository, etc.
- **External Service Implementations**: CurrentUserService, etc.
- **Data Access Logic**: Entity configurations, migrations, etc.

This layer depends on the Application and Domain layers.

### Presentation Layer

The Presentation Layer is the entry point to the application:

- **Controllers**: AuthController, TasksController, etc.
- **Middleware**: Authentication, error handling, etc.
- **Filters**: Authorization, validation, etc.
- **Configuration**: Startup, Program, etc.

This layer depends on the Application Layer and sometimes on the Infrastructure Layer for dependency injection.

## CQRS Implementation

TeamTasker implements the Command Query Responsibility Segregation (CQRS) pattern:

```
┌───────────────────┐                 ┌───────────────────┐
│                   │                 │                   │
│     Commands      │────────────────▶│     Command       │
│  (Create, Update, │                 │     Handlers      │
│     Delete)       │                 │                   │
│                   │                 └─────────┬─────────┘
└───────────────────┘                           │
                                               │
                                               ▼
                                      ┌─────────────────────┐
                                      │                     │
                                      │     Domain Model    │
                                      │                     │
                                      └─────────┬───────────┘
                                                │
                                                │
┌───────────────────┐                 ┌─────────▼─────────┐
│                   │                 │                   │
│     Queries       │◀────────────────│      Query        │
│      (Read)       │                 │     Handlers      │
│                   │                 │                   │
└───────────────────┘                 └───────────────────┘
```

## Microservices Readiness

The application is designed to be easily transitioned into a microservices architecture:

1. **Bounded Contexts**: The domain is divided into logical bounded contexts (User Management, Task Management, Project Management, Team Management) that can be extracted as separate microservices.

2. **Domain Events**: Domain events are used for communication between bounded contexts, which can be replaced with message brokers in a microservices architecture.

3. **Independent Data Access**: Each potential microservice has its own data access logic that can be separated into its own database.

4. **API Gateway**: The API layer can be transformed into an API gateway that routes requests to the appropriate microservice.

## Dependency Injection

TeamTasker uses the built-in dependency injection container in ASP.NET Core:

```
┌───────────────────────────────────────────────────────────┐
│                                                           │
│                  Dependency Injection Container           │
│                                                           │
├───────────────────┬───────────────────┬───────────────────┤
│                   │                   │                   │
│  Domain Services  │     Application   │  Infrastructure   │
│   Registration    │     Services      │     Services      │
│                   │   Registration    │   Registration    │
│                   │                   │                   │
└───────────────────┴───────────────────┴───────────────────┘
                              │
                              │
                              ▼
┌───────────────────────────────────────────────────────────┐
│                                                           │
│                      Service Resolution                   │
│                                                           │
└───────────────────────────────────────────────────────────┘
```
