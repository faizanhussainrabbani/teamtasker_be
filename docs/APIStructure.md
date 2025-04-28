# TeamTasker API Structure

This document provides an overview of the RESTful API structure in the TeamTasker application, explaining how the APIs are organized, implemented, and how they follow RESTful principles.

## API Architecture Overview

TeamTasker implements a RESTful API architecture with the following characteristics:

1. **Resource-Based Endpoints**: APIs are organized around resources (Tasks, Projects, Users, Teams)
2. **HTTP Methods**: Uses standard HTTP methods (GET, POST, PUT, PATCH, DELETE) for CRUD operations
3. **JSON Responses**: All responses are in JSON format
4. **Status Codes**: Uses appropriate HTTP status codes (200, 201, 204, 400, 401, 404, 500)
5. **Authentication**: JWT-based authentication with bearer tokens
6. **Pagination**: Support for paginated responses
7. **Filtering**: Support for filtering resources based on various criteria
8. **Error Handling**: Consistent error response format

## Controller Structure

All API controllers in TeamTasker inherit from `ApiControllerBase`, which provides common functionality:

```csharp
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
```

This base controller:
- Applies the `[ApiController]` attribute for automatic model validation
- Sets up the route prefix `api/[controller]`
- Provides access to the MediatR mediator for implementing CQRS

## API Endpoints

The application exposes the following main API endpoints:

### Authentication API (`/api/auth`)

Handles user authentication and authorization.

| Endpoint | Method | Description | Request Body | Response |
|----------|--------|-------------|--------------|----------|
| `/api/auth/login` | POST | Authenticates a user | `{ username, password }` | `{ token, refreshToken, user }` |
| `/api/auth/register` | POST | Registers a new user | `{ username, email, password, firstName, lastName, ... }` | `{ id, username, email, ... }` |
| `/api/auth/refresh-token` | POST | Refreshes an authentication token | `{ refreshToken }` | `{ token, refreshToken }` |

### Tasks API (`/api/tasks`)

Manages task resources.

| Endpoint | Method | Description | Request Body | Response |
|----------|--------|-------------|--------------|----------|
| `/api/tasks` | GET | Gets a list of tasks with optional filtering | Query parameters | Paginated list of tasks |
| `/api/tasks/{id}` | GET | Gets a single task by ID | - | Task details |
| `/api/tasks` | POST | Creates a new task | Task creation data | Created task |
| `/api/tasks/{id}` | PATCH | Updates an existing task | Task update data | Updated task |
| `/api/tasks/{id}` | DELETE | Deletes a task | - | No content |

### Projects API (`/api/projects`)

Manages project resources.

| Endpoint | Method | Description | Request Body | Response |
|----------|--------|-------------|--------------|----------|
| `/api/projects` | GET | Gets a list of projects with optional filtering | Query parameters | Paginated list of projects |
| `/api/projects/{id}` | GET | Gets a single project by ID | - | Project details |
| `/api/projects` | POST | Creates a new project | Project creation data | Created project |
| `/api/projects/{id}` | PATCH | Updates an existing project | Project update data | Updated project |
| `/api/projects/{id}` | DELETE | Deletes a project | - | No content |
| `/api/projects/{id}/tasks` | GET | Gets tasks for a specific project | Query parameters | Paginated list of tasks |

### Teams API (`/api/teams`)

Manages team resources.

| Endpoint | Method | Description | Request Body | Response |
|----------|--------|-------------|--------------|----------|
| `/api/teams` | GET | Gets a list of teams | Query parameters | Paginated list of teams |
| `/api/teams/{id}` | GET | Gets a single team by ID | - | Team details |
| `/api/teams` | POST | Creates a new team | Team creation data | Created team |
| `/api/teams/{id}` | PATCH | Updates an existing team | Team update data | Updated team |
| `/api/teams/{id}` | DELETE | Deletes a team | - | No content |
| `/api/teams/{id}/members` | GET | Gets members of a team | Query parameters | List of team members |
| `/api/teams/{id}/members` | POST | Adds a member to a team | Member data | Added team member |
| `/api/teams/{id}/members/{memberId}` | DELETE | Removes a member from a team | - | No content |

### Users API (`/api/users`)

Manages user resources.

| Endpoint | Method | Description | Request Body | Response |
|----------|--------|-------------|--------------|----------|
| `/api/users` | GET | Gets a list of users | Query parameters | Paginated list of users |
| `/api/users/{id}` | GET | Gets a single user by ID | - | User details |
| `/api/users/{id}` | PATCH | Updates an existing user | User update data | Updated user |
| `/api/users/me` | GET | Gets the current user's profile | - | User details |
| `/api/users/me` | PATCH | Updates the current user's profile | Profile update data | Updated user |

## Implementation Details

### Controller Implementation

Controllers in TeamTasker follow a consistent pattern:

1. **Dependency Injection**: Controllers receive dependencies through constructor injection
2. **Action Methods**: Each endpoint is implemented as an action method
3. **Response Types**: Action methods use `ActionResult<T>` to specify return types
4. **Documentation**: Methods are documented with XML comments
5. **Response Attributes**: Methods use `[ProducesResponseType]` to document possible responses
6. **Authorization**: Methods use `[Authorize]` to require authentication
7. **Error Handling**: Methods use try-catch blocks to handle exceptions

Example from TasksController:

```csharp
/// <summary>
/// Get a single task by ID
/// </summary>
/// <param name="id">Task ID</param>
/// <returns>Task details</returns>
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<TaskDetailDto>> GetTask(int id)
{
    try
    {
        _logger.LogInformation("Getting task {TaskId}", id);

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        // Map to DTO
        var taskDto = new TaskDetailDto
        {
            // Properties mapping...
        };

        return Ok(taskDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting task {TaskId}", id);
        return StatusCode(500, new { message = "An error occurred while processing your request" });
    }
}
```

### CQRS Implementation

While the controllers currently use repositories directly, the application is designed to transition to a full CQRS implementation using MediatR:

1. **Commands**: For write operations (create, update, delete)
2. **Queries**: For read operations
3. **Mediator**: For dispatching commands and queries to their handlers

The `ApiControllerBase` already provides access to the MediatR mediator, which will be used in future refactoring.

### Request Validation

Request validation is handled through:

1. **Model Validation**: Automatic validation through `[ApiController]` attribute
2. **Custom Validation**: Manual validation in controller actions
3. **Fluent Validation**: Validation rules defined using FluentValidation (in progress)

### Response Formatting

Responses follow a consistent format:

1. **Success Responses**: Return the requested data with appropriate status codes
2. **Error Responses**: Return an error object with a message and details
3. **Pagination**: Paginated responses include metadata (total count, page number, page size)

Example of a paginated response:

```json
{
  "items": [
    {
      "id": 1,
      "title": "Task 1",
      "description": "Description for Task 1",
      "status": "in-progress",
      "priority": "High",
      "dueDate": "2023-12-31T00:00:00Z",
      "progress": 50,
      "projectId": 1,
      "projectName": "Project 1",
      "assigneeId": 2,
      "assigneeTeamMemberId": 3,
      "assignee": {
        "id": 2,
        "name": "John Doe",
        "avatar": "https://example.com/avatar.jpg",
        "initials": "JD",
        "teamMemberId": 3,
        "teamId": 1,
        "teamRole": "Member"
      },
      "creatorId": 1,
      "creatorTeamMemberId": 2,
      "creator": {
        "id": 1,
        "name": "Jane Smith",
        "avatar": "https://example.com/avatar2.jpg",
        "initials": "JS",
        "teamMemberId": 2,
        "teamId": 1,
        "teamRole": "Owner"
      },
      "tags": ["frontend", "bug"],
      "createdDate": "2023-01-01T00:00:00Z",
      "updatedDate": "2023-01-15T00:00:00Z",
      "completedDate": null
    },
    // More items...
  ],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Authentication and Authorization

The API uses JWT-based authentication:

1. **Token Generation**: `/api/auth/login` generates a JWT token and refresh token
2. **Token Validation**: The `[Authorize]` attribute validates the JWT token
3. **Token Refresh**: `/api/auth/refresh-token` refreshes an expired token
4. **User Context**: The `ICurrentUserService` provides access to the current user's ID

## API Versioning

API versioning is planned but not yet implemented. The planned approach is:

1. **URL-Based Versioning**: `/api/v1/tasks`, `/api/v2/tasks`
2. **Version-Specific Controllers**: `TasksV1Controller`, `TasksV2Controller`
3. **API Version Attributes**: `[ApiVersion("1.0")]`, `[ApiVersion("2.0")]`

## Error Handling

The API implements consistent error handling:

1. **Try-Catch Blocks**: Each controller action has a try-catch block
2. **Logging**: Exceptions are logged with context information
3. **Error Responses**: A consistent error response format is used
4. **Status Codes**: Appropriate HTTP status codes are returned

Example error response:

```json
{
  "message": "An error occurred while processing your request",
  "details": "Task not found with ID 123",
  "statusCode": 404
}
```

## Future Enhancements

Planned enhancements to the API structure:

1. **Complete CQRS Implementation**: Fully implement CQRS using MediatR
2. **API Versioning**: Implement API versioning
3. **Rate Limiting**: Add rate limiting to prevent abuse
4. **API Documentation**: Enhance Swagger documentation
5. **Caching**: Implement response caching for frequently accessed resources
6. **Hypermedia Links**: Add HATEOAS links to responses
7. **GraphQL Endpoint**: Add a GraphQL endpoint for more flexible querying
