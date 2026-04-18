---
title: P1 Correctness and Silent UX Bugs Implementation Plan
description: Step 1 execution blueprint to resolve all P1 correctness and silent UX issues from the integration audit.
author: TeamTasker Engineering
ms.date: 2026-04-16
ms.topic: how-to
keywords:
  - p1 remediation
  - correctness fixes
  - ux bug fixes
estimated_reading_time: 14
---

## Scope and Objectives

This plan resolves all yellow P1 findings in the integration audit while preserving P0 behavior already implemented.

Objectives:

* Eliminate silent frontend failures caused by contract drift and non-actionable errors
* Enforce role-aware authorization semantics on backend write paths without regressing core flows
* Remove stale or unsupported API usage in frontend hooks and config surfaces
* Add server-side route protection to reduce client-side auth flash behavior

## In Scope

* User transport contract compatibility for `name` display fields
* Task status and priority enum alignment across backend transport and frontend type and UI handling
* 403 error surfacing in frontend global API listener
* API base URL and timeout normalization plus stale mock/reference cleanup
* RBAC hardening on selected write endpoints and client-side admin action gating
* Middleware route protection using auth cookie presence checks
* Removal of unsupported frontend user mutation hooks for non-existent backend endpoints
* Activity feed invalidation from task mutation hooks
* Verification of token-expiry surfacing correctness introduced in prior P0 work

## Out of Scope

* Migration from localStorage token storage to httpOnly cookie auth
* Full Teams, Activities, or Communication backend implementation
* Full role and permission matrix redesign beyond P1 safeguards

## Measurable Outcomes

* User list and detail payloads include a `name` field consumed by frontend views
* Task transport status and priority values are consistent and explicit across backend and frontend
* 403 responses raise visible user feedback through global toast handling
* Frontend API config exposes one consistent base URL and timeout value
* Legacy Mockoon base URL reference is removed from shipped docs and unused proxy middleware file is removed
* Backend write endpoints targeted in this plan include explicit role-aware authorization attributes
* Middleware blocks unauthenticated access to protected app routes and redirects authenticated users away from auth pages
* Frontend code no longer exports unsupported user create update delete API hooks against missing backend endpoints
* Task mutation hooks invalidate activity query keys to avoid stale dashboard feeds

## Workstream Breakdown

### WS1 User Contract Compatibility

Implement a frontend-compatible `name` field in backend user DTOs while retaining existing `fullName` fields.

Planned behavior:

* Extend `UserDto` and `UserDetailDto` with `Name`
* Map `Name` from domain full-name in user controller mapping paths

### WS2 Task Enum Contract Alignment

Align backend task transport values and frontend task type unions.

Planned behavior:

* Return explicit client status strings for all domain task states
* Return explicit client priority strings including `critical`
* Expand frontend task status and priority unions accordingly
* Update task list and kanban UI handling to gracefully render expanded status values

### WS3 Authorization and RBAC Hardening

Introduce explicit role-aware authorization on write endpoints in scope and expose role context to frontend.

Planned behavior:

* Add role claim-type handling in JWT bearer validation configuration
* Add role-aware authorize attributes on selected write actions in projects and tasks controllers
* Extend auth response contract with user role to support client-side action gating
* Gate admin-only UI actions where backend write path is admin-restricted

### WS4 Route Protection and Error Visibility

Reduce auth-route flash and silent permission denials.

Planned behavior:

* Update Next.js middleware to enforce protected-route redirects using auth cookie presence
* Mirror token presence in browser cookie lifecycle helpers used by middleware
* Add forbidden-event dispatch in API client and forbidden toast handling in API error listener

### WS5 API Config and Legacy Surface Cleanup

Normalize API configuration and remove stale artifacts.

Planned behavior:

* Standardize API timeout and base URL values across config and client layers
* Update stale base URL reference in `BE APIs/README.md`
* Remove unused `proxy.js`

### WS6 Frontend Hook Correctness

Remove unsupported user mutation calls and ensure task updates invalidate activity views.

Planned behavior:

* Remove create update delete user endpoint wrappers and corresponding hooks
* Keep supported profile and user-read hooks intact
* Add activity query invalidation on task create update status progress delete mutations

### WS7 Test and Verification Coverage

Validate P1 changes through targeted backend tests and static diagnostics.

Planned behavior:

* Update and run backend mapper tests for new task enum mapping expectations
* Run targeted API unit tests for auth and tasks if affected
* Run backend solution build and selected tests for regression guard
* Capture frontend validation constraints if runtime toolchain remains unavailable

## Dependencies

* Existing standards references:
  * `docs/project/standards/engineering_standards.md`
  * `docs/project/standards/FAANG_Documentation_Standards.md`
  * `docs/testing/testing_strategy.md`
* Existing P0 artifacts and behavior:
  * refresh token issuance and refresh endpoint
  * tasks vertical slice and pagination aliases

## Risks and Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Over-restrictive role guards could regress task workflows for non-admin users | Feature regression | Apply role-aware guards to explicitly admin-scoped operations and preserve authenticated access where required |
| Expanding task status values may cause unrendered tasks in existing 3-column kanban | Hidden tasks in UI | Add explicit grouping fallback in kanban and status rendering helpers |
| Middleware cookie presence check could misclassify auth state | Redirect loops or false redirects | Keep route allowlist explicit and mirror token cookie updates in auth helper set/remove paths |
| Removing unsupported hooks may break imports | Frontend build failures | Confirm no external call sites before deletion and run diagnostics after edits |

## Assumptions

* Browser token remains localStorage-backed in this phase and cookie mirror is used only for route gating
* Frontend role-gating will use role values already minted by backend JWT generator (`User`, `Admin`)
* Unimplemented backend user management write endpoints should not be exposed by frontend hooks in this phase

## File and Module Breakdown

### Backend

* `src/Core/TeamTasker.Application/Users/Models/UserDto.cs`
* `src/Core/TeamTasker.Application/Users/Models/UserDetailDto.cs`
* `src/Presentation/TeamTasker.API/Controllers/UsersController.cs`
* `src/Core/TeamTasker.Application/Tasks/Common/TaskDto.cs`
* `src/Presentation/TeamTasker.API/Controllers/ProjectsController.cs`
* `src/Presentation/TeamTasker.API/Controllers/TasksController.cs`
* `src/Core/TeamTasker.Application/Auth/Models/AuthResponse.cs`
* `src/Presentation/TeamTasker.API/Controllers/AuthController.cs`
* `src/Infrastructure/TeamTasker.Infrastructure/DependencyInjection.cs`
* `tests/TeamTasker.Application.UnitTests/Tasks/TaskContractMapperTests.cs`

### Frontend

* `lib/api/types/tasks.ts`
* `components/tasks/tasks-list.tsx`
* `components/tasks/tasks-kanban.tsx`
* `components/tasks/task-dialog.tsx`
* `lib/api/client.ts`
* `components/api-error-listener.tsx`
* `lib/auth.ts`
* `middleware.ts`
* `lib/api/config.ts`
* `constants/app.ts`
* `lib/api/endpoints/users.ts`
* `lib/api/hooks/useUsers.ts`
* `lib/api/hooks/useTasks.ts`
* `BE APIs/README.md`
* `proxy.js` (delete)
* `lib/api/types/auth.ts`
* `context/auth-context.tsx`

### Documentation Artifacts

* `docs/project/execution/p1_correctness_silent_ux_bugs_implementation_plan.md`
* `docs/project/execution/p1_correctness_silent_ux_bugs_plan_compliance_check.md`
* `docs/project/execution/p1_correctness_silent_ux_bugs_acceptance_criteria.md`
* `docs/project/execution/p1_correctness_silent_ux_bugs_acceptance_verification.md`
* `docs/project/execution/p1_correctness_silent_ux_bugs_run_validation_report.md`
* `changelog.md`

## Execution Sequence

1. Apply backend transport and authorization updates (WS1, WS2, WS3)
2. Apply frontend contract, routing, error, and hook updates (WS2, WS4, WS5, WS6)
3. Run build and targeted tests, then capture verification outputs (WS7)
4. Publish acceptance verification and run-validation artifacts
5. Update changelog with traceability links

## Completion Definition

P1 implementation is complete when all acceptance criteria pass or are explicitly blocked with environment evidence, no new regressions are introduced in targeted backend validations, and execution artifacts are fully documented.
