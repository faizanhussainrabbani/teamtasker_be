# TeamTasker — Business-Rule & Logic-Constraint Catalog

Authoritative catalog of business rules, validation constraints, RBAC policies, workflow state machines, and architectural invariants for TeamTasker. Synchronized with source code as of 2026-04-18. Each row is backed by file:line evidence; click through to verify.

**Enforcement labels**
- **Enforced** — implemented directly in runtime code
- **Documented** — stated in standards / E2E docs only
- **Partial** — documented and partially implemented, or inconsistent across layers (see Section 6 for the specific gap)

---

## 1) Domain & Entity Rules

| Functional area | Layer | Rule / constraint | Enforcement | Evidence |
|---|---|---|---|---|
| Task aggregate defaults | Domain | New task starts as ToDo with progress 0; CreatedDate/UpdatedDate set to `DateTime.UtcNow` | Enforced | [Task.cs:21-25](../../../src/TeamTasker.Domain/Entities/Task.cs#L21-L25) |
| Task progress → state coupling | Domain | Progress clamped to `[0,100]`; `progress == 100` forces `Done` and sets `CompletedDate`; `progress == 0` forces `ToDo`; `progress ∈ (0,100)` forces `InProgress` | Enforced | [Task.cs:58-92](../../../src/TeamTasker.Domain/Entities/Task.cs#L58-L92) |
| Task state → progress coupling | Domain | `Status = Done` forces progress 100 + sets `CompletedDate`; `Status = ToDo` forces progress 0; `Status = InProgress` nudges progress into `[10,90]` when out of band; `Blocked/OnHold/Cancelled` reduces 100 to 90 | Enforced | [Task.cs:101-135](../../../src/TeamTasker.Domain/Entities/Task.cs#L101-L135) |
| Task assignment | Domain | Assignee is optional; `AssignTo()` / `Unassign()` are explicit domain operations that emit events | Enforced | [Task.cs](../../../src/TeamTasker.Domain/Entities/Task.cs) |
| Task tags relation | Domain + Infrastructure | Task has many tags; tag records are separate `TaskTag` entities | Enforced | [TaskTag.cs](../../../src/TeamTasker.Domain/Entities/TaskTag.cs), [TaskTagConfiguration.cs](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/TaskTagConfiguration.cs) |
| Project defaults | Domain | New project starts as `NotStarted`; UTC timestamps; `TeamId` optional | Enforced | [Project.cs:25-27](../../../src/TeamTasker.Domain/Entities/Project.cs#L25-L27) |
| Project → task composition | Domain | `Project.AddTask()` creates a task scoped to the project and raises `TaskAddedToProjectEvent` | Enforced | [Project.cs:79-89](../../../src/TeamTasker.Domain/Entities/Project.cs#L79-L89) |
| User defaults | Domain | Defaults: `Status = Active`, `Role = "User"`, empty profile fields, `Initials = firstName[0] + lastName[0]` | Enforced | [User.cs:22-39](../../../src/TeamTasker.Domain/Entities/User.cs#L22-L39) |
| User lifecycle | Domain | `Activate`/`Deactivate`; password/role/profile/address updates are explicit methods | Enforced | [User.cs:75-138](../../../src/TeamTasker.Domain/Entities/User.cs#L75-L138) |
| Team model | Domain | Team has optional lead user and mutable metadata (name/description/department) | Enforced | [Team.cs](../../../src/TeamTasker.Domain/Entities/Team.cs) |
| Skill model | Domain | `Skill` has name/category/description; `UserSkill` tracks level, years, project count | Enforced | [Skill.cs](../../../src/TeamTasker.Domain/Entities/Skill.cs), [UserSkill.cs](../../../src/TeamTasker.Domain/Entities/UserSkill.cs) |
| Domain event contracts | Domain | Create/update/status/progress/assignment/tag actions emit corresponding events for task/project/user/team/skill aggregates | Enforced | [TaskEvents.cs](../../../src/TeamTasker.Domain/Events/TaskEvents.cs), [ProjectEvents.cs](../../../src/TeamTasker.Domain/Events/ProjectEvents.cs), [UserEvents.cs](../../../src/TeamTasker.Domain/Events/UserEvents.cs), [TeamEvents.cs](../../../src/TeamTasker.Domain/Events/TeamEvents.cs), [SkillEvents.cs](../../../src/TeamTasker.Domain/Events/SkillEvents.cs) |
| Entity relationship constraints | Infrastructure | `Project → Tasks` cascade delete; `Project → Team` optional with SetNull; `Task → Assignee` optional with SetNull; `Task → TaskTags` cascade; `TeamMember` and `UserSkill` relations cascade | Enforced | [ProjectConfiguration.cs:26-36](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/ProjectConfiguration.cs#L26-L36), [TaskConfiguration.cs:38-42](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/TaskConfiguration.cs#L38-L42), [TaskTagConfiguration.cs:18-21](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/TaskTagConfiguration.cs#L18-L21), [TeamMemberConfiguration.cs:18-27](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/TeamMemberConfiguration.cs#L18-L27), [UserSkillConfiguration.cs](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/UserSkillConfiguration.cs) |
| Uniqueness constraints | Infrastructure | User `Email` and `Username` are unique indexed fields | Enforced | [UserConfiguration.cs:29-37](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/UserConfiguration.cs#L29-L37) |

---

## 2) Validation Logic

| Functional area | Layer | Rule / constraint | Enforcement | Evidence |
|---|---|---|---|---|
| Task title | Application validator + DB | Required on create; max length 100 | Enforced | [CreateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommandValidator.cs), [TaskConfiguration.cs](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/TaskConfiguration.cs) |
| Task description | Application validator + DB | Optional; max length 500 | Enforced | [CreateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommandValidator.cs), [UpdateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTask/UpdateTaskCommandValidator.cs) |
| Task status vocabulary | Application validator | Allowed canonical values: `todo`, `in-progress`, `completed`, `blocked`, `on-hold`, `cancelled` | Enforced | [CreateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommandValidator.cs), [UpdateTaskStatusCommandValidator.cs:10-23](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTaskStatus/UpdateTaskStatusCommandValidator.cs#L10-L23) |
| Task priority vocabulary | Application validator | Allowed values: `low`, `medium`, `high`, `critical` | Enforced | [CreateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommandValidator.cs), [UpdateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTask/UpdateTaskCommandValidator.cs) |
| Task progress boundaries | Application validator + domain | Must be `[0,100]` in update commands; domain method also clamps | Enforced | [UpdateTaskProgressCommandValidator.cs:15-17](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTaskProgress/UpdateTaskProgressCommandValidator.cs#L15-L17), [Task.cs:58-61](../../../src/TeamTasker.Domain/Entities/Task.cs#L58-L61) |
| Task due date | Application validator + handler | `DueDate` must parse as date; create defaults to `UtcNow + 7 days` when omitted | Enforced | [CreateTaskCommand.cs:128-132](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommand.cs#L128-L132) |
| Assignee format | Application validator + handler | `AssigneeId` must be numeric string and reference an existing user | Enforced | [CreateTaskCommand.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommand.cs), [UpdateTaskCommand.cs](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTask/UpdateTaskCommand.cs) |
| Task tags | Application validator + handler + DB | Each tag max 50 chars. Create enforces max 20 tags; **update validator does not enforce the 20-tag cap** | Partial — gap **G1** | [CreateTaskCommandValidator.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommandValidator.cs), [UpdateTaskCommandValidator.cs:54-55](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTask/UpdateTaskCommandValidator.cs#L54-L55) |
| Project name/description | Application validator + DB | Name required, max 100; description max 500 | Enforced | [CreateProjectCommandValidator.cs:13-26](../../../src/TeamTasker.Application/Projects/Commands/CreateProject/CreateProjectCommandValidator.cs#L13-L26), [UpdateProjectCommandValidator.cs:12-28](../../../src/TeamTasker.Application/Projects/Commands/UpdateProject/UpdateProjectCommandValidator.cs#L12-L28) |
| Project dates | Application validator | `StartDate` required; `EndDate ≥ StartDate` when provided | Enforced | [CreateProjectCommandValidator.cs](../../../src/TeamTasker.Application/Projects/Commands/CreateProject/CreateProjectCommandValidator.cs), [UpdateProjectCommandValidator.cs](../../../src/TeamTasker.Application/Projects/Commands/UpdateProject/UpdateProjectCommandValidator.cs) |
| Auth request validation | Application DTO annotations | `[Required]` username/password on login; registration + password-change require min length 6; confirm-password fields compared | Enforced | [AuthRequest.cs:13-20](../../../src/TeamTasker.Application/Common/Models/AuthRequest.cs#L13-L20), [RegisterRequest.cs:14-40](../../../src/TeamTasker.Application/Common/Models/RegisterRequest.cs#L14-L40), [ChangePasswordRequest.cs:13-28](../../../src/TeamTasker.Application/Common/Models/ChangePasswordRequest.cs#L13-L28) |
| Profile update validation | Application DTO + DB | `FirstName`/`LastName`/`Email` required; email format required; persistence max lengths apply | Enforced | [UpdateProfileRequest.cs](../../../src/TeamTasker.Application/Common/Models/UpdateProfileRequest.cs), [UserConfiguration.cs](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/UserConfiguration.cs) |
| Pagination bounds | Application query models | Users: page size clamped `[1,50]` via `PaginationParams`. Tasks: limit clamped to max 100 in handler | Enforced | [PaginationParams.cs:27](../../../src/TeamTasker.Application/Common/Models/PaginationParams.cs#L27), [GetTasksQuery.cs:99](../../../src/TeamTasker.Application/Tasks/Queries/GetTasks/GetTasksQuery.cs#L99) |
| Canonical client vocab | Application mapping | `TaskDto` maps domain enums to canonical strings: `todo`, `in-progress`, `completed`, `blocked`, `on-hold`, `cancelled` and `low`/`medium`/`high`/`critical` | Enforced | [TaskDto.cs:140-164](../../../src/TeamTasker.Application/Common/DTOs/TaskDto.cs#L140-L164) |

---

## 3) Security & Authorization

| Functional area | Layer | Rule / constraint | Enforcement | Evidence |
|---|---|---|---|---|
| Security baseline | Standards | State-changing endpoints must require authentication unless explicitly public | Documented | [engineering_standards.md](../standards/engineering_standards.md) |
| API auth middleware | API + Infrastructure | JWT bearer auth enabled globally; role claim mapped from token role key | Enforced | [DependencyInjection.cs](../../../src/TeamTasker.Infrastructure/DependencyInjection.cs) |
| Task endpoint RBAC | API | Tasks controller requires auth. Create/Update/Status/Progress allow `Admin` and `User`. Delete is `Admin`-only | Enforced | [TasksController.cs:48,61,81,100,119](../../../src/TeamTasker.API/Controllers/TasksController.cs#L48) |
| Project endpoint RBAC | API | Projects controller requires auth. Create/Update/Delete are `Admin`-only. Read endpoints require any authenticated user | Enforced | [ProjectsController.cs:18,65,82,104](../../../src/TeamTasker.API/Controllers/ProjectsController.cs#L18) |
| User endpoint protection | API | Users controller is authenticated; profile update requires `Admin` or `User` role | Enforced | [UsersController.cs:25,256](../../../src/TeamTasker.API/Controllers/UsersController.cs#L25) |
| JWT claims contract | Infrastructure | Access tokens include `sub`, `email`, `name`, `username`, `role`, `jti` | Enforced | [JwtTokenGenerator.cs:34-42](../../../src/TeamTasker.Infrastructure/Authentication/JwtTokenGenerator.cs#L34-L42) |
| Refresh token lifecycle | Infrastructure + API | Refresh tokens stored in distributed cache, 7-day expiry, validated server-side, revoked on use, rotated on refresh | Enforced | [RefreshTokenService.cs:18,35,40-44](../../../src/TeamTasker.Infrastructure/Authentication/RefreshTokenService.cs#L18), [AuthController.cs:270-273](../../../src/TeamTasker.API/Controllers/AuthController.cs#L270-L273) |
| Replay protection | API + docs | Replayed rotated refresh tokens are rejected as unauthorized | Enforced + Documented | [AuthController.cs](../../../src/TeamTasker.API/Controllers/AuthController.cs), [e2e_test_cases.md](../../testing/e2e_test_cases.md) |
| Frontend protected routing | Frontend middleware | Protected routes require a valid auth cookie; localStorage alone cannot pass Next.js middleware (runs server-side) | Enforced | [middleware.ts:52-53](../../../../teamtasker_fe/middleware.ts#L52-L53) |
| Client token handling | Frontend auth | Tokens saved in localStorage and mirrored to cookie; auth data cleared on 401 except pre-auth endpoints (login/register/forgot/reset) | Enforced | [auth.ts:54-55,85-88](../../../../teamtasker_fe/lib/auth.ts#L54-L55), [client.ts:81-95](../../../../teamtasker_fe/lib/api/client.ts#L81-L95) |
| Proactive refresh | Frontend API | If access token is within 5 min of expiry, client attempts refresh before each protected request | Enforced | [auth.ts:101-112](../../../../teamtasker_fe/lib/auth.ts#L101-L112), [token-refresh.ts:97-109](../../../../teamtasker_fe/lib/api/token-refresh.ts#L97-L109) |
| Logout hygiene (client) | Frontend hooks | `useLogout` removes tokens and clears full react-query cache on success AND error paths | Enforced | [useAuth.ts:94-96](../../../../teamtasker_fe/lib/api/hooks/useAuth.ts#L94-L96) |
| Logout hygiene (server) | API | Server-side logout endpoint does **not** revoke the active refresh token; session invalidation relies on client removal plus rotation/expiry | Partial — gap **G2** | [AuthController.cs:219-230](../../../src/TeamTasker.API/Controllers/AuthController.cs#L219-L230) |
| UI defense in depth | Frontend UI | Delete action rendered only for Admin (`role === 'admin'`) on tasks list | Enforced | [tasks-list.tsx:104,263](../../../../teamtasker_fe/components/tasks/tasks-list.tsx#L104) |

---

## 4) Workflow & State Transitions

| Functional area | Layer | Workflow rule | Enforcement | Evidence |
|---|---|---|---|---|
| Task creation flow | Application + Domain | Project resolution priority: explicit `ProjectId` → first unscoped project → auto-create `General Backlog` project | Enforced | [CreateTaskCommand.cs:76-110](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommand.cs#L76-L110) |
| Task initialization | Application + Domain | Priority defaults to `medium` when omitted; `DueDate` defaults to `UtcNow + 7 days`; optional initial status applied after create | Enforced | [CreateTaskCommand.cs](../../../src/TeamTasker.Application/Tasks/Commands/CreateTask/CreateTaskCommand.cs) |
| Task update flow | Application | Patch semantics: unspecified fields keep existing values; optional status/progress/assignee/tags apply independently | Enforced | [UpdateTaskCommand.cs](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTask/UpdateTaskCommand.cs) |
| Task status lifecycle | Domain + Application | States: `todo`, `in-progress`, `completed`, `blocked`, `on-hold`, `cancelled`; transitions emit `TaskStatusUpdatedEvent` | Enforced | [Task.cs](../../../src/TeamTasker.Domain/Entities/Task.cs), [TaskEvents.cs](../../../src/TeamTasker.Domain/Events/TaskEvents.cs) |
| Task progress lifecycle | Domain + Application | Progress updates auto-transition status at boundaries (0 and 100) and update `CompletedDate` accordingly | Enforced | [Task.cs:58-92](../../../src/TeamTasker.Domain/Entities/Task.cs#L58-L92) |
| Task visibility workflow | Application query | Non-admin task reads are scope-limited by assignee or team membership; Admin sees all tasks | Enforced | [GetTasksQuery.cs:118-140](../../../src/TeamTasker.Application/Tasks/Queries/GetTasks/GetTasksQuery.cs#L118-L140), [GetTaskByIdQuery.cs](../../../src/TeamTasker.Application/Tasks/Queries/GetTaskById/GetTaskByIdQuery.cs) |
| Project lifecycle | Domain + Application + API | Create project (default `NotStarted`), update details, delete. Domain has `UpdateStatus` method but no dedicated API endpoint for project status transition | Partial — gap **G4** | [Project.cs](../../../src/TeamTasker.Domain/Entities/Project.cs), [ProjectsController.cs](../../../src/TeamTasker.API/Controllers/ProjectsController.cs) |
| Auth session workflow | API + Infrastructure + Frontend | Register/login issue access + refresh tokens; refresh rotates tokens; failed refresh clears session client-side and triggers unauthorized UX path | Enforced + Documented | [AuthController.cs](../../../src/TeamTasker.API/Controllers/AuthController.cs), [RefreshTokenService.cs](../../../src/TeamTasker.Infrastructure/Authentication/RefreshTokenService.cs), [token-refresh.ts](../../../../teamtasker_fe/lib/api/token-refresh.ts) |

---

## 5) Architectural Constraints

| Constraint | Layer | Requirement | Enforcement | Evidence |
|---|---|---|---|---|
| Clean separation of concerns | Cross-layer | Domain holds business rules; Application orchestrates use cases; Infrastructure handles external concerns; API is presentation only | Documented + Enforced | [engineering_standards.md](../standards/engineering_standards.md), [Architecture.md](../../Architecture.md) |
| CQRS boundary | Application | Commands mutate state, queries read state via MediatR handlers | Enforced | [TeamTasker.Application](../../../src/TeamTasker.Application/) |
| Thin controllers | API | Controllers delegate to MediatR handlers | Enforced | [TasksController.cs](../../../src/TeamTasker.API/Controllers/TasksController.cs), [ProjectsController.cs](../../../src/TeamTasker.API/Controllers/ProjectsController.cs) |
| Centralized validation pipeline | Application | FluentValidation validators auto-registered and executed via MediatR pipeline behavior | Enforced | [DependencyInjection.cs:19-20](../../../src/TeamTasker.Application/DependencyInjection.cs#L19-L20), [ValidationBehavior.cs](../../../src/TeamTasker.Application/Common/Behaviors/ValidationBehavior.cs) |
| Request logging behavior | Application | MediatR pipeline logs request identity and duration for every command/query | Enforced | [LoggingBehavior.cs](../../../src/TeamTasker.Application/Common/Behaviors/LoggingBehavior.cs) |
| Error contract normalization | API | Global middleware + `ApiBehavior` enforce `{status, title, detail, errors}`; some controllers still return ad-hoc `{message}` payloads | Partial — gap **G3** | [ExceptionHandlingMiddleware.cs:47-53](../../../src/TeamTasker.API/Middleware/ExceptionHandlingMiddleware.cs#L47-L53), [Program.cs:34-56](../../../src/TeamTasker.API/Program.cs#L34-L56), [AuthController.cs:230](../../../src/TeamTasker.API/Controllers/AuthController.cs#L230), [TasksController.cs:69](../../../src/TeamTasker.API/Controllers/TasksController.cs#L69) |
| Pagination contract normalization | Application | `PaginatedList<T>` exposes aliases `data`/`total`/`page`/`limit`/`totalPages` for frontend contract consistency | Enforced | [PaginatedList.cs](../../../src/TeamTasker.Application/Common/Models/PaginatedList.cs) |
| Transport mapping boundary | Application | `TaskDto` mapper translates domain enums and relationship shape to the frontend contract vocabulary | Enforced | [TaskDto.cs](../../../src/TeamTasker.Application/Common/DTOs/TaskDto.cs) |

---

## 6) Gaps & Constraint Mismatches (Consolidated)

| ID | Gap | Severity | Recommended Owner | Evidence |
|---|---|---|---|---|
| **G1** | `UpdateTaskCommandValidator` does not enforce the 20-tag cap that `CreateTaskCommandValidator` enforces. Tag count can exceed the intended limit via update. | Low | Backend / Application layer | [UpdateTaskCommandValidator.cs:54-55](../../../src/TeamTasker.Application/Tasks/Commands/UpdateTask/UpdateTaskCommandValidator.cs#L54-L55) |
| **G2** | Server-side `POST /auth/logout` does not revoke the active refresh token. A leaked refresh token remains valid until natural rotation or 7-day expiry even after the user "logs out." | **High** | Backend / API + Infrastructure | [AuthController.cs:219-230](../../../src/TeamTasker.API/Controllers/AuthController.cs#L219-L230), [RefreshTokenService.cs](../../../src/TeamTasker.Infrastructure/Authentication/RefreshTokenService.cs) |
| **G3** | Ad-hoc `{message}` payloads in `AuthController` and `TasksController` violate the global error contract `{status, title, detail, errors}`. Strict clients that parse `ProblemDetails` may fail. | Medium | Backend / API | [AuthController.cs:230](../../../src/TeamTasker.API/Controllers/AuthController.cs#L230), [TasksController.cs:69](../../../src/TeamTasker.API/Controllers/TasksController.cs#L69) |
| **G4** | No `TeamsController` / Teams CQRS surface exists despite `Team` and `TeamMember` domain entities. Projects API has no dedicated status-transition endpoint (handled only via generic Update). | Medium | Backend / API + Application | Backend tree; [ProjectsController.cs](../../../src/TeamTasker.API/Controllers/ProjectsController.cs), [Team.cs](../../../src/TeamTasker.Domain/Entities/Team.cs) |
| **G5** | No Project Management UI in the frontend (no list, create, or detail views) despite full backend support for projects. | Medium | Frontend | [teamtasker_fe/app](../../../../teamtasker_fe/app/) tree |
| **G6** | Kanban drag-drop has no optimistic UI and no rollback on mutation failure. Users see the card appear to move; on API failure the card stays until cache invalidation, producing silent desync. | Medium | Frontend | [tasks-kanban.tsx:52-66](../../../../teamtasker_fe/components/tasks/tasks-kanban.tsx#L52-L66) |
| **G7** | Task Dialog has no inline field-level validation UX. Server errors surface only via toast; users cannot see per-field error messages. | Low | Frontend | [task-dialog.tsx](../../../../teamtasker_fe/components/tasks/task-dialog.tsx) |
| **G8** | RBAC is gated on the client for UI affordances (e.g., delete button hidden for non-admins) with no confirmation prompt before destructive action. Defense in depth depends entirely on backend authorization. | Low | Frontend | [tasks-list.tsx:104,263](../../../../teamtasker_fe/components/tasks/tasks-list.tsx#L104) |
| **G9** | No offline support, no undo functionality. All mutations depend on live API connectivity. | Low | Frontend platform | Frontend tree |
| **G10** | No dedicated admin console. `/app/team` exists but is not an admin-only user-management surface. | Low | Frontend | [teamtasker_fe/app](../../../../teamtasker_fe/app/) tree |

Severity legend: **High** = security / data-integrity impact; **Medium** = user-visible behavior or contract mismatch; **Low** = polish / consistency improvement.

---

## Change Log

| Date | Change | By |
|---|---|---|
| 2026-04-18 | Catalog created and synchronized with source code. Verified 35/39 original rules; documented 4 confirmed discrepancies (G1–G4) and 6 additional findings (G5–G10). | Audit run |
