# TeamTasker — Strategic Application Optimization Report

**Prepared:** 2026-04-18
**Scope:** TeamTasker full stack (Next.js 15 frontend, .NET 8 backend, EF Core 8 + SQL Server, Clean Architecture + CQRS via MediatR)
**Baseline:** [business_rule_catalog.md](../standards/business_rule_catalog.md) — current-state ground truth as of this date.
**Audience:** Engineering leadership, product, and architecture review.

---

## Executive Summary

TeamTasker is a solidly engineered codebase with a disciplined Clean Architecture, working CQRS boundaries, and strong baseline security (JWT + refresh-token rotation, server-side scope filtering for non-admin task reads). The gaps we found are not foundational — they are *productization* gaps: missing optimistic UX, incomplete multi-tenant isolation, an unrevoked server-side logout, a handful of non-normalized error payloads, and a missing frontend surface for Projects and Teams despite backend readiness. None of these require architectural rework; they require targeted investment along a sequenced roadmap.

This report proposes **21 action items across 4 priority tiers (P0–P3)**. P0 items (G1, G2, G3 from the catalog) are safe, same-day fixes with material security and contract benefits. P1 items deliver the largest user-visible UX improvement per engineering hour. P2 items make TeamTasker multi-tenant-ready. P3 items are the platform investments that let the system scale to activity-feed volume seen at FAANG-scale task trackers.

---

## 1. Usability & User Flows

### 1.1 Current State Assessment

| Surface | Current Behavior | Reference |
|---|---|---|
| **Kanban board** | 6-column board (`todo`, `in-progress`, `completed`, `blocked`, `on-hold`, `cancelled`). Drag-drop via native HTML5 `draggable` + `onDrop`. Calls `updateTaskStatus.mutate()` on drop; no optimistic state update, no rollback. | [tasks-kanban.tsx:40-66](../../../../teamtasker_fe/components/tasks/tasks-kanban.tsx#L40-L66) |
| **Task Dialog** | Radix Dialog with title/description/status/priority/dueDate/assignee/progress/tags. Normalizes `dueDate`, clamps progress, validates assignee presence. No inline field-level error messaging; failures surface as toasts only. | [task-dialog.tsx](../../../../teamtasker_fe/components/tasks/task-dialog.tsx) |
| **Tasks list** | Table view with search, status/priority filters. Delete action hidden for non-admin. No bulk selection or bulk actions. | [tasks-list.tsx:104,263](../../../../teamtasker_fe/components/tasks/tasks-list.tsx#L104) |
| **Navigation** | App-router groups `(auth)`, `(dashboard)`, `tasks`, `team`, `profile`, `development`, `communication`. No global command palette. No breadcrumbs on `/app/tasks/[id]`. No Projects UI. | [teamtasker_fe/app](../../../../teamtasker_fe/app/) |

### 1.2 Friction Points

**Kanban**
- **Perceived lag on drop.** The card visually moves only after the server round-trip completes. Comparable tools (Linear, Notion, Jira Next-Gen) commit locally in < 16 ms.
- **Silent desync on API failure.** If the mutation fails, the card remains visually in the new column until `queryClient.invalidateQueries` re-fetches. Users assume the move succeeded.
- **Small-viewport collapse.** Six columns side-by-side overflow on < 1440 px. The native `overflow-x` scrollbar is discoverable but disruptive to flow.
- **No swim-lanes.** At 50+ tasks per column, visual scanning by assignee or priority becomes impossible.

**Task Dialog**
- **No per-field validation messages.** The underlying validators (FluentValidation on the backend) return structured errors via the `errors` array in ProblemDetails, but the dialog does not bind them to fields.
- **No keyboard shortcuts.** No `Esc` to close, no `Cmd/Ctrl+Enter` to submit.
- **Raw `<input type="date">`.** No presets ("Today", "Tomorrow", "Next Monday"), no relative-date input ("in 2 weeks").
- **Assignee `<select>` without type-ahead.** At 100+ users this breaks.
- **No draft persistence.** Accidentally closing the dialog discards in-progress work.

**Navigation**
- **No command palette.** Power users cannot jump to a task / project / user without navigating a menu.
- **No breadcrumbs.** `/app/tasks/[id]` has no way back to a filtered list context.
- **No bulk actions.** Admins cannot reassign, relabel, or delete multiple tasks at once.
- **No Projects UI at all.** The backend supports projects fully but the frontend cannot create, list, or view them.

### 1.3 Recommendations

| # | Recommendation | Stack | Rationale |
|---|---|---|---|
| U1 | Add optimistic update + rollback to Kanban drag-drop using TanStack Query `onMutate` / `onError` / `onSettled` pattern, keyed by task list query. | Frontend | Eliminates perceived lag and silent desync. Industry baseline for task boards. |
| U2 | Refactor Task Dialog to use `react-hook-form` + `zod` with a shared schema mirroring FluentValidation rules. Bind server-side `errors[field]` payload from ProblemDetails into field error state. | Frontend | Single source of truth for validation; users see per-field errors without a full submit cycle. |
| U3 | Add keyboard shortcuts (`Esc` close, `Cmd+Enter` submit) via Radix Dialog's built-in focus management. | Frontend | FAANG-grade keyboard ergonomics. 30-min change. |
| U4 | Replace raw date input with a date picker (shadcn `<Calendar>` already available) with presets: Today, Tomorrow, Next Week, Custom. | Frontend | Measurably faster due-date selection; consistent with design system. |
| U5 | Replace assignee `<select>` with `cmdk`-powered combobox, type-ahead against the paginated users endpoint. | Frontend | Scales to 100+ users. |
| U6 | Persist Task Dialog drafts to `sessionStorage` keyed by `taskId` (or `"new"`). Prompt user on reopen: "Restore unsaved changes?". | Frontend | Protects user work; common SaaS pattern. |
| U7 | Swim-lanes in Kanban: group rows by assignee, priority, or "none". Toggle via header dropdown. | Frontend | Scales the board view to realistic team sizes. |
| U8 | Global command palette (`cmdk`) with actions: go-to-task, go-to-project, create-task, search. Bind to `Cmd/Ctrl+K`. | Frontend | High-leverage power-user feature. |
| U9 | Breadcrumbs in task/project detail routes. Use Next.js `useSelectedLayoutSegments` or a shadcn `<Breadcrumb>`. | Frontend | Navigation recoverability. |
| U10 | Bulk select + bulk actions (assign, status-change, delete) on Tasks list for Admins. Requires a new `PATCH /tasks/bulk` endpoint. | Full-stack | Operational efficiency for admins managing large backlogs. |
| U11 | Build `/app/projects` surface: list, create, detail, edit views. Wire to existing `/api/projects` endpoints. | Frontend | Closes gap **G5**. Backend is already ready. |

---

## 2. Business Logic Enhancements

### 2.1 Current State Assessment

The domain layer is robust: `Task.cs:58-135` implements symmetric status↔progress coupling cleanly, all mutations emit domain events, and `CreateTaskCommand` has a three-tier project-resolution fallback. However, there is no passive state-change logic (stale-task handling), no intelligence in assignment, and domain events currently flow only through the MediatR pipeline — they do not fan out to notifications or an audit log.

### 2.2 Recommendations

| # | Recommendation | Details | Closes Gap |
|---|---|---|---|
| B1 | **Fix update-task tag cap** — add `RuleFor(x => x.Tags).Must(t => t == null || t.Count <= 20).WithMessage("Cannot exceed 20 tags")` to `UpdateTaskCommandValidator`. | 1-line change. Trivial parity fix. | **G1** |
| B2 | **Revoke refresh token on logout** — call `IRefreshTokenService.RevokeRefreshTokenAsync(userId)` from `AuthController.Logout`. Requires reading `userId` from the authenticated principal. | Closes a real session-hijack window (leaked refresh token stays valid until 7-day expiry today). | **G2** |
| B3 | **Stale-task auto-transition** — background job that moves tasks with `Status ∈ {ToDo, InProgress}` and `UpdatedDate < UtcNow - N days` to `OnHold`, emitting `TaskStatusUpdatedEvent`. Configurable threshold per project. Use a hosted service with `PeriodicTimer`. | Reduces backlog stagnation; makes "Blocked" and "OnHold" meaningful. |  |
| B4 | **Workload balancing service** — `IWorkloadService` computes per-assignee metrics (active-task count, progress-weighted effort, overdue count). Surfaced in Task Dialog as "Suggest assignee" chip showing least-loaded eligible user. | Non-blocking suggestion; user can override. Backend computation only, no DB schema change needed initially — later can be pre-aggregated into a materialized view. |  |
| B5 | **Notification pipeline** — new MediatR `NotificationDispatchBehavior` subscribes to domain events via a `INotificationService`. Fans out on `TaskAssignedEvent`, `TaskStatusUpdatedEvent` (Blocked/Done), `TaskCreatedEvent`, `TaskAddedToProjectEvent`. Delivery channels: in-app via SignalR hub, email via queued background job. | Domain events already exist — this is pure Infrastructure/wiring work. |  |
| B6 | **Dedicated project status-transition endpoint** — `POST /api/projects/{id}/status` with `UpdateProjectStatusCommand`; align vocabulary to kebab-case (`not-started`, `in-progress`, `on-hold`, `completed`, `cancelled`) matching tasks. | Consistent contract with tasks; frees `UpdateProjectCommand` to focus on descriptive fields. | **G4 (project half)** |
| B7 | **Teams CQRS surface** — `TeamsController` with list/create/update/delete + membership commands (`AddMemberCommand`, `RemoveMemberCommand`). Team entity already exists. | Unblocks UI features that depend on teams (swim-lanes, team-scoped views). | **G4 (teams half)** |
| B8 | **Project-level task auto-assignment policy** — optional `DefaultAssigneeId` + `AutoAssignStrategy` (`RoundRobin` \| `LeastLoaded` \| `None`) on Project. Applied at task-creation time if not explicitly assigned. | Aligns with B4 once workload data exists. |  |

---

## 3. Data Visibility & Security (RBAC Deep Dive)

### 3.1 Current RBAC Implementation

**Strengths:**
- **JWT role claim** is mapped correctly via `DependencyInjection.cs`, and every state-changing endpoint uses `[Authorize(Roles=...)]` (catalog Section 3).
- **Server-side scope filtering** on non-admin task reads: `GetTasksQuery.ApplyVisibilityScope` at [GetTasksQuery.cs:118-140](../../../src/TeamTasker.Application/Tasks/Queries/GetTasks/GetTasksQuery.cs#L118-L140) returns all tasks for Admin but filters non-admins by `AssignedToUserId` OR team membership. This is the right pattern.
- **Frontend middleware** validates the auth cookie server-side before serving protected routes. localStorage alone cannot bypass.
- **Proactive refresh + 401 handling** correctly excludes pre-auth endpoints from auto-logout.

**Weaknesses:**

| Weakness | Impact |
|---|---|
| **No resource-level authorization.** A non-admin `GET /projects/{id}` returns any project, even ones the caller has no membership or task-stake in. | Horizontal data leak between users in the same tenant. |
| **No tenant isolation.** There is no `TenantId` column on any aggregate root. The system is effectively single-tenant. | Not deployable as multi-tenant SaaS; all data co-mingles. |
| **No audit log.** Privileged operations (role changes, deletions) are untraceable after the fact. | Compliance gap (SOC 2, ISO 27001 baseline); no forensic capability. |
| **Server-side logout does not revoke refresh token** (catalog G2). | Session-invalidation gap; leaked refresh token survives user "logout." |
| **Frontend delete gating is client-only** (catalog G8). | Not a vulnerability (backend enforces) but no user-facing confirmation before destructive action. |
| **Error contract drift** (catalog G3). | Strict clients that parse `ProblemDetails` can crash on ad-hoc `{message}` payloads. |

### 3.2 Recommendations

| # | Recommendation | Details | Closes Gap |
|---|---|---|---|
| S1 | **Introduce resource-level authorization** via ASP.NET Core `IAuthorizationHandler`. Example: `TaskAccessRequirement` — allow if user is Admin, assignee, or on the project's team. Wire via `[Authorize(Policy="CanAccessTask")]` on `GET /tasks/{id}`, `UPDATE`, `DELETE`. Mirror for projects. | Closes horizontal data leaks. Centralizes the logic that is currently duplicated in `GetTasksQuery.ApplyVisibilityScope`. |  |
| S2 | **Add `TenantId` to aggregate roots** (`Project`, `Task`, `User`, `Team`). Derive `ICurrentUserService.TenantId` from a JWT claim. Add a global EF Core query filter in `ApplicationDbContext.OnModelCreating` that appends `WHERE TenantId = @currentTenant` to every query. | Multi-tenant-ready. Single place to enforce — cannot be bypassed by a handler that forgets to filter. Requires migration + backfill. |  |
| S3 | **Audit log pipeline behavior** — `AuditBehavior : IPipelineBehavior<TRequest, TResponse>` captures actor (user ID), action (request type), resource (aggregate ID from response), before/after snapshot (JSON), and persists to `AuditLogs` table. Register after `ValidationBehavior`. | Attaches to every command with zero per-handler changes. Pattern mirrors existing `LoggingBehavior`. |  |
| S4 | **Normalize all error payloads to `ProblemDetails`** — replace `return Ok(new { message = ... })` in `AuthController.cs:230` and `TasksController.cs:69` (and any others) with `return Problem(detail: ..., statusCode: ...)`. | Closes **G3**. Enables strict client parsers and better telemetry. | **G3** |
| S5 | **Revoke refresh token on logout** (duplicate of B2 — same ticket). | Closes **G2**. | **G2** |
| S6 | **Confirmation dialog on destructive actions** — shadcn `<AlertDialog>` wrapping every delete button (tasks, projects, users, teams). "Type the task title to confirm" for permanent deletions. | Closes **G8**. Defense-in-depth UX layer. | **G8** |
| S7 | **Rate limiting on auth endpoints** — ASP.NET Core `AddRateLimiter` with a fixed-window limiter on `/auth/login` (5 requests / min / IP), `/auth/refresh` (20 / min / user). | Mitigates credential stuffing and refresh-token abuse. |  |
| S8 | **Session-device fingerprinting** — extend refresh-token record with `DeviceId`, `UserAgent`, `IpAddress`. Allow users to list and revoke individual sessions ("Sign out other devices"). | Enables account-takeover response workflow. |  |

---

## 4. Database Schema Evaluation

### 4.1 Current EF Core Schema

Confirmed EF configurations ([Configurations](../../../src/TeamTasker.Infrastructure/Persistence/Configurations/)):
- **Project → Tasks**: cascade delete (correct — tasks are owned by projects).
- **Project → Team**: SetNull (correct — orphaning is benign).
- **Task → AssignedToUser**: SetNull (correct — user deletion shouldn't destroy task history).
- **Task → TaskTags**: cascade (correct — tags are owned).
- **TeamMember**, **UserSkill**: cascade on both sides (correct for join tables).
- **User.Email**, **User.Username**: unique indexes.

No composite indexes detected on Task query patterns.

### 4.2 Indexing Audit

| Index | Target Query | Priority |
|---|---|---|
| `IX_Tasks_AssignedToUserId_Status` | Dashboard: "my open tasks" | **High** — hits on every non-admin task list render. |
| `IX_Tasks_ProjectId_Status` | Kanban: tasks grouped by status within a project | **High** — hit per Kanban render. |
| `IX_Tasks_DueDate` | Overdue-task banners, stale-task background job (B3) | Medium |
| `IX_Tasks_UpdatedDate` | Stale-task job (B3) | Medium |
| `IX_TaskTags_Name` (or `IX_TaskTags_TaskId_Name`) | Tag-filter queries | Medium |
| `IX_AuditLogs_UserId_CreatedDate` | Audit viewer (future, per S3) | Medium (when built) |
| `IX_OutboxMessages_ProcessedOn_OccurredOn` | Outbox dispatcher (P3, D5) | High (when built) |
| `IX_*_TenantId` on every aggregate | Global query filter support (S2) | **Required** if S2 proceeds. |

### 4.3 Relationship Modeling

| # | Recommendation | Rationale |
|---|---|---|
| D1 | **Composite indexes as above.** Start with `(AssignedToUserId, Status)` and `(ProjectId, Status)`; measure before adding more. | Low-risk, high-impact. Add in a migration. |
| D2 | **Subtasks: add `ParentTaskId` (self-referencing FK, nullable) on `Task`.** Configure `Restrict` on delete to prevent orphan subtrees. Expose as nested list in Task Dialog. | Major UX feature ("break this down"). Common SaaS baseline. |
| D3 | **Watchers: many-to-many `TaskCollaborators` (TaskId, UserId, Role).** `Role` ∈ {assignee, watcher, reviewer}. Replace the single `AssignedToUserId` column over time with a primary-assignee flag on this table. | Enables multi-assignee UX and notification-opt-in without schema changes. |
| D4 | **Project → Teams M:N (currently 1:1 via `TeamId`).** Introduce `ProjectTeam` join if a project can span multiple teams. Defer until product signals demand. | Don't build prematurely — flagged as a watchlist item, not a P-level action. |
| D5 | **Outbox pattern for domain events.** New `OutboxMessages (Id, AggregateType, AggregateId, EventType, Payload, OccurredOn, ProcessedOn)` table. Command handlers write events to outbox in the same transaction as state mutation; a `BackgroundService` polls and dispatches via MediatR `INotificationPublisher` or external broker. | Decouples event handling from request path; gives at-least-once delivery guarantees; required before B5 goes to production scale. |

### 4.4 Scalability for High-Volume Activity Feed

The current design invokes domain event handlers synchronously inside MediatR's command pipeline. At task-heavy-tenant scale (10k+ tasks, 100+ concurrent editors), this will dominate request latency.

| # | Recommendation | Rationale |
|---|---|---|
| D6 | **Denormalized `ActivityFeedProjection` read model** — one row per event, columns `(Id, UserId, ActorId, Verb, ObjectType, ObjectId, OccurredOn, Metadata)`. Index `(UserId, OccurredOn DESC)`. Populated by an event handler subscribed to the outbox (D5). | Read path is a single indexed range query; no joins across Tasks/Projects/Users per feed render. |
| D7 | **Paginated feed via keyset pagination** — `WHERE UserId = @u AND OccurredOn < @lastSeen ORDER BY OccurredOn DESC LIMIT 50`. Not offset pagination. | O(log n) per page regardless of depth. Required for multi-thousand-event histories. |
| D8 | **At Redis-stream scale or beyond** — if feed throughput exceeds a dedicated SQL partition, move the projection to Redis Streams (trim by length) with a fallback archive to cold storage. | Platform-scale move; only when metrics justify. |
| D9 | **Database read replicas + CQRS read-side routing** — register separate `IReadDbContext` bound to a read-replica connection string; route queries via MediatR `IRequestHandler` interface separation. | Existing CQRS pattern already separates read/write conceptually — this is the physical realization. |

---

## 5. Actionable Roadmap (Prioritized)

Roadmap is sequenced by *security/contract impact → UX leverage → architectural maturity → platform-scale investment*.

### P0 — Ship Immediately (< 1 day each)

| # | Action | Layer | Expected Impact | Closes |
|---|---|---|---|---|
| P0-1 | Add 20-tag cap to `UpdateTaskCommandValidator` (B1). | Application | Parity with create; data-integrity. | **G1** |
| P0-2 | Revoke refresh token on logout (B2 / S5). | API + Infrastructure | Closes session-hijack window post-logout. | **G2** |
| P0-3 | Replace ad-hoc `{message}` payloads in `AuthController` and `TasksController` with `Problem(...)` (S4). | API | Error-contract consistency; strict-client safety. | **G3** |

### P1 — High-Leverage UX & Feature Completion (1–2 sprints)

| # | Action | Layer | Expected Impact |
|---|---|---|---|
| P1-1 | Optimistic Kanban drag-drop with TanStack Query rollback (U1). | Frontend | Removes perceived lag; eliminates silent desync on failure. |
| P1-2 | `react-hook-form` + `zod` refactor of Task Dialog with server-side error binding (U2). | Frontend | Per-field validation UX; reduces support volume. |
| P1-3 | Build `/app/projects` surface (U11). | Frontend | Closes **G5**; unblocks project-scoped workflows. |
| P1-4 | `UpdateProjectStatusCommand` + endpoint (B6). | Backend | Closes **G4 (project half)**; contract alignment with tasks. |
| P1-5 | Composite indexes `(AssignedToUserId, Status)` and `(ProjectId, Status)` (D1). | Infrastructure (migration) | Query latency improvement on hottest paths. |
| P1-6 | Keyboard shortcuts + date picker + assignee combobox (U3, U4, U5). | Frontend | FAANG-grade ergonomics. |
| P1-7 | Confirmation dialog on destructive actions (S6). | Frontend | Closes **G8**; reduces accidental deletion. |
| P1-8 | Rate limiting on `/auth/login` and `/auth/refresh` (S7). | API | Mitigates credential stuffing. |

### P2 — Architectural Maturity (2–4 sprints)

| # | Action | Layer | Expected Impact |
|---|---|---|---|
| P2-1 | `TenantId` + EF global query filter (S2). | Domain + Infrastructure | Multi-tenant SaaS readiness. |
| P2-2 | Resource-level authorization handlers (S1). | API + Application | Closes horizontal data-leak risk on project/user reads. |
| P2-3 | Audit log pipeline behavior (S3). | Application + Infrastructure | Compliance baseline; forensic capability. |
| P2-4 | `IWorkloadService` + suggest-assignee (B4). | Application + Frontend | Reduces assignment guesswork. |
| P2-5 | Stale-task auto-transition background job (B3). | Infrastructure | Backlog hygiene; makes "OnHold" meaningful. |
| P2-6 | Teams CQRS surface (B7). | Application + API + Frontend | Closes **G4 (teams half)**; unblocks swim-lanes. |
| P2-7 | `TaskCollaborators` many-to-many (D3). | Domain + Infrastructure | Watchers + multi-assignee capability. |
| P2-8 | Draft persistence + bulk actions + breadcrumbs + swim-lanes (U6, U7, U9, U10). | Frontend (+ bulk endpoint backend) | Power-user feature set. |

### P3 — Platform Investment (quarter-scale)

| # | Action | Layer | Expected Impact |
|---|---|---|---|
| P3-1 | Outbox pattern for domain events (D5). | Infrastructure | Decouples event fan-out from request path; at-least-once delivery. |
| P3-2 | `ActivityFeedProjection` + keyset pagination (D6, D7). | Infrastructure + Application | Scales activity feed to 10k+ events per user. |
| P3-3 | Notification pipeline behavior + SignalR hub (B5). | Application + API + Frontend | Real-time in-app notifications; email fan-out via queue. |
| P3-4 | Subtasks (`ParentTaskId`) (D2). | Domain + UI | Major UX feature. |
| P3-5 | Session-device fingerprinting + "sign out other devices" (S8). | Infrastructure + UI | Account-takeover response workflow. |
| P3-6 | Read-replica routing via `IReadDbContext` (D9). | Infrastructure | Read-side horizontal scale. |
| P3-7 | Global command palette (U8). | Frontend | Power-user leverage. |
| P3-8 | Admin console `/app/admin` (closes **G10**). | Frontend | User management; role changes; audit log viewer. |

### Reuse Before Rebuild

When executing this roadmap, prefer existing utilities:

- **`PaginatedList<T>`** — already exposes `data/total/page/limit/totalPages`. Any new paginated endpoint should return this type, not a bespoke shape.
- **`ValidationBehavior` + `LoggingBehavior`** — already registered in MediatR pipeline at [DependencyInjection.cs:19-20](../../../src/TeamTasker.Application/DependencyInjection.cs#L19-L20). New cross-cutting behaviors (`AuditBehavior`, `NotificationDispatchBehavior`) should be registered identically.
- **Domain events bus** — every recommended notification/audit path should hook domain events, not add new coupling inside command handlers.
- **`IRefreshTokenService`** — already distributed-cache-backed; extend it for device fingerprinting rather than building a parallel store.
- **shadcn `<Calendar>`, `<AlertDialog>`, `<Breadcrumb>`, `<Command>`** — already in the frontend component library; no new dependencies required for U4, S6, U9, U8.

---

## 6. Risk & Dependencies

| Risk | Mitigation |
|---|---|
| P2-1 (TenantId migration) requires backfill on an existing production database. | Use staged migration: add column nullable, backfill from a derived tenant, then make NOT NULL in a follow-up. |
| P3-1 (Outbox) changes command-handler semantics — events now fire asynchronously. | Introduce per-event handler rather than big-bang. Keep synchronous pipeline for latency-sensitive events (e.g., audit) and move high-volume events (notifications, activity feed) to async first. |
| P1-1 (optimistic Kanban) requires the server to be reliably idempotent on status updates. | Verify `UpdateTaskStatusCommand` is idempotent (it is: setting `Status = Done` twice is a no-op). Confirm before shipping optimistic UI. |
| P2-2 (resource-level authorization) could regress existing behavior if policies are mis-scoped. | Wrap policy wiring in integration tests mirroring `e2e_test_cases.md` matrix before enabling in production. |

---

## 7. How This Report Connects to the Catalog

Every P0 item in this report directly closes a documented gap in the [business rule catalog](../standards/business_rule_catalog.md) (Section 6). P1–P3 items either close additional gaps (G4–G10) or add capability beyond the current catalog baseline. When an item ships, update the catalog's affected rows and move the corresponding gap row to the Change Log.

**Catalog-to-Roadmap mapping:**

| Catalog Gap | Severity | Closed By |
|---|---|---|
| G1 — Update tag cap missing | Low | P0-1 |
| G2 — Logout does not revoke refresh | **High** | P0-2 |
| G3 — Error contract drift | Medium | P0-3 |
| G4 — Teams / Project status endpoint | Medium | P1-4 (project), P2-6 (teams) |
| G5 — No Project UI | Medium | P1-3 |
| G6 — Kanban no optimistic UI | Medium | P1-1 |
| G7 — Task Dialog no inline validation | Low | P1-2 |
| G8 — Client-only delete gating | Low | P1-7 |
| G9 — No offline / undo | Low | (out of current roadmap — platform-level investment) |
| G10 — No admin console | Low | P3-8 |
