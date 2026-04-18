---
title: End-to-End Test Cases
description: Comprehensive E2E test suite covering all critical business flows across the TeamTasker frontend and backend.
author: TeamTasker Engineering
ms.date: 2026-04-17
ms.topic: reference
keywords:
  - e2e tests
  - integration tests
  - acceptance validation
  - test cases
standards_references:
  - docs/project/standards/engineering_standards.md
  - docs/testing/testing_strategy.md
estimated_reading_time: 35
---

## Purpose

This document defines end-to-end test cases that validate full-stack business flows across the TeamTasker Next.js 15 frontend and .NET 8 backend. Each test case specifies the backend API contract expectation and the corresponding frontend UI state change, enabling teams to identify what is working and what requires fixing.

Test cases trace to the acceptance criteria defined in `p0_core_user_journeys_acceptance_criteria.md` (AC-01 through AC-18) and `p1_correctness_silent_ux_bugs_acceptance_criteria.md` (AC-P1-01 through AC-P1-20).

## Conventions

* **Base URL**: `http://localhost:5220/api` (configurable via `NEXT_PUBLIC_API_URL`)
* **Auth header**: `Authorization: Bearer <token>` on all protected endpoints
* **Paginated response shape**: `{ data: T[], total: number, page: number, limit: number, totalPages: number }`
* **Error response shape**: `{ status: number, title: string, detail: string, errors?: Record<string, string[]> }`
* **Task status vocabulary**: `todo`, `in-progress`, `completed`, `blocked`, `on-hold`, `cancelled`
* **Task priority vocabulary**: `low`, `medium`, `high`, `critical`
* **User roles**: `Admin`, `User`

## Strict Contract Enforcement Mode

All tests in this document run in strict contract mode:

* Expected HTTP status codes are exact and non-negotiable
* Expected response shapes are exact and non-negotiable
* Frontend event contracts (custom events and redirect behavior) are exact and non-negotiable
* Fallback parsing behavior in the frontend does not relax API contract assertions
* Any mismatch is a FAIL, even when UX still appears functional

## Verification Status Legend

| Symbol | Meaning |
|---|---|
| PASS | Verified working in current codebase |
| FAIL | Known broken or not yet implemented |
| BLOCKED | Cannot validate (e.g., missing Node toolchain) |
| UNTESTED | Not yet executed |

---

## 1. Authentication and Session Management

### E2E-AUTH-01: User Registration

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-01 |
| **Title** | New user registration issues JWT and refresh token |
| **Traces to** | AC-10, AC-P1-12 |
| **Pre-conditions** | No existing user with email `test-reg@example.com` or username `testreguser` |
| **Steps** | |

1. `POST /api/auth/register` with body:
   ```json
   {
     "firstName": "Test",
     "lastName": "User",
     "email": "test-reg@example.com",
     "username": "testreguser",
     "password": "SecurePass123!"
   }
   ```
2. Inspect response.
3. Navigate to `/register` in frontend and submit the same form.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 201. Response body: `{ id: <int>, username: "testreguser", email: "test-reg@example.com", fullName: "Test User", token: "<jwt>", refreshToken: "<token>", expiresAt: "<iso-datetime>", role: "User" }`. JWT `sub` claim equals user ID. `role` claim equals `"User"`. |
| **Frontend** | Token stored in `localStorage` under `auth_token`. Refresh token stored under `auth_refresh_token`. User redirected to `/dashboard`. Auth context `isAuthenticated` is `true`. User display name renders `"Test User"`. |
| **Status** | UNTESTED |

---

### E2E-AUTH-02: User Login with Username

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-02 |
| **Title** | Existing user logs in with username and receives tokens |
| **Traces to** | AC-10 |
| **Pre-conditions** | User `testreguser` exists with password `SecurePass123!` |
| **Steps** | |

1. `POST /api/auth/login` with body:
   ```json
   {
     "username": "testreguser",
     "password": "SecurePass123!"
   }
   ```
2. Inspect response.
3. Navigate to `/login` in frontend and submit the login form.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body contains `token`, `refreshToken`, `expiresAt`, `id`, `username`, `email`, `fullName`, `role`. |
| **Frontend** | Toast: "Welcome back, Test User!". Redirect to `/dashboard`. Sidebar renders. Auth context populated. |
| **Status** | UNTESTED |

---

### E2E-AUTH-03: User Login with Email

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-03 |
| **Title** | Login accepts email in the username field |
| **Traces to** | AC-10 |
| **Pre-conditions** | User exists with email `test-reg@example.com` |
| **Steps** | |

1. `POST /api/auth/login` with body:
   ```json
   {
     "username": "test-reg@example.com",
     "password": "SecurePass123!"
   }
   ```

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Same shape as E2E-AUTH-02. Backend `AuthRequest.Username` field accepts email per docstring "Username or email". |
| **Frontend** | Same as E2E-AUTH-02. |
| **Status** | UNTESTED |

---

### E2E-AUTH-04: Login with Invalid Credentials

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-04 |
| **Title** | Invalid password returns 401 and frontend shows error toast |
| **Traces to** | AC-09 |
| **Pre-conditions** | User `testreguser` exists |
| **Steps** | |

1. `POST /api/auth/login` with body `{ "username": "testreguser", "password": "WrongPass" }`.
2. Submit the same via frontend login form.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401. Body exactly: `{ status: 401, title: "Unauthorized", detail: "Invalid username or password" }`. Any alternate shape (for example `{ message: "Invalid username or password" }`) is a contract failure. |
| **Frontend** | Error toast renders with non-empty message (reads `detail` field). No redirect. No token stored. |
| **Status** | PASS (verified 2026-04-17 via `e2e/auth.contract.spec.ts`) |

---

### E2E-AUTH-05: Get Current User

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-05 |
| **Title** | Authenticated user retrieves profile via /auth/me |
| **Traces to** | AC-P1-01, AC-P1-12 |
| **Pre-conditions** | Valid JWT token from login |
| **Steps** | |

1. `GET /api/auth/me` with `Authorization: Bearer <token>`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ id, username, email, fullName, token: null, refreshToken: null, expiresAt: null, role }`. |
| **Frontend** | Auth context `user` object populated. Dashboard renders user info. |
| **Status** | UNTESTED |

---

### E2E-AUTH-06: JWT Refresh Token Rotation

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-06 |
| **Title** | Valid refresh token issues new access and refresh tokens |
| **Traces to** | AC-10, AC-11 |
| **Pre-conditions** | User logged in. Valid `refreshToken` stored in localStorage. |
| **Steps** | |

1. `POST /api/auth/refresh-token` with body `{ "refreshToken": "<stored-refresh-token>" }`.
2. Inspect response.
3. Trigger a protected frontend API call while access token is within 5 minutes of expiry (for example `GET /api/tasks`) and observe automatic refresh.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ token: "<new-jwt>", refreshToken: "<new-refresh-token>", expiresAt: "<future-datetime>" }`. New refresh token differs from the submitted one (rotation). Old refresh token is invalidated. |
| **Frontend** | `auth_token` and `auth_refresh_token` in localStorage are silently updated. No page reload. Session continues uninterrupted. |
| **Status** | UNTESTED |

---

### E2E-AUTH-07: Refresh Token Replay Rejection

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-07 |
| **Title** | Replayed (already-rotated) refresh token is rejected |
| **Traces to** | AC-11 |
| **Pre-conditions** | Refresh token from E2E-AUTH-06 step 1 (now rotated/invalidated) |
| **Steps** | |

1. `POST /api/auth/refresh-token` with the old (rotated) refresh token.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401. Body exactly: `{ status: 401, title: "Unauthorized", detail: "Invalid or expired refresh token" }`. No new tokens issued. |
| **Frontend** | Interceptor catches 401. `UNAUTHORIZED_EVENT` dispatched. Toast: "Your session has expired". Redirect to `/login`. localStorage tokens cleared. |
| **Status** | PASS (verified 2026-04-17 via `e2e/auth.contract.spec.ts`) |

---

### E2E-AUTH-08: Logout Clears Session and Cache

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-08 |
| **Title** | Logout clears tokens and full React Query cache |
| **Traces to** | AC-14 |
| **Pre-conditions** | User A logged in. Task list and user list queries cached. |
| **Steps** | |

1. Click logout button in frontend.
2. Observe network call.
3. Log in as User B.
4. Observe dashboard data.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/auth/logout` returns HTTP 200. |
| **Frontend** | `queryClient.clear()` is called (verified in `useLogout` hook). `auth_token`, `auth_refresh_token`, `auth_token_expiry` removed from localStorage. Redirect to `/login`. On User B login, dashboard fetches fresh data with no stale User A cache. |
| **Status** | UNTESTED |

---

### E2E-AUTH-09: Change Password

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-09 |
| **Title** | Authenticated user changes password |
| **Traces to** | — |
| **Pre-conditions** | Valid JWT token. Current password known. |
| **Steps** | |

1. `POST /api/auth/change-password` with body `{ "currentPassword": "SecurePass123!", "newPassword": "NewSecure456!" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ message: "Password changed successfully" }`. |
| **Frontend** | Success toast. User remains logged in. |
| **Status** | UNTESTED |

---

### E2E-AUTH-10: Forgot and Reset Password

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-10 |
| **Title** | Password reset flow issues token and accepts reset |
| **Traces to** | — |
| **Pre-conditions** | User with email `test-reg@example.com` exists |
| **Steps** | |

1. `POST /api/auth/forgot-password` with body `{ "email": "test-reg@example.com" }`.
2. Extract reset token from email/response.
3. `POST /api/auth/reset-password` with body `{ "email": "test-reg@example.com", "token": "<reset-token>", "newPassword": "ResetPass789!" }`.

| **Expected Result** | |
|---|---|
| **Backend** | Step 1: HTTP 200, `{ message: "..." }`. Step 3: HTTP 200, `{ message: "..." }`. |
| **Frontend** | `/forgot-password` page shows success message. User can log in with new password. |
| **Status** | UNTESTED |

---

### E2E-AUTH-11: Invalid Login Does Not Trigger Session Expiration Flow

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-11 |
| **Title** | Login failure remains a local auth error and does not emit global unauthorized event |
| **Traces to** | AC-09 |
| **Pre-conditions** | User is on `/login` with no active session |
| **Steps** | |

1. Submit `/login` form with valid username and invalid password.
2. Observe global browser events and toast stream.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401 with strict error body `{ status: 401, title: "Unauthorized", detail: "Invalid username or password" }`. |
| **Frontend** | Login mutation surfaces an error toast and keeps user on `/login`. `UNAUTHORIZED_EVENT` is not dispatched for `/api/auth/login` failures. No forced logout side effects occur. |
| **Status** | UNTESTED |

---

### E2E-AUTH-12: Refresh Failure During Protected Request Forces Logout

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-12 |
| **Title** | Expired access token plus invalid refresh token clears session on next protected request |
| **Traces to** | AC-11, AC-14 |
| **Pre-conditions** | `auth_token` is expired. `auth_refresh_token` is invalid or revoked. |
| **Steps** | |

1. Trigger any protected request from frontend (for example load `/tasks`).
2. Observe refresh attempt and follow-up behavior.

| **Expected Result** | |
|---|---|
| **Backend** | Protected endpoint returns HTTP 401, refresh endpoint returns HTTP 401 with strict unauthorized error body. |
| **Frontend** | Refresh helper fails, auth storage keys are removed, `UNAUTHORIZED_EVENT` is dispatched, and user is redirected to `/login` with a session-expired toast. |
| **Status** | UNTESTED |

---

### E2E-AUTH-13: Pre-Expiry Proactive Refresh

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-13 |
| **Title** | Client refreshes access token when remaining TTL crosses the pre-expiry threshold |
| **Traces to** | AC-12, AC-P1-17 |
| **Pre-conditions** | Authenticated session with `auth_token_expiry` within pre-expiry window (default 5 minutes). Valid refresh token present. |
| **Steps** | |

1. Seed localStorage with an access token expiring in about 2 minutes.
2. Trigger a protected request (for example `GET /api/users/profile`).
3. Observe request ordering and localStorage mutations.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/auth/refresh-token` returns HTTP 200 with rotated tokens before protected call completes. |
| **Frontend** | Refresh call occurs before protected request, session remains active, tokens are silently updated, and no redirect/toast occurs. |
| **Status** | UNTESTED |

---

### E2E-AUTH-14: Change Password With Incorrect Current Password

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-14 |
| **Title** | Change-password rejects when `currentPassword` is incorrect |
| **Traces to** | AC-09 |
| **Pre-conditions** | Authenticated user with known current password |
| **Steps** | |

1. `POST /api/auth/change-password` with invalid `currentPassword`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400 or 401 with strict error body `{ status, title, detail }`. Password remains unchanged. |
| **Frontend** | Error toast displays non-empty detail and session remains active. |
| **Status** | UNTESTED |

---

### E2E-AUTH-15: Reset Password With Invalid Token

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-15 |
| **Title** | Reset-password rejects invalid or expired token |
| **Traces to** | AC-09 |
| **Pre-conditions** | User exists without a valid active reset token |
| **Steps** | |

1. `POST /api/auth/reset-password` using a tampered or expired token.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400 or 401 with strict `{ status, title, detail }` payload indicating token failure. |
| **Frontend** | Error toast renders detail and reset form remains visible. |
| **Status** | UNTESTED |

---

## 2. Task Lifecycle

### E2E-TASK-01: Create Task

| Field | Value |
|---|---|
| **ID** | E2E-TASK-01 |
| **Title** | Authenticated user creates a task via API and frontend |
| **Traces to** | AC-03 |
| **Pre-conditions** | Authenticated user with role `Admin` or `User`. At least one project exists (for `projectId`). |
| **Steps** | |

1. `POST /api/tasks` with body:
   ```json
   {
     "title": "Implement login page",
     "description": "Build the login page with validation",
     "projectId": 1,
     "priority": "high",
     "dueDate": "2026-05-01T00:00:00Z"
   }
   ```
2. Inspect response.
3. In frontend, open task creation dialog and submit.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 201. Body: `{ id: <int>, title: "Implement login page", description: "...", status: "todo", priority: "high", progress: 0, dueDate: "2026-05-01T00:00:00Z", assignee: null, tags: [], createdAt: "<iso>", updatedAt: "<iso>" }`. |
| **Frontend** | Task list query invalidated. New task appears in list view and Kanban "Todo" column. Success toast. |
| **Status** | UNTESTED |

---

### E2E-TASK-02: Get Task List with Pagination

| Field | Value |
|---|---|
| **ID** | E2E-TASK-02 |
| **Title** | Paginated task list returns aligned response shape |
| **Traces to** | AC-01, AC-08 |
| **Pre-conditions** | At least 15 tasks exist in the database |
| **Steps** | |

1. `GET /api/tasks?page=1&limit=10`.
2. `GET /api/tasks?page=2&limit=10`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ data: Task[10], total: 15, page: 1, limit: 10, totalPages: 2 }`. Page 2: `{ data: Task[5], total: 15, page: 2, limit: 10, totalPages: 2 }`. |
| **Frontend** | List view renders 10 rows. Pagination controls show "Page 1 of 2". Kanban view groups tasks by status column. |
| **Status** | UNTESTED |

---

### E2E-TASK-03: Get Task List with Filtering

| Field | Value |
|---|---|
| **ID** | E2E-TASK-03 |
| **Title** | Task list supports status and priority filters |
| **Traces to** | AC-01 |
| **Pre-conditions** | Tasks exist with mixed statuses and priorities |
| **Steps** | |

1. `GET /api/tasks?status=blocked&page=1&limit=10`.
2. `GET /api/tasks?priority=critical&page=1&limit=10`.
3. `GET /api/tasks?search=login&page=1&limit=10`.

| **Expected Result** | |
|---|---|
| **Backend** | Each call returns HTTP 200 with paginated response. Only tasks matching the filter appear in `data`. Search filter key is `search` (not `searchTerm`). |
| **Frontend** | Filter dropdowns in list/kanban views trigger re-fetch. Displayed tasks match filter criteria. |
| **Status** | UNTESTED |

---

### E2E-TASK-04: Get Task by ID

| Field | Value |
|---|---|
| **ID** | E2E-TASK-04 |
| **Title** | Task detail returns full task with assignee and tags |
| **Traces to** | AC-02 |
| **Pre-conditions** | Task with known ID exists |
| **Steps** | |

1. `GET /api/tasks/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ id, title, description, status, priority, progress, dueDate, assignee: { id, name, avatar, initials } | null, tags: string[], projectId, createdAt, updatedAt }`. Status and priority use client vocabulary. |
| **Frontend** | Task detail panel/modal renders all fields. Assignee avatar displays if assigned. |
| **Status** | UNTESTED |

---

### E2E-TASK-05: Update Task Details

| Field | Value |
|---|---|
| **ID** | E2E-TASK-05 |
| **Title** | Task title, description, priority, and due date update correctly |
| **Traces to** | AC-04 |
| **Pre-conditions** | Existing task. Authenticated user with role `Admin` or `User`. |
| **Steps** | |

1. `PATCH /api/tasks/{id}` with body:
   ```json
   {
     "title": "Updated title",
     "description": "Updated description",
     "priority": "critical",
     "dueDate": "2026-06-01T00:00:00Z"
   }
   ```
2. `GET /api/tasks/{id}` to confirm.

| **Expected Result** | |
|---|---|
| **Backend** | PATCH returns HTTP 200 with updated task. GET confirms persisted values. Priority is `"critical"`. |
| **Frontend** | Task list query invalidated. Task detail query updated via `setQueryData`. Updated values render immediately. Activity feed refreshed (activity query invalidated). |
| **Status** | UNTESTED |

---

### E2E-TASK-06: Update Task Status (All Transitions)

| Field | Value |
|---|---|
| **ID** | E2E-TASK-06 |
| **Title** | Task status transitions through all valid states |
| **Traces to** | AC-05, AC-P1-02, AC-P1-05 |
| **Pre-conditions** | Task in `todo` status |
| **Steps** | |

1. `PATCH /api/tasks/{id}/status` with `{ "status": "in-progress" }` — verify.
2. `PATCH /api/tasks/{id}/status` with `{ "status": "blocked" }` — verify.
3. `PATCH /api/tasks/{id}/status` with `{ "status": "on-hold" }` — verify.
4. `PATCH /api/tasks/{id}/status` with `{ "status": "in-progress" }` — verify.
5. `PATCH /api/tasks/{id}/status` with `{ "status": "completed" }` — verify.
6. `PATCH /api/tasks/{id}/status` with `{ "status": "cancelled" }` — verify.

| **Expected Result** | |
|---|---|
| **Backend** | Each call returns HTTP 200. Returned `status` matches the submitted value. `progress` auto-adjusts: `completed` sets `progress: 100`; `todo` sets `progress: 0`. Domain event `TaskStatusUpdatedEvent` raised per transition. |
| **Frontend** | Kanban: task card moves to the target column on each transition. List: status badge updates. Activity feed query invalidated. All six status values render with correct labels and styling. |
| **Status** | UNTESTED |

---

### E2E-TASK-07: Update Task Progress

| Field | Value |
|---|---|
| **ID** | E2E-TASK-07 |
| **Title** | Task progress updates and auto-adjusts status at boundaries |
| **Traces to** | AC-06 |
| **Pre-conditions** | Task in `in-progress` status, progress at 50 |
| **Steps** | |

1. `PATCH /api/tasks/{id}/progress` with `{ "progress": 75 }`.
2. `PATCH /api/tasks/{id}/progress` with `{ "progress": 100 }`.
3. `PATCH /api/tasks/{id}/progress` with `{ "progress": 0 }`.

| **Expected Result** | |
|---|---|
| **Backend** | Step 1: HTTP 200, `progress: 75`, status stays `in-progress`. Step 2: HTTP 200, `progress: 100`, status auto-changes to `completed` (domain rule `UpdateProgress` sets `Done` at 100). Step 3: HTTP 200, `progress: 0`, status auto-changes to `todo` (domain rule sets `ToDo` at 0). |
| **Frontend** | Progress bar updates. Status badge auto-updates when progress hits 0 or 100. Kanban card relocates to matching column. |
| **Status** | UNTESTED |

---

### E2E-TASK-08: Delete Task

| Field | Value |
|---|---|
| **ID** | E2E-TASK-08 |
| **Title** | Admin deletes a task; subsequent fetch returns 404 |
| **Traces to** | AC-07 |
| **Pre-conditions** | Task exists. Authenticated user with role `Admin`. |
| **Steps** | |

1. `DELETE /api/tasks/{id}`.
2. `GET /api/tasks/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | DELETE: HTTP 204 (No Content). GET: HTTP 404, `{ status: 404, title: "Resource Not Found", detail: "Task with id ... not found" }`. |
| **Frontend** | Task removed from cache via `removeQueries`. List and kanban views re-render without the deleted task. Activity feed refreshed. |
| **Status** | UNTESTED |

---

### E2E-TASK-09: Non-Admin Delete Rejected

| Field | Value |
|---|---|
| **ID** | E2E-TASK-09 |
| **Title** | User-role attempt to delete task returns 403 |
| **Traces to** | AC-P1-10, AC-P1-13 |
| **Pre-conditions** | Task exists. Authenticated user with role `User` (not `Admin`). |
| **Steps** | |

1. `DELETE /api/tasks/{id}` with User-role JWT.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 403 Forbidden. |
| **Frontend** | Interceptor dispatches `FORBIDDEN_EVENT`. Global toast listener shows forbidden message. Delete button should be hidden for non-Admin users (client-side role gating per AC-P1-13). |
| **Status** | UNTESTED |

---

### E2E-TASK-10: Task Dialog Save Persists Task

| Field | Value |
|---|---|
| **ID** | E2E-TASK-10 |
| **Title** | Creating a task from the UI submits a POST request and persists data |
| **Traces to** | AC-03, AC-04 |
| **Pre-conditions** | Authenticated user on `/tasks` page with create permission |
| **Steps** | |

1. Open task creation dialog.
2. Fill required fields and click save.
3. Observe network requests and refetched list.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/tasks` is called once and returns HTTP 201 with created task payload. |
| **Frontend** | Dialog closes after success. Success toast appears. New task appears in list and Kanban after query invalidation. |
| **Verification note** | Runtime path validated on 2026-04-17 using `e2e/task-dialog.runtime.spec.ts`, including create (`POST /api/tasks` => 201) and update (`PATCH /api/tasks/{id}` => 200) assertions. |
| **Status** | PASS |

---

### E2E-TASK-11: Unassigned Task Renders Across All Views

| Field | Value |
|---|---|
| **ID** | E2E-TASK-11 |
| **Title** | Tasks with `assignee: null` render safely in list, Kanban, and calendar views |
| **Traces to** | AC-01, AC-P1-05 |
| **Pre-conditions** | At least one task exists with `assignee: null` and a due date in the visible calendar range |
| **Steps** | |

1. Load `/tasks` list view.
2. Switch to Kanban view.
3. Switch to calendar view and open the task's date cell.

| **Expected Result** | |
|---|---|
| **Backend** | Task payload allows `assignee: null` without schema violations. |
| **Frontend** | All views render task cards without runtime exceptions. Assignee fallback UI is used consistently for unassigned tasks. |
| **Status** | UNTESTED |

---

### E2E-TASK-12: Full Task Lifecycle Invalidates Activity Query Keys

| Field | Value |
|---|---|
| **ID** | E2E-TASK-12 |
| **Title** | Create, update, progress, complete, and delete mutations invalidate `['activities']` query keys |
| **Traces to** | AC-P1-16, AC-03, AC-04, AC-05, AC-06, AC-07 |
| **Pre-conditions** | Authenticated admin user. Test harness subscribes to `['activities']` query key. |
| **Steps** | |

1. Register `['activities']` query and capture initial `dataUpdatedAt`.
2. `POST /api/tasks`.
3. `PATCH /api/tasks/{id}`.
4. `PATCH /api/tasks/{id}/progress`.
5. `PATCH /api/tasks/{id}/status` to `completed`.
6. `DELETE /api/tasks/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | Each endpoint returns expected lifecycle contract status and shape. |
| **Frontend** | `['activities']` query invalidates after each mutation and `dataUpdatedAt` advances. |
| **Status** | UNTESTED |

---

### E2E-TASK-13: Task Assignment Round-Trip

| Field | Value |
|---|---|
| **ID** | E2E-TASK-13 |
| **Title** | Assignee assignment and unassignment round-trip correctly |
| **Traces to** | AC-03, AC-04, AC-02 |
| **Pre-conditions** | Authenticated user and valid assignee user ID |
| **Steps** | |

1. Create task with `assigneeId`.
2. Fetch task and assert shaped `assignee` object.
3. Update task with `assigneeId: null`.
4. Fetch task and assert `assignee: null`.

| **Expected Result** | |
|---|---|
| **Backend** | Assignee object is present when assigned and null when cleared. |
| **Frontend** | Assignee avatar/initials render when assigned and fallback state renders when unassigned. |
| **Status** | UNTESTED |

---

### E2E-TASK-14: Task Tags Round-Trip

| Field | Value |
|---|---|
| **ID** | E2E-TASK-14 |
| **Title** | Tags persist across create, update, and clear operations |
| **Traces to** | AC-03, AC-04 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. Create task with initial tags.
2. Fetch and verify tags.
3. Update task with additional tags.
4. Fetch and verify updated tags.
5. Update task with empty tag array.
6. Fetch and verify tags cleared.

| **Expected Result** | |
|---|---|
| **Backend** | Returned tags reflect latest submitted array (order-insensitive). |
| **Frontend** | Tag chips in task UI match persisted values after each save. |
| **Status** | UNTESTED |

---

### E2E-TASK-15: Critical Priority Round-Trip

| Field | Value |
|---|---|
| **ID** | E2E-TASK-15 |
| **Title** | `critical` priority remains lossless across create and update boundaries |
| **Traces to** | AC-P1-03, AC-P1-04 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. Create task with `priority: critical`.
2. Fetch task and assert `priority === "critical"`.
3. Update to `priority: low`.
4. Fetch and assert `priority === "low"`.

| **Expected Result** | |
|---|---|
| **Backend** | Priority mapping remains symmetric and does not coerce `critical` to `high`. |
| **Frontend** | Critical styling and label render correctly. |
| **Status** | UNTESTED |

---

### E2E-TASK-16: Kanban Renders All Six Status Columns

| Field | Value |
|---|---|
| **ID** | E2E-TASK-16 |
| **Title** | Kanban view renders and routes tasks across all six statuses |
| **Traces to** | AC-P1-02, AC-P1-05 |
| **Pre-conditions** | At least one task exists per status |
| **Steps** | |

1. Open `/tasks` Kanban view.
2. Verify six status columns.
3. Verify cards appear in matching columns.

| **Expected Result** | |
|---|---|
| **Backend** | Task list includes records for all six statuses. |
| **Frontend** | Columns render for `todo`, `in-progress`, `blocked`, `on-hold`, `completed`, and `cancelled` with no dropped cards. |
| **Status** | UNTESTED |

---

### E2E-CAL-01: Calendar View Renders Expanded Status Tasks

| Field | Value |
|---|---|
| **ID** | E2E-CAL-01 |
| **Title** | Calendar view renders tasks regardless of status, including expanded status vocabulary |
| **Traces to** | AC-01, AC-P1-05 |
| **Pre-conditions** | Seeded tasks across all statuses with visible-month due dates |
| **Steps** | |

1. Open tasks calendar view for seeded month.
2. Verify each seeded task appears on expected day.

| **Expected Result** | |
|---|---|
| **Backend** | Task list endpoint returns seeded tasks with expected statuses. |
| **Frontend** | Calendar renders tasks across all statuses without silent suppression. |
| **Status** | UNTESTED |

---

## 3. Project and Team Management

### E2E-PROJ-01: Create Project (Admin)

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-01 |
| **Title** | Admin creates a project |
| **Traces to** | AC-13 |
| **Pre-conditions** | Authenticated user with role `Admin` |
| **Steps** | |

1. `POST /api/projects` with body:
   ```json
   {
     "name": "Q3 Feature Sprint",
     "description": "Sprint planning for Q3 features",
     "startDate": "2026-05-01T00:00:00Z",
     "endDate": "2026-07-31T00:00:00Z"
   }
   ```

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 201 with created project ID. Validation passes (name required, max 100; description max 500; endDate >= startDate). |
| **Frontend** | N/A — frontend does not currently have a project creation UI. Backend-only validation. |
| **Status** | UNTESTED |

---

### E2E-PROJ-02: Get Projects with Pagination

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-02 |
| **Title** | Project list returns paginated results |
| **Traces to** | — |
| **Pre-conditions** | Multiple projects exist |
| **Steps** | |

1. `GET /api/projects?pageNumber=1&pageSize=10`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ data: ProjectDto[], total: <n>, page: 1, limit: 10, totalPages: <n> }`. Each `ProjectDto` includes `id`, `name`, `description`, `status`, `startDate`, `endDate`. |
| **Frontend** | N/A — no project list page exists in frontend. Backend-only contract verification. |
| **Status** | UNTESTED |

---

### E2E-PROJ-03: Get Project Detail with Tasks

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-03 |
| **Title** | Project detail includes nested task list |
| **Traces to** | — |
| **Pre-conditions** | Project with tasks exists |
| **Steps** | |

1. `GET /api/projects/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `ProjectDetailDto` with `tasks` array containing task summaries. |
| **Frontend** | N/A. |
| **Status** | UNTESTED |

---

### E2E-PROJ-04: Anonymous Create Project Rejected

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-04 |
| **Title** | Unauthenticated request to create project returns 401 |
| **Traces to** | AC-13 |
| **Pre-conditions** | No auth header |
| **Steps** | |

1. `POST /api/projects` with body `{ "name": "test" }` and **no** `Authorization` header.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401 Unauthorized. No project created. |
| **Frontend** | N/A (API-level test). |
| **Status** | UNTESTED |

---

### E2E-PROJ-05: Non-Admin Create Project Rejected

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-05 |
| **Title** | User-role attempt to create project returns 403 |
| **Traces to** | AC-P1-10 |
| **Pre-conditions** | Authenticated user with role `User` |
| **Steps** | |

1. `POST /api/projects` with User-role JWT and valid body.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 403 Forbidden. |
| **Frontend** | N/A. |
| **Status** | UNTESTED |

---

### E2E-PROJ-06: Admin Project Update and Delete

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-06 |
| **Title** | Admin updates and deletes an existing project |
| **Traces to** | AC-13, AC-P1-10 |
| **Pre-conditions** | Admin token and existing project |
| **Steps** | |

1. `PUT /api/projects/{id}` with valid update body.
2. `GET /api/projects/{id}` and verify persistence.
3. `DELETE /api/projects/{id}`.
4. `GET /api/projects/{id}` and expect not found.

| **Expected Result** | |
|---|---|
| **Backend** | PUT returns 200, DELETE returns 204, follow-up GET returns 404 with strict error payload. |
| **Frontend** | N/A. |
| **Status** | UNTESTED |

---

### E2E-TEAM-01: Teams Controller Status

| Field | Value |
|---|---|
| **ID** | E2E-TEAM-01 |
| **Title** | Teams API endpoints are not yet implemented |
| **Traces to** | AC-15 |
| **Pre-conditions** | None |
| **Steps** | |

1. `GET /api/teams`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 404 — no `TeamsController` exists. |
| **Frontend** | Team page computes workload from `/api/users` and `/api/tasks` (does not call `/api/teams` directly). Team page should load without 404 errors per AC-15. Frontend should not issue requests to missing `/api/teams` endpoint during normal navigation. |
| **Known gap** | `TeamsController` and CQRS layer are not implemented. Domain entity `Team` exists but has no Application/API surface. |
| **Status** | FAIL (expected — known missing feature) |

---

### E2E-TEAM-02: Team Workload Data Source

| Field | Value |
|---|---|
| **ID** | E2E-TEAM-02 |
| **Title** | Team page derives workload from `/api/users` and `/api/tasks` and never calls `/api/teams` |
| **Traces to** | AC-15, AC-P1-01 |
| **Pre-conditions** | Authenticated session with seeded users and tasks |
| **Steps** | |

1. Navigate to `/team`.
2. Inspect requests for `/api/users`, `/api/tasks`, and `/api/teams*`.
3. Validate rendered workload rows.

| **Expected Result** | |
|---|---|
| **Backend** | Requests target users/tasks endpoints only; `/api/teams*` is never requested. |
| **Frontend** | Workload rows are computed from real API payloads and render without fallback mock dependency. |
| **Verification note** | Runtime contract validated on 2026-04-17 via `e2e/surface-contract.runtime.spec.ts` (`/team` requests `/api/users` and `/api/tasks`; no `/api/teams` requests). |
| **Status** | PASS |

---

## 4. User Management

### E2E-USER-01: Get Users with Pagination and Filtering

| Field | Value |
|---|---|
| **ID** | E2E-USER-01 |
| **Title** | User list returns paginated results with filter support |
| **Traces to** | AC-P1-01 |
| **Pre-conditions** | Multiple users exist. Authenticated request. |
| **Steps** | |

1. `GET /api/users?page=1&limit=10`.
2. `GET /api/users?search=test&page=1&limit=10`.
3. `GET /api/users?role=Admin&page=1&limit=10`.
4. `GET /api/users?department=Engineering&page=1&limit=10`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ data: UserDto[], total, page, limit, totalPages }`. Each `UserDto` includes `id`, `firstName`, `lastName`, `name`, `email`, `username`, `role`, `avatar`, `initials`, `department`, `status`. The `name` field contains `"{FirstName} {LastName}"` (alias for frontend consumption per AC-P1-01). |
| **Frontend** | Team page renders user cards with `user.name` displaying correctly (not `undefined`). Pagination controls work. Filter dropdowns trigger re-fetch. |
| **Status** | UNTESTED |

---

### E2E-USER-02: Get User by ID

| Field | Value |
|---|---|
| **ID** | E2E-USER-02 |
| **Title** | User detail includes name and profile fields |
| **Traces to** | AC-P1-01 |
| **Pre-conditions** | User with known ID exists |
| **Steps** | |

1. `GET /api/users/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `UserDetailDto` with `id`, `firstName`, `lastName`, `name`, `email`, `username`, `role`, `avatar`, `initials`, `department`, `bio`, `location`, `phone`, `status`. |
| **Frontend** | User detail view or profile modal renders all fields. |
| **Status** | UNTESTED |

---

### E2E-USER-03: Get Current User Profile

| Field | Value |
|---|---|
| **ID** | E2E-USER-03 |
| **Title** | Authenticated user retrieves own profile |
| **Traces to** | — |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `GET /api/users/profile`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Same shape as `UserDetailDto`. Returned user matches JWT `sub` claim. |
| **Frontend** | Profile page renders bio, location, phone, department, and avatar. |
| **Status** | UNTESTED |

---

### E2E-USER-04: Update Current User Profile

| Field | Value |
|---|---|
| **ID** | E2E-USER-04 |
| **Title** | User patches own profile fields |
| **Traces to** | AC-P1-01 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `PATCH /api/users/profile` with body:
   ```json
   {
     "bio": "Senior Engineer at TeamTasker",
     "location": "New York",
     "phone": "+1-555-0100"
   }
   ```
2. `GET /api/users/profile` to confirm.

| **Expected Result** | |
|---|---|
| **Backend** | PATCH: HTTP 200 with updated `UserDetailDto`. GET confirms persisted values. |
| **Frontend** | Profile page re-renders with updated bio, location, phone. Cache updated via `setQueryData` on `userKeys.profile()`. |
| **Status** | UNTESTED |

---

### E2E-USER-05: Admin User Management Endpoints

| Field | Value |
|---|---|
| **ID** | E2E-USER-05 |
| **Title** | Admin user create/update/delete endpoints status |
| **Traces to** | AC-P1-15 |
| **Pre-conditions** | None |
| **Steps** | |

1. Verify frontend `useCreateUser`, `useUpdateUser`, `useDeleteUser` hooks are removed per AC-P1-15.
2. `POST /api/users` — expect no endpoint.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/users`: HTTP 404 or 405 (no endpoint). User creation is via `/api/auth/register` only. |
| **Frontend** | Per AC-P1-15, unsupported user create/update/delete hooks should be removed from exported API surface. If still present, this is a known gap. |
| **Status** | UNTESTED |

---

### E2E-USER-06: Profile Skills Endpoint Status

| Field | Value |
|---|---|
| **ID** | E2E-USER-06 |
| **Title** | `PUT /api/users/profile/skills` endpoint is not implemented |
| **Traces to** | AC-P1-01 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `PUT /api/users/profile/skills` with valid skills payload.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 404 due to missing route in `UsersController`. |
| **Frontend** | Any mutation path should surface non-empty error details and avoid silent cache corruption. |
| **Status** | FAIL (expected — known missing feature) |

---

### E2E-USER-07: Development Goals CRUD Endpoint Status

| Field | Value |
|---|---|
| **ID** | E2E-USER-07 |
| **Title** | `/api/users/profile/development-goals` CRUD routes are not implemented |
| **Traces to** | AC-P1-01 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `GET /api/users/profile/development-goals`.
2. `POST /api/users/profile/development-goals`.
3. `PATCH /api/users/profile/development-goals/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | All requests return HTTP 404. |
| **Frontend** | Query/mutation failures are surfaced without runtime crashes. |
| **Status** | FAIL (expected — known missing feature) |

---

### E2E-DEV-01: Development Surfaces Degrade Gracefully

| Field | Value |
|---|---|
| **ID** | E2E-DEV-01 |
| **Title** | Development-related frontend surfaces degrade safely when profile subresource backends are unavailable |
| **Traces to** | AC-P1-01, AC-15 |
| **Pre-conditions** | Authenticated user and missing profile subresource endpoints |
| **Steps** | |

1. Navigate to development-related UI surfaces.
2. Observe UI behavior when profile subresource requests fail.

| **Expected Result** | |
|---|---|
| **Backend** | Profile subresource routes remain unimplemented and direct calls return HTTP 404. |
| **Frontend** | Development and dashboard surfaces remain interactive without mandatory runtime calls to missing profile subresource routes. |
| **Verification note** | Runtime contract validated on 2026-04-17 via `e2e/surface-contract.runtime.spec.ts` for `/development` and `/dashboard` navigation without profile subresource requests. |
| **Status** | PASS |

---

## 5. Authorization and Security

### E2E-SEC-01: Unauthenticated Access to Protected Routes (Middleware)

| Field | Value |
|---|---|
| **ID** | E2E-SEC-01 |
| **Title** | Middleware redirects unauthenticated users to /login |
| **Traces to** | AC-P1-14 |
| **Pre-conditions** | No auth token in cookies or localStorage |
| **Steps** | |

1. Navigate to `/dashboard` without authentication.
2. Navigate to `/tasks`.
3. Navigate to `/team`.
4. Navigate to `/profile`.

| **Expected Result** | |
|---|---|
| **Backend** | N/A (frontend middleware). |
| **Frontend** | Next.js `middleware.ts` checks for auth cookie. If absent, responds with 307 redirect to `/login`. No dashboard content flashes. Protected route prefixes: `/dashboard`, `/tasks`, `/team`, `/profile`, `/development`, `/communication`. |
| **Status** | UNTESTED |

---

### E2E-SEC-02: Authenticated User Redirected from Auth Pages

| Field | Value |
|---|---|
| **ID** | E2E-SEC-02 |
| **Title** | Authenticated user accessing /login is redirected to /dashboard |
| **Traces to** | AC-P1-14 |
| **Pre-conditions** | Valid auth cookie/token present |
| **Steps** | |

1. Navigate to `/login` while authenticated.
2. Navigate to `/register` while authenticated.

| **Expected Result** | |
|---|---|
| **Backend** | N/A. |
| **Frontend** | Middleware or auth context redirects to `/dashboard`. Login/register forms do not render. |
| **Status** | UNTESTED |

---

### E2E-SEC-03: RBAC on Task Endpoints

| Field | Value |
|---|---|
| **ID** | E2E-SEC-03 |
| **Title** | Role-based access control enforced on task write operations |
| **Traces to** | AC-P1-10, AC-P1-13 |
| **Pre-conditions** | Two users: one `Admin`, one `User` |
| **Steps** | |

1. As `User` role: `POST /api/tasks` — expect 200 (allowed).
2. As `User` role: `PATCH /api/tasks/{id}` — expect 200 (allowed).
3. As `User` role: `DELETE /api/tasks/{id}` — expect 403 (Admin only).
4. As `Admin` role: `DELETE /api/tasks/{id}` — expect 204 (allowed).

| **Expected Result** | |
|---|---|
| **Backend** | Steps 1-2: HTTP 200/201 (both `Admin` and `User` can create/update). Step 3: HTTP 403. Step 4: HTTP 204. |
| **Frontend** | Delete button hidden for `User` role (client-side gating). If `User` somehow triggers delete, 403 toast appears via `FORBIDDEN_EVENT` listener. |
| **Status** | UNTESTED |

---

### E2E-SEC-04: RBAC on Project Endpoints

| Field | Value |
|---|---|
| **ID** | E2E-SEC-04 |
| **Title** | Only Admin can create, update, and delete projects |
| **Traces to** | AC-13, AC-P1-10 |
| **Pre-conditions** | One `Admin` user, one `User` user, one project exists |
| **Steps** | |

1. As `User`: `POST /api/projects` — expect 403.
2. As `User`: `PUT /api/projects/{id}` — expect 403.
3. As `User`: `DELETE /api/projects/{id}` — expect 403.
4. As `Admin`: all three — expect success.
5. As anonymous (no token): `GET /api/projects` — expect 401.

| **Expected Result** | |
|---|---|
| **Backend** | User-role: 403 on all write ops. Admin: 201/200/204. Anonymous: 401 on all (class-level `[Authorize]`). |
| **Frontend** | N/A (no project UI). |
| **Status** | UNTESTED |

---

### E2E-SEC-05: 403 Forbidden Triggers Global Toast

| Field | Value |
|---|---|
| **ID** | E2E-SEC-05 |
| **Title** | Frontend forbidden event dispatches visible toast |
| **Traces to** | AC-P1-06, AC-P1-07 |
| **Pre-conditions** | User logged in with `User` role |
| **Steps** | |

1. Trigger a 403 response (e.g., attempt to delete a task as non-admin, if delete button is somehow accessible).

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 403. |
| **Frontend** | Axios interceptor dispatches `FORBIDDEN_EVENT` (custom event with `{ error, status: 403 }`). `ApiErrorListener` listens for `api:forbidden` and renders a destructive toast with permission-denied intent. Test asserts event dispatch and destructive variant, and only asserts toast copy contains either `forbidden` or `permission`. |
| **Status** | UNTESTED |

---

### E2E-SEC-06: JWT Token Tampering Rejected

| Field | Value |
|---|---|
| **ID** | E2E-SEC-06 |
| **Title** | Modified JWT is rejected by backend |
| **Traces to** | — |
| **Pre-conditions** | Valid JWT obtained |
| **Steps** | |

1. Modify one character in the JWT payload section.
2. `GET /api/auth/me` with the tampered token.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401 Unauthorized. Signature validation fails. |
| **Frontend** | Interceptor catches 401. Session cleared. Redirect to `/login`. |
| **Status** | UNTESTED |

---

### E2E-SEC-07: LocalStorage Token Cannot Bypass Middleware

| Field | Value |
|---|---|
| **ID** | E2E-SEC-07 |
| **Title** | Protected route access requires auth cookie, not only localStorage token |
| **Traces to** | AC-P1-14 |
| **Pre-conditions** | `auth_token` exists in localStorage, but auth cookie is absent |
| **Steps** | |

1. Set `auth_token` and `auth_refresh_token` in localStorage only.
2. Navigate to `/dashboard`.

| **Expected Result** | |
|---|---|
| **Backend** | N/A (request blocked by middleware redirect before protected API flow completes). |
| **Frontend** | Middleware redirects to `/login` with HTTP 307 because cookie is missing. Presence of localStorage token alone is insufficient for protected route access. |
| **Status** | UNTESTED |

---

### E2E-SEC-08: Hidden Delete Control Plus API-Level Denial

| Field | Value |
|---|---|
| **ID** | E2E-SEC-08 |
| **Title** | Non-admin cannot delete task even when invoking endpoint directly |
| **Traces to** | AC-P1-10, AC-P1-13 |
| **Pre-conditions** | Authenticated `User` role session and existing task |
| **Steps** | |

1. Verify delete action is not visible in task UI for `User` role.
2. Attempt direct `DELETE /api/tasks/{id}` using same User token.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 403 Forbidden for direct API delete attempt. |
| **Frontend** | UI remains defense-in-depth compliant: destructive control hidden for non-admin users. If forced request is made through frontend runtime, `FORBIDDEN_EVENT` handling executes. |
| **Status** | UNTESTED |

---

## 6. Error Handling

### E2E-ERR-01: Validation Error Response

| Field | Value |
|---|---|
| **ID** | E2E-ERR-01 |
| **Title** | Backend validation errors render in frontend toast |
| **Traces to** | AC-09 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `POST /api/tasks` with empty body `{}` (missing required `title`).

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400. Body: `{ status: 400, title: "Validation Error", detail: "One or more validation errors occurred.", errors: { "Title": ["Title is required."] } }`. |
| **Frontend** | `parseApiError()` reads `detail` field (falls through `message ?? detail ?? title`). Toast renders: "One or more validation errors occurred." Field-level `errors` available for form rendering. |
| **Status** | UNTESTED |

---

### E2E-ERR-02: Not Found Error Response

| Field | Value |
|---|---|
| **ID** | E2E-ERR-02 |
| **Title** | 404 for non-existent resource renders meaningful message |
| **Traces to** | AC-02, AC-09 |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `GET /api/tasks/99999`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 404. Body: `{ status: 404, title: "Resource Not Found", detail: "Task with id 99999 not found" }`. |
| **Frontend** | `parseApiError()` reads `detail`. Toast or inline error renders: "Task with id 99999 not found". |
| **Status** | UNTESTED |

---

### E2E-ERR-03: Server Error Response

| Field | Value |
|---|---|
| **ID** | E2E-ERR-03 |
| **Title** | 500 server error triggers error event and toast |
| **Traces to** | AC-09 |
| **Pre-conditions** | Authenticated user. Scenario that triggers 500 (e.g., database unavailable). |
| **Steps** | |

1. Trigger a server error (stop database, then call `GET /api/tasks`).

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 500. Body: `{ status: 500, title: "Server Error", detail: "<exception message in dev, generic in prod>" }`. |
| **Frontend** | Interceptor dispatches `SERVER_ERROR_EVENT`. `ApiErrorListener` renders toast: "A server error occurred (500). Please try again later." |
| **Status** | UNTESTED |

---

### E2E-ERR-04: Network Error Handling

| Field | Value |
|---|---|
| **ID** | E2E-ERR-04 |
| **Title** | Network failure triggers network error toast |
| **Traces to** | — |
| **Pre-conditions** | Backend server is stopped |
| **Steps** | |

1. Stop the backend server.
2. Perform any action in the frontend that triggers an API call.

| **Expected Result** | |
|---|---|
| **Backend** | N/A (unreachable). |
| **Frontend** | Axios throws without `error.response`. Interceptor dispatches `NETWORK_ERROR_EVENT`. Toast: "Unable to connect to the server. Please check your connection." |
| **Status** | UNTESTED |

---

### E2E-ERR-05: Error Boundary Catches Render Errors

| Field | Value |
|---|---|
| **ID** | E2E-ERR-05 |
| **Title** | React ErrorBoundary catches component render failures |
| **Traces to** | — |
| **Pre-conditions** | Application loaded |
| **Steps** | |

1. Introduce a render error in a dashboard component (or simulate via test).

| **Expected Result** | |
|---|---|
| **Backend** | N/A. |
| **Frontend** | `ErrorBoundary` catches the error. Fallback UI renders: red alert icon, "Something went wrong" message, error details in `<details>` (dev mode), "Try Again" and "Go to Home" buttons. Application does not white-screen. |
| **Status** | UNTESTED |

---

## 7. Edge Cases

### E2E-EDGE-01: Empty Paginated Results

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-01 |
| **Title** | Empty task list returns valid paginated shape with zero items |
| **Traces to** | testing_strategy.md: Edge Case Coverage |
| **Pre-conditions** | No tasks exist, or filter matches zero tasks |
| **Steps** | |

1. `GET /api/tasks?status=cancelled&page=1&limit=10` (assuming no cancelled tasks).

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 200. Body: `{ data: [], total: 0, page: 1, limit: 10, totalPages: 0 }`. |
| **Frontend** | List view renders empty state (e.g., "No tasks found"). Kanban columns render empty. No JavaScript errors. Pagination controls hidden or show "0 results". |
| **Status** | UNTESTED |

---

### E2E-EDGE-02: Invalid Task ID (Non-Numeric)

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-02 |
| **Title** | Non-numeric task ID returns 400 or 404 |
| **Traces to** | testing_strategy.md: Edge Case Coverage |
| **Pre-conditions** | Authenticated user |
| **Steps** | |

1. `GET /api/tasks/abc`.
2. `PATCH /api/tasks/abc/status` with `{ "status": "completed" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 404 (route `/api/tasks/{id:int}` does not match non-numeric values). |
| **Frontend** | Frontend uses `string` type for IDs but backend expects `int`. If frontend constructs a URL with a non-numeric ID, backend rejects. Frontend should handle gracefully. |
| **Status** | UNTESTED |

---

### E2E-EDGE-03: Invalid Task ID (Non-Existent Numeric)

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-03 |
| **Title** | Non-existent numeric task ID returns 404 |
| **Traces to** | AC-02, testing_strategy.md: Edge Case Coverage |
| **Pre-conditions** | Task ID 99999 does not exist |
| **Steps** | |

1. `GET /api/tasks/99999`.
2. `PATCH /api/tasks/99999/status` with `{ "status": "completed" }`.
3. `DELETE /api/tasks/99999`.

| **Expected Result** | |
|---|---|
| **Backend** | All: HTTP 404. Body: `{ status: 404, title: "Resource Not Found", detail: "Task with id 99999 not found" }`. |
| **Frontend** | Error toast with detail message. No crash. |
| **Status** | UNTESTED |

---

### E2E-EDGE-04: Expired Refresh Token

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-04 |
| **Title** | Expired refresh token is rejected and forces re-login |
| **Traces to** | AC-11, testing_strategy.md: Edge Case Coverage |
| **Pre-conditions** | Refresh token that has exceeded its TTL |
| **Steps** | |

1. `POST /api/auth/refresh-token` with `{ "refreshToken": "<expired-token>" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401. Body: `{ status: 401, title: "Unauthorized", detail: "Invalid or expired refresh token" }`. No new tokens issued. |
| **Frontend** | Token refresh fails. `UNAUTHORIZED_EVENT` dispatched. Toast: "Your session has expired. Please log in again." Redirect to `/login`. All tokens cleared from localStorage. |
| **Status** | UNTESTED |

---

### E2E-EDGE-05: Invalid Refresh Token (Garbage String)

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-05 |
| **Title** | Garbage refresh token is rejected |
| **Traces to** | AC-11 |
| **Pre-conditions** | None |
| **Steps** | |

1. `POST /api/auth/refresh-token` with `{ "refreshToken": "not-a-real-token" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 401. Same shape as E2E-EDGE-04. |
| **Frontend** | Same behavior as E2E-EDGE-04. |
| **Status** | UNTESTED |

---

### E2E-EDGE-06: Task Progress Boundary Values

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-06 |
| **Title** | Out-of-range progress values are rejected |
| **Traces to** | AC-06 |
| **Pre-conditions** | Task exists |
| **Steps** | |

1. `PATCH /api/tasks/{id}/progress` with `{ "progress": -10 }`.
2. `PATCH /api/tasks/{id}/progress` with `{ "progress": 150 }`.
3. `PATCH /api/tasks/{id}/progress` with `{ "progress": 0 }`.
4. `PATCH /api/tasks/{id}/progress` with `{ "progress": 100 }`.

| **Expected Result** | |
|---|---|
| **Backend** | Steps 1-2: HTTP 400 with validation error shape `{ status, title, detail, errors }` because command validation enforces `0 <= progress <= 100`. Steps 3-4: HTTP 200 with auto-status-update. |
| **Frontend** | Progress bar renders 0% or 100%. No negative or over-100 rendering. |
| **Status** | UNTESTED |

---

### E2E-EDGE-07: Invalid Status String

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-07 |
| **Title** | Unrecognized status string returns 400 |
| **Traces to** | AC-05 |
| **Pre-conditions** | Task exists |
| **Steps** | |

1. `PATCH /api/tasks/{id}/status` with `{ "status": "invalid-status" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400. Body: `{ status: 400, title: "Bad Request", detail: "Invalid status value: invalid-status" }`. `TryParseStatus` returns false. |
| **Frontend** | Error toast with detail message. Status dropdown does not include invalid values. |
| **Status** | UNTESTED |

---

### E2E-EDGE-08: Duplicate User Registration

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-08 |
| **Title** | Registering with existing email or username returns conflict |
| **Traces to** | — |
| **Pre-conditions** | User with email `test-reg@example.com` already exists |
| **Steps** | |

1. `POST /api/auth/register` with same email.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 409 Conflict. Body: `{ status: 409, title: "Conflict", detail: "User with this email already exists" }`. |
| **Frontend** | Error toast with detail message. Registration form remains visible. |
| **Status** | UNTESTED |

---

### E2E-EDGE-09: Project Validation Failures

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-09 |
| **Title** | Invalid project data returns field-level validation errors |
| **Traces to** | — |
| **Pre-conditions** | Authenticated Admin user |
| **Steps** | |

1. `POST /api/projects` with body `{ "name": "", "startDate": "2026-05-01", "endDate": "2026-04-01" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400. Body: `{ status: 400, title: "Validation Error", detail: "...", errors: { "Name": ["Name is required."], "EndDate": ["End date must be after start date."] } }`. |
| **Frontend** | N/A (no project form in UI). |
| **Status** | UNTESTED |

---

### E2E-EDGE-10: Communication and Activity Endpoints

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-10 |
| **Title** | Frontend navigation does not issue requests to missing endpoints |
| **Traces to** | AC-15 |
| **Pre-conditions** | User authenticated |
| **Steps** | |

1. Navigate to `/communication` page.
2. Navigate to `/dashboard` and observe network tab.
3. Check if any requests go to `/api/communication/*` or `/api/activities`.

| **Expected Result** | |
|---|---|
| **Backend** | No `CommunicationController` or `ActivitiesController` exists. |
| **Frontend** | Per AC-15, communication and activity UI surfaces should no longer issue requests to missing backend endpoints in normal navigation. If they still do, this is a known gap requiring stubbing or conditional rendering. |
| **Known gap** | These controllers are not implemented. Frontend may still issue requests. |
| **Status** | UNTESTED |

---

### E2E-EDGE-11: Missing Profile Subresource Endpoints

| Field | Value |
|---|---|
| **ID** | E2E-EDGE-11 |
| **Title** | Frontend handles missing profile subresource endpoints without fatal UI failure |
| **Traces to** | AC-P1-01 |
| **Pre-conditions** | Authenticated user and profile subresource endpoints absent in backend |
| **Steps** | |

1. Directly call profile subresource endpoints (`/api/users/profile/skills`, `/api/users/profile/development-goals`) and capture response status.
2. Navigate to profile, dashboard, and development surfaces.
3. Confirm the UI remains usable and does not require missing subresource endpoints during normal navigation.

| **Expected Result** | |
|---|---|
| **Backend** | Endpoints such as `/api/users/profile/skills` and `/api/users/profile/development-goals` return HTTP 404 because they are not implemented. |
| **Frontend** | Pages degrade gracefully with non-blocking fallbacks and visible error handling. No crash or blank screen occurs. |
| **Known gap** | Backend profile subresource endpoints remain outside `UsersController` contract surface. |
| **Verification note** | Runtime contract validated on 2026-04-17 via `e2e/surface-contract.runtime.spec.ts`, including direct endpoint 404 checks and stable `/profile` UI navigation without mandatory subresource calls. |
| **Status** | PASS |

---

### E2E-CFG-01: API Base URL and Timeout Normalization Smoke

| Field | Value |
|---|---|
| **ID** | E2E-CFG-01 |
| **Title** | Frontend API calls use normalized base URL and 15s timeout |
| **Traces to** | AC-P1-08, AC-P1-09 |
| **Pre-conditions** | Frontend running with default environment |
| **Steps** | |

1. Instrument API client and trigger auth, users, tasks, and projects requests.
2. Capture `baseURL` and `timeout` for each request.

| **Expected Result** | |
|---|---|
| **Backend** | N/A |
| **Frontend** | Every request uses `API_CONFIG.baseURL` and `timeout === 15000`; no request targets legacy `:3002` Mockoon path. |
| **Status** | UNTESTED |

---

## Traceability Matrix

| Test Case | AC Trace | Category | Risk Level |
|---|---|---|---|
| E2E-AUTH-01 | AC-10, AC-P1-12 | Auth | Critical |
| E2E-AUTH-02 | AC-10 | Auth | Critical |
| E2E-AUTH-03 | AC-10 | Auth | High |
| E2E-AUTH-04 | AC-09 | Auth | High |
| E2E-AUTH-05 | AC-P1-01, AC-P1-12 | Auth | High |
| E2E-AUTH-06 | AC-10, AC-11 | Auth | Critical |
| E2E-AUTH-07 | AC-11 | Auth | Critical |
| E2E-AUTH-08 | AC-14 | Auth | Critical |
| E2E-AUTH-09 | — | Auth | Medium |
| E2E-AUTH-10 | — | Auth | Medium |
| E2E-AUTH-11 | AC-09 | Auth | Medium |
| E2E-AUTH-12 | AC-11, AC-14 | Auth | Critical |
| E2E-AUTH-13 | AC-12, AC-P1-17 | Auth | Critical |
| E2E-AUTH-14 | AC-09 | Auth | Medium |
| E2E-AUTH-15 | AC-09 | Auth | Medium |
| E2E-TASK-01 | AC-03 | Task | Critical |
| E2E-TASK-02 | AC-01, AC-08 | Task | Critical |
| E2E-TASK-03 | AC-01 | Task | High |
| E2E-TASK-04 | AC-02 | Task | High |
| E2E-TASK-05 | AC-04 | Task | High |
| E2E-TASK-06 | AC-05, AC-P1-02, AC-P1-05 | Task | Critical |
| E2E-TASK-07 | AC-06 | Task | High |
| E2E-TASK-08 | AC-07 | Task | High |
| E2E-TASK-09 | AC-P1-10, AC-P1-13 | Task | High |
| E2E-TASK-10 | AC-03, AC-04 | Task | Critical |
| E2E-TASK-11 | AC-01, AC-P1-05 | Task | High |
| E2E-TASK-12 | AC-P1-16, AC-03, AC-04, AC-05, AC-06, AC-07 | Task | High |
| E2E-TASK-13 | AC-03, AC-04, AC-02 | Task | High |
| E2E-TASK-14 | AC-03, AC-04 | Task | Medium |
| E2E-TASK-15 | AC-P1-03, AC-P1-04 | Task | High |
| E2E-TASK-16 | AC-P1-02, AC-P1-05 | Task | High |
| E2E-CAL-01 | AC-01, AC-P1-05 | Task | Medium |
| E2E-PROJ-01 | AC-13 | Project | Medium |
| E2E-PROJ-02 | — | Project | Medium |
| E2E-PROJ-03 | — | Project | Low |
| E2E-PROJ-04 | AC-13 | Security | High |
| E2E-PROJ-05 | AC-P1-10 | Security | High |
| E2E-PROJ-06 | AC-13, AC-P1-10 | Security | High |
| E2E-TEAM-01 | AC-15 | Team | Medium |
| E2E-TEAM-02 | AC-15, AC-P1-01 | Team | Medium |
| E2E-USER-01 | AC-P1-01 | User | High |
| E2E-USER-02 | AC-P1-01 | User | Medium |
| E2E-USER-03 | — | User | Medium |
| E2E-USER-04 | AC-P1-01 | User | Medium |
| E2E-USER-05 | AC-P1-15 | User | Low |
| E2E-USER-06 | AC-P1-01 | User | Medium |
| E2E-USER-07 | AC-P1-01 | User | Medium |
| E2E-DEV-01 | AC-P1-01, AC-15 | User | Medium |
| E2E-SEC-01 | AC-P1-14 | Security | Critical |
| E2E-SEC-02 | AC-P1-14 | Security | Medium |
| E2E-SEC-03 | AC-P1-10, AC-P1-13 | Security | Critical |
| E2E-SEC-04 | AC-13, AC-P1-10 | Security | High |
| E2E-SEC-05 | AC-P1-06, AC-P1-07 | Security | High |
| E2E-SEC-06 | — | Security | Medium |
| E2E-SEC-07 | AC-P1-14 | Security | High |
| E2E-SEC-08 | AC-P1-10, AC-P1-13 | Security | High |
| E2E-ERR-01 | AC-09 | Error | High |
| E2E-ERR-02 | AC-02, AC-09 | Error | High |
| E2E-ERR-03 | AC-09 | Error | Medium |
| E2E-ERR-04 | — | Error | Medium |
| E2E-ERR-05 | — | Error | Low |
| E2E-EDGE-01 | testing_strategy | Edge | High |
| E2E-EDGE-02 | testing_strategy | Edge | Medium |
| E2E-EDGE-03 | AC-02, testing_strategy | Edge | High |
| E2E-EDGE-04 | AC-11, testing_strategy | Edge | Critical |
| E2E-EDGE-05 | AC-11 | Edge | High |
| E2E-EDGE-06 | AC-06 | Edge | High |
| E2E-EDGE-07 | AC-05 | Edge | Medium |
| E2E-EDGE-08 | — | Edge | Medium |
| E2E-EDGE-09 | — | Edge | Low |
| E2E-EDGE-10 | AC-15 | Edge | Medium |
| E2E-EDGE-11 | AC-P1-01 | Edge | Medium |
| E2E-CFG-01 | AC-P1-08, AC-P1-09 | Edge | Medium |

## Known Gaps Summary

The following items are expected to fail or require attention during test execution:

| Area | Gap | Impact |
|---|---|---|
| Teams | No `TeamsController` or CQRS layer. Domain entity exists. | E2E-TEAM-01 will 404. Frontend team page must not call `/api/teams`. |
| Activities and announcements | No `ActivitiesController` or `CommunicationController`. | E2E-EDGE-10 must verify frontend does not issue calls to missing endpoints during normal navigation. |
| Profile subresources | `UsersController` does not implement profile skills and development goals endpoints. | E2E-USER-06 and E2E-USER-07 remain expected endpoint-level failures until routes are implemented. E2E-EDGE-11 and E2E-DEV-01 are runtime-validated PASS checks for graceful UI behavior with callers removed. |
| User Admin CRUD | `UsersController` is read-only + profile patch. No admin user create/update/delete. | E2E-USER-05: frontend hooks should be removed per AC-P1-15. |
| Team workload data path | Team workload view is API-backed and should avoid `/api/teams`; runtime evidence should be maintained. | E2E-TEAM-02 is runtime-validated PASS for users/tasks-based rendering and no `/api/teams` requests. |
| Legacy test failures | 2 pre-existing unit test failures in `UserTests` and `CreateProjectCommandTests`. | Not related to P0/P1 changes but should be triaged. |
| Playwright environment | E2E execution requires backend and frontend servers plus seeded users or dynamic registration strategy. | Configure runtime variables and test data before CI gating. |

## Execution Priority

Run tests in this order to maximize early signal:

1. **Critical auth flow**: E2E-AUTH-01, E2E-AUTH-02, E2E-AUTH-06, E2E-AUTH-07, E2E-AUTH-12
2. **Critical task CRUD and UI integration**: E2E-TASK-01, E2E-TASK-02, E2E-TASK-06, E2E-TASK-10, E2E-TASK-12, E2E-TASK-16
3. **Security gates**: E2E-SEC-01, E2E-SEC-03, E2E-SEC-07, E2E-PROJ-04
4. **Error handling**: E2E-ERR-01, E2E-ERR-02, E2E-AUTH-04, E2E-AUTH-14, E2E-AUTH-15
5. **Edge and config checks**: E2E-EDGE-01, E2E-EDGE-04, E2E-EDGE-06, E2E-EDGE-11, E2E-CFG-01
6. **Remaining tests**: all others

## Playwright Execution Strategy

Use Playwright as the primary E2E runner for strict contract enforcement.

* API contract suites run with Playwright `request` fixture for deterministic status and payload assertions
* Browser suites run with Playwright `page` fixture for middleware redirects, role-gated UI behavior, and toast/event behavior
* Contract tests fail on any status-code or schema mismatch, even when frontend fallback logic can render an error message

Recommended command order:

1. `npm run test:e2e -- e2e/auth.contract.spec.ts`
2. `npm run test:e2e -- e2e/tasks.contract.spec.ts`
3. `npm run test:e2e -- e2e/security.middleware.spec.ts`
4. `npm run test:e2e`

Runtime variables for local and CI execution:

* `E2E_API_BASE_URL` (default `http://localhost:5220/api`)
* `E2E_WEB_BASE_URL` (default `http://localhost:3000`)
* `E2E_ADMIN_USERNAME` and `E2E_ADMIN_PASSWORD` for admin-only RBAC cases
