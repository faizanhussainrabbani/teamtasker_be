---
title: End-to-End Test Cases Audit
description: Coverage, gap, and broken-flow audit of the E2E test suite against P0 and P1 acceptance criteria and current application state.
author: TeamTasker Engineering
ms.date: 2026-04-17
ms.topic: reference
keywords:
  - e2e audit
  - coverage analysis
  - gap analysis
  - broken flows
  - acceptance criteria traceability
standards_references:
  - docs/project/standards/engineering_standards.md
  - docs/project/standards/FAANG_Documentation_Standards.md
  - docs/testing/testing_strategy.md
  - docs/testing/e2e_test_cases.md
  - docs/project/execution/p0_core_user_journeys_acceptance_criteria.md
  - docs/project/execution/p1_correctness_silent_ux_bugs_acceptance_criteria.md
  - docs/project/execution/p0_core_user_journeys_run_validation_report.md
  - docs/project/execution/p1_correctness_silent_ux_bugs_run_validation_report.md
estimated_reading_time: 30
---

## Purpose

This audit evaluates the E2E suite defined in [e2e_test_cases.md](./e2e_test_cases.md) against:

1. Acceptance criteria from [p0_core_user_journeys_acceptance_criteria.md](../project/execution/p0_core_user_journeys_acceptance_criteria.md) (AC-01 … AC-18) and [p1_correctness_silent_ux_bugs_acceptance_criteria.md](../project/execution/p1_correctness_silent_ux_bugs_acceptance_criteria.md) (AC-P1-01 … AC-P1-20).
2. Implemented backend controller and frontend component surfaces confirmed on 2026-04-17.
3. "Known Gaps Summary" in `e2e_test_cases.md` and the "Partial fail" rows in the P0/P1 run-validation reports.

The audit produces four deliverables:

* A coverage matrix and coverage percentage.
* A feature-gap catalogue listing implemented journeys with no direct E2E case.
* A broken-flow inventory enumerating features that are known to fail or to degrade silently under strict contract enforcement.
* A draft of new E2E test cases (E2E-*-NEW) suitable for direct insertion into the master suite.

## Methodology

* **AC classification.** ACs are split into two groups:
  * *E2E-relevant.* Runtime behavior that can be exercised through API and/or browser automation.
  * *Process/gate-only.* Build, lint, targeted-test execution, and documentation artifacts (AC-16, AC-17, AC-18, AC-P1-18, AC-P1-19, AC-P1-20). These are not counted against the E2E coverage denominator because they are validated by CI or document review, not by E2E execution.
* **Direct vs. indirect coverage.** "Direct" coverage means at least one E2E case's `Expected Result` exercises the AC's primary observable behavior. "Indirect" means the AC is only a side-assertion or depends on inference.
* **State verification.** The audit re-checked the current codebase to confirm controller presence, frontend hook wiring, and the behavior of subresource routes (`/api/users/profile/skills`, `/api/users/profile/development-goals`). Findings are cited with `file:line` references.

## 1. Coverage Analysis

### 1.1 Coverage against P0 acceptance criteria

| AC | Criterion (summary) | E2E cases that cover it | Coverage |
|---|---|---|---|
| AC-01 | Paginated `GET /api/tasks` contract | E2E-TASK-02, E2E-TASK-03, E2E-TASK-11, E2E-EDGE-01 | Direct |
| AC-02 | `GET /api/tasks/{id}` 200 and 404 behavior | E2E-TASK-04, E2E-EDGE-03, E2E-ERR-02 | Direct |
| AC-03 | `POST /api/tasks` create-then-read flow | E2E-TASK-01, E2E-TASK-10 | Direct |
| AC-04 | `PATCH /api/tasks/{id}` field updates | E2E-TASK-05, E2E-TASK-10 | Direct |
| AC-05 | Frontend status vocabulary persisted | E2E-TASK-06, E2E-EDGE-07 | Direct |
| AC-06 | Progress clamping and persistence | E2E-TASK-07, E2E-EDGE-06 | Direct |
| AC-07 | Delete then not-found | E2E-TASK-08 | Direct |
| AC-08 | FE list/kanban load without 404 | E2E-TASK-02 (browser assertions), E2E-TASK-11 | Direct |
| AC-09 | Non-empty error toasts | E2E-AUTH-04, E2E-AUTH-11, E2E-ERR-01, E2E-ERR-02, E2E-ERR-03 | Direct |
| AC-10 | Refresh-token issuance and rotation | E2E-AUTH-01, E2E-AUTH-02, E2E-AUTH-03, E2E-AUTH-06 | Direct |
| AC-11 | Invalid/expired refresh rejection | E2E-AUTH-06, E2E-AUTH-07, E2E-AUTH-12, E2E-EDGE-04, E2E-EDGE-05 | Direct |
| AC-12 | Pre-expiry proactive refresh keeps session active | E2E-AUTH-06 (step 3 only) | **Indirect — gap** |
| AC-13 | Anonymous project writes rejected | E2E-PROJ-04, E2E-SEC-04 | Direct |
| AC-14 | Logout clears React Query cache | E2E-AUTH-08, E2E-AUTH-12 | Direct |
| AC-15 | No calls to missing comms/activity endpoints in normal nav | E2E-TEAM-01, E2E-EDGE-10 | Direct |
| AC-16 | Backend unit/controller tests | Out of scope for E2E | Process gate |
| AC-17 | Frontend build and lint | Out of scope for E2E | Process gate |
| AC-18 | Documentation artifacts | Out of scope for E2E | Process gate |

P0 denominator (E2E-relevant ACs): **15** (AC-01 through AC-15).
P0 direct coverage: **14/15 = 93.3%**.
P0 indirect-only: 1 (AC-12). Proposed fix: new case **E2E-AUTH-13** below.

### 1.2 Coverage against P1 acceptance criteria

| AC | Criterion (summary) | E2E cases that cover it | Coverage |
|---|---|---|---|
| AC-P1-01 | Users payload includes `name` | E2E-AUTH-05, E2E-USER-01, E2E-USER-02, E2E-USER-04, E2E-EDGE-11 | Direct |
| AC-P1-02 | Status mapping includes blocked/on-hold/cancelled | E2E-TASK-06 | Direct |
| AC-P1-03 | Priority mapping includes `critical` | E2E-TASK-05 (partial — sets `critical` on update but does not assert the mapper boundary) | **Indirect — gap** |
| AC-P1-04 | FE type unions accept all BE transport values | — (static type check, arguably out of scope) | **Gap** |
| AC-P1-05 | All statuses render in list/kanban without dropping | E2E-TASK-06, E2E-TASK-11 | Direct |
| AC-P1-06 | API client dispatches `FORBIDDEN_EVENT` on 403 | E2E-SEC-05 | Direct |
| AC-P1-07 | Global listener shows forbidden toast | E2E-SEC-05 | Direct |
| AC-P1-08 | Base URL and timeout normalization | — | **Gap (config-level, testable via smoke)** |
| AC-P1-09 | Stale mock port 3002 and proxy.js removed | — | **Gap (static check, but observable)** |
| AC-P1-10 | Role-aware authorize on writes | E2E-TASK-09, E2E-PROJ-05, E2E-SEC-03, E2E-SEC-04, E2E-SEC-08 | Direct |
| AC-P1-11 | JWT `RoleClaimType` binding | E2E-SEC-03, E2E-TASK-09 (observed indirectly through role-gated 403/204) | **Indirect — acceptable** |
| AC-P1-12 | Auth response includes `role` | E2E-AUTH-01, E2E-AUTH-05 | Direct |
| AC-P1-13 | Admin-only FE actions gated by role | E2E-TASK-09, E2E-SEC-03, E2E-SEC-08 | Direct |
| AC-P1-14 | Middleware route protection | E2E-SEC-01, E2E-SEC-02, E2E-SEC-07 | Direct |
| AC-P1-15 | Unsupported user CRUD hooks removed | E2E-USER-05 | Direct |
| AC-P1-16 | Task mutations invalidate activity query keys | E2E-TASK-05 (side-note), E2E-TASK-08 (side-note) — not asserted as a primary expectation | **Indirect — gap** |
| AC-P1-17 | `expiresAt` end-to-end | E2E-AUTH-01, E2E-AUTH-06 | Direct |
| AC-P1-18 | Targeted mapper tests pass | Out of scope for E2E | Process gate |
| AC-P1-19 | Backend build succeeds | Out of scope for E2E | Process gate |
| AC-P1-20 | Standards-aligned documents | Out of scope for E2E | Process gate |

P1 denominator (E2E-relevant ACs): **17** (AC-P1-01 through AC-P1-17).
P1 direct coverage: **11/17 ≈ 64.7%**.
P1 direct + indirect coverage: **15/17 ≈ 88.2%**.
P1 fully uncovered: AC-P1-04 (static), AC-P1-08 and AC-P1-09 (config hygiene — observable through smoke).

### 1.3 Combined coverage summary

| Scope | Denominator | Direct | Direct + Indirect | Direct % | Combined % |
|---|---|---|---|---|---|
| P0 (E2E-relevant) | 15 | 14 | 15 | 93.3% | 100.0% |
| P1 (E2E-relevant) | 17 | 11 | 15 | 64.7% | 88.2% |
| **Combined** | **32** | **25** | **30** | **78.1%** | **93.8%** |

Overall: **78.1% of E2E-relevant acceptance criteria have direct E2E coverage**, rising to **93.8% when partial/indirect coverage is included**. The remaining 2 criteria (AC-P1-04, AC-P1-08 or AC-P1-09) either are static/type-level (AC-P1-04) or live at the configuration layer (AC-P1-08, AC-P1-09) where a single runtime smoke test would suffice. New cases in §4 close all remaining P0/P1 gaps except AC-P1-04.

## 2. Feature Gap Review

The existing suite was cross-checked against implemented code paths. The following user journeys or features are implemented (or partially implemented) but are **not directly represented by any E2E case**.

| # | User journey / feature | Implementation evidence | Why uncovered |
|---|---|---|---|
| 1 | Full task lifecycle with activity-feed invalidation (create → update → progress → complete → delete) exercised as one scenario, asserting the four `['activities']` query invalidations per [useTasks.ts](../../../teamtasker_fe/lib/api/hooks/useTasks.ts) | Frontend mutation hooks invalidate activity keys | Existing task cases are single-step; no multi-step lifecycle case asserts cache invalidation across the flow. Closes AC-P1-16 gap. |
| 2 | Pre-expiry proactive refresh when token has <5 min left | `lib/api/token-refresh.ts` interceptor check | AC-12 only observed as a side-effect in E2E-AUTH-06. |
| 3 | Task assignment via `assigneeId` (user picker in Task Dialog) | [task-dialog.tsx:136](../../../teamtasker_fe/components/tasks/task-dialog.tsx#L136) passes `assigneeId`; `TasksController.Update` persists `AssigneeId` | No case asserts the resulting `assignee` DTO shape (`{id, name, avatar, initials}`). |
| 4 | Task tag add/remove round-trip | [task-dialog.tsx:137](../../../teamtasker_fe/components/tasks/task-dialog.tsx#L137); domain `TaskTag` entity | No case creates a task with tags and re-reads to validate persistence. |
| 5 | Critical-priority round-trip asserting mapper symmetry | `TaskContractMapper` maps `Critical ↔ critical` | E2E-TASK-05 sends `critical` but does not read back and assert the client string. Partial AC-P1-03 gap. |
| 6 | All six statuses rendered simultaneously in kanban (including `blocked`, `on-hold`, `cancelled` columns) | P1 expanded columns per AC-P1-05 | E2E-TASK-06 exercises transitions but not simultaneous column rendering. |
| 7 | Development and dashboard navigation while profile subresource routes are absent | [development-view.tsx](../../../teamtasker_fe/components/development/development-view.tsx) and [development-goals-card.tsx](../../../teamtasker_fe/components/dashboard/development-goals-card.tsx) run without mandatory profile subresource calls | Previously uncovered; now represented by E2E-DEV-01 and runtime-validated in `e2e/surface-contract.runtime.spec.ts`. |
| 8 | Skills update flow (`PUT /api/users/profile/skills`) | `UsersController` still has no `profile/skills` route | Endpoint-level missing backend contract, represented by E2E-USER-06. |
| 9 | `GET /api/users/{id}` returning nested `skills` when user has any | `UsersController.GetUserById` joins `UserSkills` | E2E-USER-02 asserts base fields but not the skills array. |
| 10 | Team workload computed from `/api/users` and `/api/tasks` | [team-workload-view.tsx](../../../teamtasker_fe/components/team/team-workload-view.tsx) now uses `useUsers` and `useTasks` hooks | Previously uncovered; now represented by E2E-TEAM-02 and runtime-validated in `e2e/surface-contract.runtime.spec.ts`. |
| 11 | `PUT /api/projects/{id}` and `DELETE /api/projects/{id}` for admins | `ProjectsController` exposes both | No E2E case exercises success paths; only 403 paths are covered. |
| 12 | Change-password wrong-current-password rejection | `AuthController.ChangePassword` validates current password | E2E-AUTH-09 only covers the happy path. |
| 13 | Password-reset with invalid/expired token | `AuthController.ResetPassword` | E2E-AUTH-10 only covers happy path. |
| 14 | `AuthResponse.expiresAt` used by FE for proactive refresh (AC-P1-17) | `auth-context.tsx` stores `auth_token_expiry` | Partially covered; no explicit boundary test. |
| 15 | Global `UNAUTHORIZED_EVENT` not dispatched for `/api/auth/login` 401s (AC-09 nuance) | `client.ts` interceptor excludes login path | Covered by E2E-AUTH-11 conceptually but only the *absence* of redirect is asserted — not the event counter. |
| 16 | API base-URL and timeout normalization smoke (AC-P1-08) | [config.ts](../../../teamtasker_fe/lib/api/config.ts) single source-of-truth | Not exercised at runtime. |
| 17 | Calendar view rendering for expanded statuses | [tasks-calendar.tsx](../../../teamtasker_fe/components/tasks/tasks-calendar.tsx) groups by `dueDate` | E2E-TASK-11 mentions calendar but does not assert expanded-status rendering. |
| 18 | Sidebar/nav link for `/communication` removed per AC-15 | [app-sidebar.tsx](../../../teamtasker_fe/components/app-sidebar.tsx) | Not exercised — only API-layer behavior is asserted. |

Items 1, 2, 3, 4, 5, 7, 8, 11, 12, 13, 17 are addressed by proposed new cases in §4. Items 10 and 18 are recommended future work; they are lower-risk because the surfaces are currently mock-only.

## 3. Broken Flow Assessment (Confirmed 2026-04-17)

The features below are confirmed broken flows as of 2026-04-17 and are **expected to fail** under strict contract enforcement given the current codebase. Evidence is drawn from the "Known Gaps Summary" of the existing E2E document, the partial-fail rows of the P0 and P1 run-validation reports, and direct re-verification against the repository.

Confirmed missing backend surfaces include `TeamsController`, `CommunicationController`, `ActivitiesController`, all `/api/users/profile/skills` and `/api/users/profile/development-goals` routes, and admin user CRUD routes (`POST/PATCH/DELETE /api/users`).

### 3.1 Completely broken (missing backend entirely)

| Feature | Observable failure | Root cause | Impacted tests |
|---|---|---|---|
| Teams API | `GET /api/teams` → HTTP 404 | `TeamsController` not implemented. Domain `Team` entity exists but has no Application/CQRS layer. | E2E-TEAM-01 (already FAIL); new **E2E-TEAM-02** below. |
| Communication API (Team chat, DMs, announcements) | `GET /api/communication/*` → HTTP 404 | No `CommunicationController`. Endpoint module [announcements.ts](../../../teamtasker_fe/lib/api/endpoints/announcements.ts) exists but is unreachable because [`app/communication/page.tsx`](../../../teamtasker_fe/app/communication/page.tsx) renders an `EmptyState` instead of `CommunicationView`. | Covered by E2E-EDGE-10. |
| Activities API | `GET /api/activities` → HTTP 404 | No `ActivitiesController`. Frontend `recent-activity-card.tsx` is stubbed and does not issue requests. Task mutation invalidations target a query key that has no active query. | Covered by E2E-EDGE-10; partially by new **E2E-TASK-NEW-01**. |
| Profile subresource endpoints | `GET/PUT/POST/PATCH /api/users/profile/skills` and `/api/users/profile/development-goals` → HTTP 404 | `UsersController` has no routes for these paths. Frontend callers for these endpoints were removed from active dashboard and development surfaces, so the gap is now endpoint-level contract scope rather than a guaranteed page-break path. | E2E-EDGE-11; new **E2E-USER-06**, **E2E-USER-07**, **E2E-DEV-01**. |
| Admin user CRUD (`POST/PATCH/DELETE /api/users`) | HTTP 404/405 | No admin user write endpoints. Frontend hooks were removed per AC-P1-15 (verified). | E2E-USER-05. |

### 3.2 Partially broken or recently resolved

| Feature | Observable failure | Root cause | Impacted tests |
|---|---|---|---|
| Strict auth error shape on login and refresh failures | Previously drift-prone 401 payloads now return strict `{ status, title, detail }` for validated login and refresh negative paths | `AuthController` negative responses were normalized and re-validated through runtime contract execution on 2026-04-17. | E2E-AUTH-04, E2E-AUTH-07 (both now PASS). |
| Legacy unit tests | 2 tests fail on full-suite run: `UserTests.Constructor_ShouldCreateUser_WithCorrectProperties`, `CreateProjectCommandTests.Handle_ShouldCreateProject_WithCorrectValues` | Pre-existing legacy expectation mismatches outside P0/P1 scope. | Not E2E; tracked separately in AC-16/AC-P1-18 evidence. |
| Task dialog runtime persistence | Previously marked FAIL due stale status | Runtime re-verification completed on 2026-04-17 through `e2e/task-dialog.runtime.spec.ts`, with create (`POST /api/tasks` => 201) and update (`PATCH /api/tasks/{id}` => 200) assertions passing. | E2E-TASK-10 (now PASS). |

### 3.3 Blocked by environment

| Item | Why blocked | Source |
|---|---|---|
| Frontend lint and build gate (AC-17) | Toolchain is available, but repo-wide pre-existing TypeScript debt prevents clean full-project type-gate enforcement in this continuation scope. | Local `pnpm exec tsc --noEmit` execution in continuation validation. |
| Playwright E2E execution | Executable when backend and frontend services are running and seeded auth context is available. | `e2e_test_cases.md` §Playwright Execution Strategy and continuation runtime runs. |

### 3.4 Consolidated FAIL-list and re-verification list

All currently known confirmed expected-to-fail features, combined:

* Teams controller absence (E2E-TEAM-01).
* Communication controller absence (E2E-EDGE-10 under strict interpretation).
* Activities controller absence (E2E-EDGE-10 under strict interpretation).
* Missing profile skills and development-goals routes for direct endpoint contracts (E2E-USER-06, E2E-USER-07).
* Admin user CRUD route absence (`POST/PATCH/DELETE /api/users`; E2E-USER-05).

Runtime-validated in this continuation and recommended for periodic regression checks:

* Team workload users/tasks data path and `/api/teams` non-usage (E2E-TEAM-02).
* Graceful degradation of profile/development surfaces while profile subresource routes remain unimplemented (E2E-EDGE-11, E2E-DEV-01).

## 4. New E2E Test Cases

The following cases are drafted in the exact format used by `e2e_test_cases.md` so that they can be inserted directly. IDs are chosen so that they do not collide with existing identifiers; they use the `-NEW` suffix temporarily — when merged, they should adopt the next sequential number in their category.

### E2E-TASK-NEW-01: Full Task Lifecycle Invalidates Activity Query Keys

| Field | Value |
|---|---|
| **ID** | E2E-TASK-NEW-01 |
| **Title** | Task create → update → progress → complete → delete invalidates `['activities']` query keys on each mutation |
| **Traces to** | AC-P1-16, AC-03, AC-04, AC-05, AC-06, AC-07 |
| **Pre-conditions** | Authenticated Admin user. An `['activities']` query is registered in React Query (via a test-only `useQuery` subscription, since the dashboard activity feed is stubbed). |
| **Steps** | |

1. Subscribe to `useQuery({ queryKey: ['activities'] })` inside a test harness and capture its `dataUpdatedAt`.
2. `POST /api/tasks` with a valid payload. Wait for `onSuccess`.
3. `PATCH /api/tasks/{id}` changing title and priority.
4. `PATCH /api/tasks/{id}/progress` to `50`.
5. `PATCH /api/tasks/{id}/status` to `completed`.
6. `DELETE /api/tasks/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | Each call returns the contract specified in E2E-TASK-01, E2E-TASK-05, E2E-TASK-07, E2E-TASK-06, E2E-TASK-08 respectively. |
| **Frontend** | After every mutation, the `['activities']` query’s `dataUpdatedAt` increments (i.e., `invalidateQueries` was called). The assertion is key-level, not response-level, so it is not blocked by the missing `GET /api/activities` endpoint. |
| **Known gap** | Backend activity feed is not implemented. This case verifies the client-side invalidation contract only. Once `ActivitiesController` ships, this case should be extended to assert a successful refetch. |
| **Status** | UNTESTED |

---

### E2E-AUTH-NEW-01: Pre-Expiry Proactive Refresh

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-NEW-01 |
| **Title** | Client refreshes access token when remaining TTL crosses the pre-expiry threshold |
| **Traces to** | AC-12, AC-P1-17 |
| **Pre-conditions** | Authenticated session with a valid `auth_token_expiry` in localStorage that is within the pre-expiry window (default 5 minutes). Valid refresh token stored. |
| **Steps** | |

1. Seed localStorage with an access token whose `expiresAt` is 2 minutes in the future.
2. Issue a benign protected request (for example `GET /api/users/profile`).
3. Observe network activity and localStorage mutations.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/auth/refresh-token` returns HTTP 200 with a new token and rotated refresh token; the follow-up protected request returns HTTP 200. |
| **Frontend** | Token-refresh interceptor transparently triggers `refresh-token` before the protected call. `auth_token` and `auth_refresh_token` in localStorage are updated silently. No redirect to `/login`, no toast. The request sequence must show the refresh call strictly before the protected call. |
| **Status** | UNTESTED |

---

### E2E-TASK-NEW-02: Task Assignment Round-Trip

| Field | Value |
|---|---|
| **ID** | E2E-TASK-NEW-02 |
| **Title** | Creating/updating a task with `assigneeId` returns a shaped `assignee` object on read |
| **Traces to** | AC-03, AC-04, AC-02 |
| **Pre-conditions** | Authenticated user. At least one other user exists whose id is used as `assigneeId`. |
| **Steps** | |

1. `POST /api/tasks` with `{ "title": "Assign me", "projectId": 1, "assigneeId": <userId> }`.
2. `GET /api/tasks/{id}` on the returned task.
3. `PATCH /api/tasks/{id}` with `{ "assigneeId": null }`.
4. `GET /api/tasks/{id}` again.

| **Expected Result** | |
|---|---|
| **Backend** | Step 2: `assignee` equals `{ id: <userId>, name: "<Full Name>", avatar: <string\|null>, initials: <two-letter string> }`. Step 4: `assignee` is exactly `null`. |
| **Frontend** | Task card shows the assignee avatar and initials when assigned; falls back to "Unassigned" pill when null. No runtime error thrown when `assignee` transitions from object to null. |
| **Status** | UNTESTED |

---

### E2E-TASK-NEW-03: Task Tag Round-Trip

| Field | Value |
|---|---|
| **ID** | E2E-TASK-NEW-03 |
| **Title** | Tags submitted on create and update are persisted and re-emitted |
| **Traces to** | AC-03, AC-04 |
| **Pre-conditions** | Authenticated user. |
| **Steps** | |

1. `POST /api/tasks` with `{ "title": "Tagged", "projectId": 1, "tags": ["Backend", "API"] }`.
2. `GET /api/tasks/{id}`.
3. `PATCH /api/tasks/{id}` with `{ "tags": ["Backend", "API", "Testing"] }`.
4. `GET /api/tasks/{id}`.
5. `PATCH /api/tasks/{id}` with `{ "tags": [] }`.
6. `GET /api/tasks/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | Each GET returns `tags` equal to the most recent submitted array (order-insensitive, duplicates collapsed). |
| **Frontend** | Task dialog tag chips match the returned array. Removing a chip then saving persists the removal. |
| **Status** | UNTESTED |

---

### E2E-TASK-NEW-04: Critical Priority Round-Trip

| Field | Value |
|---|---|
| **ID** | E2E-TASK-NEW-04 |
| **Title** | `critical` priority survives the full mapper boundary on both create and update |
| **Traces to** | AC-P1-03, AC-P1-04 |
| **Pre-conditions** | Authenticated user. |
| **Steps** | |

1. `POST /api/tasks` with `{ ..., "priority": "critical" }`.
2. `GET /api/tasks/{id}` — assert `priority === "critical"` (string equality, not coerced to `"high"`).
3. `PATCH /api/tasks/{id}` with `{ "priority": "low" }`.
4. `GET /api/tasks/{id}` — assert `priority === "low"`.

| **Expected Result** | |
|---|---|
| **Backend** | `TaskContractMapper.ToClientPriority` emits `"critical"` rather than collapsing it. Round-trip is lossless. |
| **Frontend** | Priority badge in list/kanban/detail reads "Critical" with the critical-variant styling and does not render as "High". |
| **Status** | UNTESTED |

---

### E2E-TASK-NEW-05: Kanban Renders All Six Status Columns Simultaneously

| Field | Value |
|---|---|
| **ID** | E2E-TASK-NEW-05 |
| **Title** | Kanban view displays columns for `todo`, `in-progress`, `blocked`, `on-hold`, `completed`, and `cancelled` and places each task in the correct column |
| **Traces to** | AC-P1-02, AC-P1-05 |
| **Pre-conditions** | At least six tasks exist, one per status. Authenticated user on `/tasks`. |
| **Steps** | |

1. Load `/tasks` kanban view.
2. Inspect the DOM for column headers and the mapping of task cards to columns.

| **Expected Result** | |
|---|---|
| **Backend** | `GET /api/tasks?page=1&limit=100` returns at least one task per status with correct `status` strings. |
| **Frontend** | Six columns render with explicit labels. No task is silently dropped or grouped into a fallback "other" bucket. Each card lives in the column matching its `status` field. |
| **Status** | UNTESTED |

---

### E2E-USER-06: Update Profile Skills Endpoint Status

| Field | Value |
|---|---|
| **ID** | E2E-USER-06 |
| **Title** | `PUT /api/users/profile/skills` endpoint is not implemented |
| **Traces to** | AC-P1-01, Known Gap (profile subresources) |
| **Pre-conditions** | Authenticated user. |
| **Steps** | |

1. `PUT /api/users/profile/skills` with body `{ "skills": [{ "skillId": 1, "proficiencyLevel": 3 }] }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 404. `UsersController` does not expose a route for `profile/skills`. |
| **Frontend** | Any direct invocation path surfaces a non-empty error state and does not silently swallow the failure. No inconsistent local cache mutation is persisted after a failed call. |
| **Known gap** | Backend skills endpoint not implemented. |
| **Status** | FAIL (expected — known missing feature) |

---

### E2E-USER-07: Development Goals CRUD Endpoint Status

| Field | Value |
|---|---|
| **ID** | E2E-USER-07 |
| **Title** | `/api/users/profile/development-goals` CRUD endpoints are not implemented |
| **Traces to** | AC-P1-01, Known Gap (profile subresources) |
| **Pre-conditions** | Authenticated user. |
| **Steps** | |

1. `GET /api/users/profile/development-goals`.
2. `POST /api/users/profile/development-goals` with a valid goal body.
3. `PATCH /api/users/profile/development-goals/1` with a partial update.

| **Expected Result** | |
|---|---|
| **Backend** | All three calls return HTTP 404. |
| **Frontend** | Any direct invocation path resolves to a visible error state and rejection handling; no crash. |
| **Known gap** | Backend development-goals endpoints not implemented. |
| **Status** | FAIL (expected — known missing feature) |

---

### E2E-DEV-01: Development Page Degradation

| Field | Value |
|---|---|
| **ID** | E2E-DEV-01 |
| **Title** | Development and dashboard surfaces remain stable while profile subresource routes are absent |
| **Traces to** | AC-P1-01, AC-15 |
| **Pre-conditions** | Authenticated user; backend running with no profile subresource endpoints. |
| **Steps** | |

1. Navigate to `/development`.
2. Navigate to `/dashboard`.
3. Observe network tab for calls to `/api/users/profile/development-goals` and `/api/users/profile/skills`.
4. Observe rendered UI.

| **Expected Result** | |
|---|---|
| **Backend** | Profile subresource routes remain unimplemented and would return HTTP 404 if called directly. |
| **Frontend** | Development and dashboard surfaces remain usable and do not require runtime calls to missing profile subresource endpoints. No uncaught error is surfaced to the React error boundary. |
| **Known gap** | Backend profile subresource routes remain unavailable for future feature expansion. |
| **Status** | UNTESTED |

---

### E2E-AUTH-NEW-02: Change Password With Incorrect Current Password

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-NEW-02 |
| **Title** | Change-password rejects when `currentPassword` is wrong |
| **Traces to** | AC-09 |
| **Pre-conditions** | Authenticated user with known password. |
| **Steps** | |

1. `POST /api/auth/change-password` with `{ "currentPassword": "WrongPass", "newPassword": "NewSecure456!" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400 or 401 (per controller implementation). Body conforms to `{ status, title, detail }`. Password not changed — subsequent login with the old password still succeeds. |
| **Frontend** | Error toast with non-empty `detail`. No logout; session persists. |
| **Status** | UNTESTED |

---

### E2E-AUTH-NEW-03: Reset Password With Invalid Token

| Field | Value |
|---|---|
| **ID** | E2E-AUTH-NEW-03 |
| **Title** | Reset-password rejects when token is tampered or expired |
| **Traces to** | AC-09 |
| **Pre-conditions** | User exists; no valid active reset token. |
| **Steps** | |

1. `POST /api/auth/reset-password` with `{ "email": "test-reg@example.com", "token": "not-a-real-token", "newPassword": "ResetPass789!" }`.

| **Expected Result** | |
|---|---|
| **Backend** | HTTP 400 or 401 with strict `{ status, title, detail }` body indicating invalid token. Password is not updated. |
| **Frontend** | Error toast renders the `detail`. Reset form stays visible. |
| **Status** | UNTESTED |

---

### E2E-PROJ-06: Admin Project Update and Delete

| Field | Value |
|---|---|
| **ID** | E2E-PROJ-06 |
| **Title** | Admin updates and deletes an existing project |
| **Traces to** | AC-13, AC-P1-10 |
| **Pre-conditions** | Admin JWT. Existing project. |
| **Steps** | |

1. `PUT /api/projects/{id}` with a valid update body.
2. `GET /api/projects/{id}`.
3. `DELETE /api/projects/{id}`.
4. `GET /api/projects/{id}`.

| **Expected Result** | |
|---|---|
| **Backend** | Step 1: HTTP 200 with updated DTO. Step 2: fields persisted. Step 3: HTTP 204. Step 4: HTTP 404 with `{ status, title, detail }`. |
| **Frontend** | N/A (no project UI). |
| **Status** | UNTESTED |

---

### E2E-TEAM-02: Team Workload Data Source

| Field | Value |
|---|---|
| **ID** | E2E-TEAM-02 |
| **Title** | Team page derives workload from `/api/users` and `/api/tasks` and never calls `/api/teams` |
| **Traces to** | AC-15, AC-P1-01 |
| **Pre-conditions** | Authenticated user. Multiple users and tasks seeded. |
| **Steps** | |

1. Navigate to `/team`.
2. Observe network tab for requests to `/api/users`, `/api/tasks`, and `/api/teams*`.
3. Inspect rendered workload view.

| **Expected Result** | |
|---|---|
| **Backend** | At most one `GET /api/users` and one `GET /api/tasks` call fire during the page load. Zero requests are issued against any `/api/teams*` path. |
| **Frontend** | Workload view renders rows computed from the two payloads. Each user's workload reflects their assigned tasks. No `GET /api/teams` 404 appears in the console or network log. |
| **Known gap** | Today `TeamWorkloadView` uses mock data and does not fetch at all. This test will initially FAIL because the live data-flow is unimplemented. The test documents the intended end-state and flags the unimplemented data path. |
| **Status** | FAIL (expected — data path not yet wired) |

---

### E2E-CFG-01: API Base URL and Timeout Normalization Smoke

| Field | Value |
|---|---|
| **ID** | E2E-CFG-01 |
| **Title** | All frontend API calls use the single normalized base URL and 15s timeout |
| **Traces to** | AC-P1-08, AC-P1-09 |
| **Pre-conditions** | Frontend running with default env. |
| **Steps** | |

1. In a test harness, instrument the axios client and perform one call per endpoint domain: `auth`, `users`, `tasks`, `projects`.
2. Capture `request.baseURL` and `request.timeout` for each.

| **Expected Result** | |
|---|---|
| **Backend** | N/A. |
| **Frontend** | Every captured request has `baseURL` equal to `API_CONFIG.baseURL` (default `http://localhost:5220/api`) and `timeout === 15000`. No requests target the legacy Mockoon `:3002` base URL. `proxy.js` is not present in the frontend bundle. |
| **Status** | UNTESTED |

---

### E2E-CAL-01: Calendar View Renders Expanded Status Tasks

| Field | Value |
|---|---|
| **ID** | E2E-CAL-01 |
| **Title** | Calendar view renders tasks regardless of status, including `blocked`, `on-hold`, `cancelled` |
| **Traces to** | AC-01, AC-P1-05 |
| **Pre-conditions** | At least one task per status, each with a `dueDate` within the visible month. Authenticated user. |
| **Steps** | |

1. Load `/tasks` calendar view for the month containing all seeded tasks.
2. Inspect the cells for each due date.

| **Expected Result** | |
|---|---|
| **Backend** | `GET /api/tasks` returns each seeded task with its expected status. |
| **Frontend** | Each seeded task appears on the expected date. Status-variant styling is applied per task. No task is silently hidden because its status is not in the legacy three-status set. |
| **Status** | UNTESTED |

---

## 5. Status Legend Update

### 5.1 Status corrections after runtime re-validation

The continuation run produced runtime evidence that changed several previously stale statuses.

| Test case | Previous | Updated | Evidence |
|---|---|---|---|
| E2E-AUTH-04 | UNTESTED/FAIL (stale) | **PASS** | `e2e/auth.contract.spec.ts` passed on 2026-04-17 with strict 401 shape assertions. |
| E2E-AUTH-07 | UNTESTED/FAIL (stale) | **PASS** | `e2e/auth.contract.spec.ts` passed on 2026-04-17 with refresh replay rejection assertions. |
| E2E-TASK-10 | FAIL (stale marker) | **PASS** | `e2e/task-dialog.runtime.spec.ts` passed on 2026-04-17 with create (201) and update (200) assertions. |
| E2E-EDGE-11 | UNTESTED | **PASS** | `e2e/surface-contract.runtime.spec.ts` validated direct profile subresource endpoint 404 responses plus stable profile navigation without mandatory subresource calls. |
| E2E-DEV-01 | FAIL draft | **PASS** | `e2e/surface-contract.runtime.spec.ts` validated development and dashboard navigation without calls to missing profile subresource endpoints. |
| E2E-TEAM-02 | FAIL draft | **PASS** | `e2e/surface-contract.runtime.spec.ts` validated `/team` usage of `/api/users` and `/api/tasks` with no `/api/teams` requests. |

### 5.2 Recommended new default statuses for the proposed cases

| New test | Suggested status | Justification |
|---|---|---|
| E2E-TASK-NEW-01 | UNTESTED | Requires client-side harness; not a known broken contract. |
| E2E-AUTH-NEW-01 | UNTESTED | Interceptor behavior exists in code — needs runtime verification. |
| E2E-TASK-NEW-02 | UNTESTED | Controller supports assignee; needs runtime assertion. |
| E2E-TASK-NEW-03 | UNTESTED | `TaskTag` entity present; round-trip unverified. |
| E2E-TASK-NEW-04 | UNTESTED | Mapper unit tests pass; UI round-trip unverified. |
| E2E-TASK-NEW-05 | UNTESTED | UI column expansion verified statically; unverified at runtime. |
| E2E-USER-06 | **FAIL** | Endpoint absent. |
| E2E-USER-07 | **FAIL** | Endpoints absent. |
| E2E-DEV-01 | PASS | Runtime-validated graceful navigation behavior without mandatory profile subresource calls. |
| E2E-AUTH-NEW-02 | UNTESTED | Handler exists; negative-path unverified. |
| E2E-AUTH-NEW-03 | UNTESTED | Handler exists; negative-path unverified. |
| E2E-PROJ-06 | UNTESTED | Endpoints exist; happy-path unverified. |
| E2E-TEAM-02 | PASS | Runtime-validated users/tasks data flow with no `/api/teams` requests. |
| E2E-CFG-01 | UNTESTED | Config normalization present; runtime assertion pending. |
| E2E-CAL-01 | UNTESTED | Calendar renders today but expanded-status render unverified. |

### 5.3 Cases requiring re-verification

| Test case | Current | Suggested action |
|---|---|---|
| E2E-TEAM-02 | PASS | Re-run as a periodic regression check to ensure `/team` continues using `/api/users` and `/api/tasks` without `/api/teams` requests. |
| E2E-EDGE-11 | PASS | Re-run when profile surfaces change to ensure direct endpoint 404 behavior remains non-fatal to navigation. |
| E2E-DEV-01 | PASS | Re-run as dashboard/development widgets evolve to ensure missing profile subresources remain non-blocking. |

### 5.4 Updated verification status summary

After continuation implementation and runtime validation:

| Status | Count (after update) | Examples |
|---|---|---|
| FAIL | Endpoint or controller absence that still exists | E2E-TEAM-01, E2E-USER-05, E2E-USER-06, E2E-USER-07 |
| PASS | Runtime-validated during continuation | E2E-AUTH-04, E2E-AUTH-07, E2E-TASK-10, E2E-TEAM-02, E2E-EDGE-11, E2E-DEV-01 |
| UNTESTED | Remaining scenarios pending execution | New draft cases (for example E2E-TASK-NEW-* and E2E-AUTH-NEW-*) plus broader suite coverage |

## 6. Recommended Follow-Up Actions

1. **Run periodic runtime checks for E2E-TEAM-02, E2E-EDGE-11, and E2E-DEV-01** to keep regressions visible while backend profile subresource routes remain unimplemented.
2. **Implement backend profile subresource routes** (`/api/users/profile/skills`, `/api/users/profile/development-goals`) when product scope requires these features.
3. **Add a CI workflow gate** for strict Playwright contract suites using `npm run test:e2e:strict` with backend/frontend service orchestration.
4. **Triage the two legacy unit-test failures** (`UserTests` and `CreateProjectCommandTests`) outside the E2E continuation scope.

## 7. User-Journey-Oriented E2E Test Cases

The existing suite is organized by API surface (auth, tasks, projects, users, security, errors, edges). Under strict contract enforcement that is exactly what is needed — but it leaves a second kind of coverage gap: the suite rarely exercises a **realistic multi-step user narrative** where one person's actions propagate across screens, roles, and sessions. The cases below are organized around personas and goals, not endpoints.

Each case is a compound scenario: it may drive the same endpoint already tested elsewhere, but it asserts the *composition* — does the full experience hold together when a real user carries out a task end-to-end?

**Personas used below**
* *New User* — first-time visitor. No session, no data.
* *Contributor* — authenticated `User` role. Works individual tasks.
* *Team Lead* — authenticated `Admin` role. Creates work, assigns, deletes, reviews.
* *Returning User* — authenticated `User` with prior session state (cached queries, stored tokens).

### E2E-JOURNEY-01: New User Onboarding to First Task

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-01 |
| **Title** | A new user registers, lands on the dashboard, completes their profile, and creates their first task |
| **Traces to** | AC-10, AC-P1-01, AC-P1-12, AC-P1-14, AC-03, AC-P1-17 |
| **Persona** | New User |
| **Pre-conditions** | Browser has no auth cookie or tokens. No existing user with email `journey01@example.com` or username `journey01`. At least one project exists to attach tasks to. |
| **Steps** | |

1. Navigate to `/` — middleware redirects unauthenticated request to `/login`.
2. Click "Register", fill the form (`firstName`, `lastName`, email `journey01@example.com`, username `journey01`, password `SecurePass123!`), submit.
3. Observe automatic redirect and initial dashboard render.
4. Navigate to `/profile`, add `bio`, `location`, `phone`, save.
5. Navigate to `/tasks`, open the Create Task dialog, fill title "My first task", priority `medium`, assignee = self, save.
6. Return to `/dashboard` (or the task list) and locate the newly created task.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/auth/register` → 201 with `{ token, refreshToken, expiresAt, role: "User" }`. `PATCH /api/users/profile` → 200 with the updated fields. `POST /api/tasks` → 201. `GET /api/tasks` list contains the new task. |
| **Frontend** | After registration, `auth_token`, `auth_refresh_token`, and `auth_token_expiry` are written to localStorage; `auth-token` cookie is also set (so the middleware grants access). User is redirected to `/dashboard` with their display name visible. Profile fields persist after reload. The new task appears in both list and kanban "Todo" column without a manual refresh. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-02: Team Lead Assigns a Critical Task to a Contributor

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-02 |
| **Title** | An Admin creates a critical task with an assignee, tags, and due date; the contributor sees it on their next login |
| **Traces to** | AC-03, AC-04, AC-P1-03, AC-P1-05, AC-P1-10, AC-P1-12, AC-P1-16 |
| **Persona** | Team Lead + Contributor (sequential) |
| **Pre-conditions** | Admin and Contributor accounts exist. At least one project exists. Two browsers or two incognito windows available. |
| **Steps** | |

1. Admin logs into browser A, navigates to `/tasks`, opens Create Task dialog.
2. Admin fills title "Fix login regression", priority `critical`, due date 3 days out, tags `["Backend", "Security"]`, assignee = Contributor, saves.
3. Admin verifies task appears in the kanban "Todo" column with a red/critical badge.
4. Admin logs out.
5. Contributor logs into browser B. Navigates to `/tasks` and filters by `assignee = me` (or opens dashboard if the dashboard surfaces "my tasks").
6. Contributor opens the task detail and reads the description, tags, priority, due date, and assignee badge.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/tasks` → 201 with `assignee: { id, name, avatar, initials }`, `priority: "critical"`, `tags: ["Backend", "Security"]`. `GET /api/tasks` called by the Contributor returns the task with the same shape. |
| **Frontend** | Critical-variant styling is applied to the priority badge (not "High"). Tags render as chips. Assignee initials match the Contributor. Task is findable via the assignee filter. Activity query keys were invalidated after the create mutation (observable via a devtools instrument). |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-03: Contributor Completes a Task Through Progress Increments

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-03 |
| **Title** | A contributor progresses a task from 0 → 25 → 50 → 100, observing automatic status transitions |
| **Traces to** | AC-05, AC-06, AC-P1-02, AC-P1-05, AC-P1-16 |
| **Persona** | Contributor |
| **Pre-conditions** | Contributor is logged in. An assigned task in `todo` status, progress 0, exists. |
| **Steps** | |

1. Open the task detail.
2. Set progress to 25. Observe kanban column.
3. Set progress to 50.
4. Set progress to 100.
5. Reload the page and re-open the detail.

| **Expected Result** | |
|---|---|
| **Backend** | Step 2: `PATCH /api/tasks/{id}/progress` → 200, returned `progress=25`, `status="in-progress"` (auto-promoted off `todo`). Step 4: `status="completed"`, `progress=100`. |
| **Frontend** | Kanban card moves Todo → In Progress at step 2 and In Progress → Completed at step 4. Status badge updates without page reload. After reload, final state persists (progress 100, status completed). No stale state from optimistic updates. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-04: Kanban Drag-and-Drop Status Change

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-04 |
| **Title** | User drags a task card across kanban columns and the new status persists after reload |
| **Traces to** | AC-05, AC-P1-02, AC-P1-05 |
| **Persona** | Contributor |
| **Pre-conditions** | User logged in. At least one task per target status exists. Kanban view contains columns for all six statuses. |
| **Steps** | |

1. Open `/tasks` kanban view.
2. Drag a `todo` card onto the `in-progress` column.
3. Drag the same card onto `blocked`.
4. Drag the same card onto `completed`.
5. Reload the page.

| **Expected Result** | |
|---|---|
| **Backend** | Each drop triggers one `PATCH /api/tasks/{id}/status` → 200 with the correct status string. No N+1 chatter. |
| **Frontend** | Optimistic UI moves the card immediately; if the API fails, the card reverts. After reload, card sits in the `completed` column. Drag source columns clean up empty placeholders. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-05: Blocked Task Escalation and Recovery

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-05 |
| **Title** | A contributor flags a task as `blocked`; the team lead investigates and moves it back to `in-progress` |
| **Traces to** | AC-05, AC-P1-02, AC-P1-05, AC-P1-10 |
| **Persona** | Contributor → Team Lead |
| **Pre-conditions** | Admin and Contributor exist. Contributor has an assigned task currently in `in-progress`. |
| **Steps** | |

1. Contributor logs in, opens the task detail, changes status to `blocked`.
2. Contributor logs out.
3. Admin logs in, navigates to `/tasks?status=blocked` (filter or kanban column).
4. Admin opens the blocked task, changes status to `in-progress`, and updates progress to `60`.
5. Admin logs out; Contributor logs in and reopens the task.

| **Expected Result** | |
|---|---|
| **Backend** | Status transitions round-trip through `/api/tasks/{id}/status`. Both roles are allowed to update status (Admin + User). |
| **Frontend** | Admin can see the `blocked` column/filter populated. After the Admin's fix, Contributor re-opens the task and sees status `in-progress`, progress `60`, with no stale cache from their previous session. |
| **Known gap** | This journey would benefit from a `comments` or `activity note` feature; none exists today. The case does not assert comment posting. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-06: Team Lead Cleans Up Cancelled Tasks

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-06 |
| **Title** | Admin filters tasks by `cancelled`, reviews the list, and deletes stale entries |
| **Traces to** | AC-01, AC-07, AC-P1-10, AC-P1-13 |
| **Persona** | Team Lead |
| **Pre-conditions** | Admin logged in. Multiple tasks with `status=cancelled` exist. |
| **Steps** | |

1. Navigate to `/tasks`, apply `status=cancelled` filter.
2. For each cancelled task, click the row-level delete action.
3. Confirm deletion in the modal.
4. Repeat until zero cancelled tasks remain.
5. Clear filter; confirm the non-cancelled tasks are untouched.

| **Expected Result** | |
|---|---|
| **Backend** | Each `DELETE /api/tasks/{id}` → 204. Subsequent `GET /api/tasks?status=cancelled` returns `data: []`. Non-cancelled counts unchanged. |
| **Frontend** | The delete action is only rendered because the user is Admin. List updates after each deletion via `removeQueries`. Empty-state message appears after the last deletion. Pagination "0 results" is handled gracefully. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-07: Transparent Session Continuity Across a Long Work Session

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-07 |
| **Title** | A user works across the near-expiry window and the client transparently refreshes the access token with no user-visible interruption |
| **Traces to** | AC-10, AC-11, AC-12, AC-P1-17 |
| **Persona** | Contributor |
| **Pre-conditions** | User is freshly logged in. Access token has a known `expiresAt`. |
| **Steps** | |

1. Start a timer. Exercise normal app features (task list, task detail, profile) every 2 minutes.
2. Simulate (or wait for) the access token reaching its pre-expiry threshold (~5 minutes before `expiresAt`).
3. Trigger one more protected request after the threshold crosses.
4. Continue normal use for 5 more minutes.

| **Expected Result** | |
|---|---|
| **Backend** | A single `POST /api/auth/refresh-token` call is issued during the pre-expiry window and returns a rotated pair. The protected request that triggered the refresh still succeeds without retry. |
| **Frontend** | No redirect, no toast, no `UNAUTHORIZED_EVENT`. `auth_token` and `auth_refresh_token` are silently replaced in localStorage. `auth_token_expiry` moves forward. Developer-console network log shows refresh → protected in strict sequence. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-08: Session Expiry After Idle Forces a Clean Re-login

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-08 |
| **Title** | A user who idles past both access-token and refresh-token TTLs is cleanly redirected to `/login` on next action |
| **Traces to** | AC-11, AC-14, AC-P1-14 |
| **Persona** | Returning User |
| **Pre-conditions** | User is logged in. Both `auth_token` and `auth_refresh_token` are expired (or manually seeded to expired values). |
| **Steps** | |

1. Load `/tasks` (the client tries to use the expired tokens).
2. Observe the network and UI.
3. Log in again with valid credentials.

| **Expected Result** | |
|---|---|
| **Backend** | Protected endpoint → 401. `POST /api/auth/refresh-token` → 401 with strict `{ status, title, detail }` body. No new tokens issued. |
| **Frontend** | Axios interceptor dispatches `UNAUTHORIZED_EVENT`. All `auth_*` localStorage keys and the auth cookie are removed. Toast reads "Your session has expired". User is redirected to `/login`. After re-login, `/tasks` loads fresh data (no stale list for the prior session). |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-09: Password Recovery End-to-End

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-09 |
| **Title** | A user who has forgotten their password recovers access via the reset flow |
| **Traces to** | AC-09 |
| **Persona** | Returning User (credentials lost) |
| **Pre-conditions** | User with email `journey09@example.com` exists. |
| **Steps** | |

1. From `/login`, click "Forgot password?".
2. Enter the email and submit.
3. Retrieve the reset token (from dev mailbox or backend test helper).
4. Navigate to `/reset-password?token=<token>`.
5. Submit a new password.
6. Log in with the new password.
7. Attempt to log in with the old password.

| **Expected Result** | |
|---|---|
| **Backend** | `POST /api/auth/forgot-password` → 200. `POST /api/auth/reset-password` → 200. Login with the new password → 200. Login with the old password → 401 (strict body). |
| **Frontend** | Each step shows a success toast. After reset, the login form accepts the new password. Old password login triggers the invalid-credentials toast with a non-empty detail. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-10: Shared Workstation — Cache Isolation Across Users

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-10 |
| **Title** | On a shared device, logging out and logging in as a second user never exposes the first user's cached data |
| **Traces to** | AC-14 |
| **Persona** | Returning User A → New session as User B |
| **Pre-conditions** | Both accounts exist. Each has distinct tasks, projects, and profiles. Browser with persistent storage. |
| **Steps** | |

1. User A logs in, navigates through `/tasks`, `/team`, `/profile` so that the React Query cache is populated.
2. User A clicks logout.
3. Without reloading the page, User B logs in on the same tab.
4. User B navigates to `/tasks`, `/team`, `/profile`.

| **Expected Result** | |
|---|---|
| **Backend** | After User B login, `GET /api/tasks`, `GET /api/users`, and `GET /api/users/profile` are called fresh (no `cache: "only-if-cached"` mirror). |
| **Frontend** | `queryClient.clear()` runs on logout; User B never sees a flash of User A's tasks or profile. Sidebar renders User B's name. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-11: Non-Admin Destructive Action Defense-in-Depth

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-11 |
| **Title** | A `User`-role session cannot delete a task through either the UI or a direct API call, and receives user-visible feedback on the API attempt |
| **Traces to** | AC-P1-06, AC-P1-07, AC-P1-10, AC-P1-13 |
| **Persona** | Contributor |
| **Pre-conditions** | Contributor logged in. At least one task visible. |
| **Steps** | |

1. Open `/tasks` list view and inspect the row actions.
2. Attempt to trigger the delete via the UI (the control should be absent).
3. Open devtools and run `fetch("/api/tasks/{id}", { method: "DELETE", headers: { Authorization: "Bearer " + localStorage.getItem("auth_token") } })`.
4. Observe the toast and the task list.

| **Expected Result** | |
|---|---|
| **Backend** | Direct delete → HTTP 403. |
| **Frontend** | No delete button rendered in the row actions. Devtools call triggers `FORBIDDEN_EVENT`; `ApiErrorListener` shows a destructive toast containing "forbidden" or "permission". Task remains in the list. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-12: Profile Update Persists Across Reload

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-12 |
| **Title** | Editing bio, location, and phone persists across a hard reload and across a logout/login cycle |
| **Traces to** | AC-P1-01, AC-04 |
| **Persona** | Contributor |
| **Pre-conditions** | Contributor logged in. Profile has known initial values. |
| **Steps** | |

1. Navigate to `/profile`. Note current `bio`, `location`, `phone`.
2. Edit all three fields, save.
3. Observe success toast.
4. Hard-reload the page (Cmd/Ctrl+Shift+R).
5. Log out and log back in.
6. Re-navigate to `/profile`.

| **Expected Result** | |
|---|---|
| **Backend** | `PATCH /api/users/profile` → 200. Subsequent `GET /api/users/profile` returns the new values. |
| **Frontend** | Updated values render immediately (via `setQueryData`). After hard reload, values are still present (fetched fresh from the server). After logout/login, values still correct. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-13: Calendar-First Planning

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-13 |
| **Title** | A user uses the calendar view to triage upcoming work and reschedule a task by changing its due date |
| **Traces to** | AC-01, AC-04, AC-P1-05 |
| **Persona** | Contributor |
| **Pre-conditions** | Contributor logged in. Multiple tasks exist across at least three distinct due dates in the visible month, including one `blocked` and one `on-hold` task. |
| **Steps** | |

1. Open `/tasks` and switch to calendar view.
2. Locate the task on its original due date.
3. Click the task to open the detail.
4. Change the due date to +7 days, save.
5. Return to the calendar.

| **Expected Result** | |
|---|---|
| **Backend** | `PATCH /api/tasks/{id}` → 200 with the new `dueDate`. |
| **Frontend** | Calendar view shows the task on its new date and the old cell no longer shows it. Expanded-status tasks (`blocked`, `on-hold`) render with the correct styling — they are not hidden. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-14: Development Journey Known-Broken (Graceful Degradation)

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-14 |
| **Title** | A user navigates to `/development` and sees a degraded-but-stable experience because backend subresources are not implemented |
| **Traces to** | AC-P1-01, AC-15 |
| **Persona** | Contributor |
| **Pre-conditions** | Contributor logged in. Backend has no `/api/users/profile/skills` or `/api/users/profile/development-goals` routes. |
| **Steps** | |

1. Click "Development" in the sidebar.
2. Wait for network requests to settle.
3. Switch across the tabs (Learning Resources, Skills Tracker, Career Path).
4. Attempt to add a development goal.

| **Expected Result** | |
|---|---|
| **Backend** | `GET /api/users/profile/development-goals` → 404. `PUT /api/users/profile/skills` → 404. `POST /api/users/profile/development-goals` → 404. |
| **Frontend** | Page does not crash. Each failing panel renders an `ErrorState` or `EmptyState` with a non-empty user message. Page header and static tabs remain interactive. The add-goal action surfaces a non-empty error toast rather than silently failing. |
| **Known gap** | Backend routes are not implemented. Frontend continues to issue these requests. Either implement the routes or remove the hooks. |
| **Status** | FAIL (expected — known missing feature) |

---

### E2E-JOURNEY-15: Registration Validation Recovery

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-15 |
| **Title** | A user who makes registration mistakes sees field-level errors, corrects them, and succeeds |
| **Traces to** | AC-09, AC-10 |
| **Persona** | New User |
| **Pre-conditions** | User with email `journey15@example.com` already exists (for the duplicate-email leg). |
| **Steps** | |

1. On `/register`, submit the form with the duplicate email and a weak password `pass`.
2. Observe server-side validation response and the rendered errors.
3. Change password to `SecurePass123!` but keep the duplicate email. Submit.
4. Change email to `journey15-fresh@example.com`. Submit.

| **Expected Result** | |
|---|---|
| **Backend** | Step 1: 400 with `errors` dictionary naming `Password` (weakness) and possibly other fields. Step 3: 409 Conflict with `{ status: 409, title: "Conflict", detail: "User with this email already exists" }`. Step 4: 201 with token pair. |
| **Frontend** | Field-level error messages render per `errors` dictionary. Global toast shows the conflict detail. On success, user is redirected to `/dashboard` with tokens stored. |
| **Status** | UNTESTED |

---

### E2E-JOURNEY-16: Pagination, Filter, and Search Exploration

| Field | Value |
|---|---|
| **ID** | E2E-JOURNEY-16 |
| **Title** | A user browses a large task list using pagination, status filter, and search, receiving correctly scoped results at every step |
| **Traces to** | AC-01, AC-08, AC-P1-05 |
| **Persona** | Contributor |
| **Pre-conditions** | At least 25 tasks across mixed statuses and priorities. At least 3 contain the token "login" in title or description. |
| **Steps** | |

1. Open `/tasks` list view at `page=1&limit=10`.
2. Advance to page 2, then page 3.
3. Apply `status=blocked` filter. Advance through resulting pages.
4. Clear status filter; apply `search=login`.
5. Combine `status=in-progress` with `search=login`.

| **Expected Result** | |
|---|---|
| **Backend** | Each request uses `search` (not `searchTerm`) as the query key. `totalPages` matches returned `total / limit`. Filter parameters are honored exactly. |
| **Frontend** | Page counter updates correctly. Filter + search combine without duplicating calls. Empty states render when combinations produce zero rows. No request to `/api/tasks?page=0` or negative pages is ever issued. |
| **Status** | UNTESTED |

---

### 7.1 User-Journey Traceability Matrix

| Journey ID | Primary persona | Touches ACs | Risk level | Expected status |
|---|---|---|---|---|
| E2E-JOURNEY-01 | New User | AC-10, AC-P1-01, AC-P1-12, AC-P1-14, AC-03, AC-P1-17 | Critical | UNTESTED |
| E2E-JOURNEY-02 | Team Lead + Contributor | AC-03, AC-04, AC-P1-03, AC-P1-05, AC-P1-10, AC-P1-12, AC-P1-16 | Critical | UNTESTED |
| E2E-JOURNEY-03 | Contributor | AC-05, AC-06, AC-P1-02, AC-P1-05, AC-P1-16 | Critical | UNTESTED |
| E2E-JOURNEY-04 | Contributor | AC-05, AC-P1-02, AC-P1-05 | High | UNTESTED |
| E2E-JOURNEY-05 | Contributor → Team Lead | AC-05, AC-P1-02, AC-P1-05, AC-P1-10 | High | UNTESTED |
| E2E-JOURNEY-06 | Team Lead | AC-01, AC-07, AC-P1-10, AC-P1-13 | High | UNTESTED |
| E2E-JOURNEY-07 | Contributor | AC-10, AC-11, AC-12, AC-P1-17 | Critical | UNTESTED |
| E2E-JOURNEY-08 | Returning User | AC-11, AC-14, AC-P1-14 | Critical | UNTESTED |
| E2E-JOURNEY-09 | Returning User | AC-09 | Medium | UNTESTED |
| E2E-JOURNEY-10 | Returning User + New Session | AC-14 | Critical | UNTESTED |
| E2E-JOURNEY-11 | Contributor | AC-P1-06, AC-P1-07, AC-P1-10, AC-P1-13 | Critical | UNTESTED |
| E2E-JOURNEY-12 | Contributor | AC-P1-01, AC-04 | Medium | UNTESTED |
| E2E-JOURNEY-13 | Contributor | AC-01, AC-04, AC-P1-05 | Medium | UNTESTED |
| E2E-JOURNEY-14 | Contributor | AC-P1-01, AC-15 | High | FAIL |
| E2E-JOURNEY-15 | New User | AC-09, AC-10 | Medium | UNTESTED |
| E2E-JOURNEY-16 | Contributor | AC-01, AC-08, AC-P1-05 | High | UNTESTED |

### 7.2 How These Complement the Existing Suite

The endpoint-oriented cases in §1–§4 remain necessary: they own the **strict contract** assertions (status codes, response shapes, exact error bodies) that protect against silent regression. The journey cases in this section are their complement — they own the **composition** assertions that an endpoint-oriented suite cannot express:

* Multi-role handoffs (E2E-JOURNEY-02, E2E-JOURNEY-05).
* Cross-session state (E2E-JOURNEY-07, E2E-JOURNEY-08, E2E-JOURNEY-10).
* UI-driven state propagation across views (E2E-JOURNEY-03, E2E-JOURNEY-04, E2E-JOURNEY-13).
* Recovery and resilience loops (E2E-JOURNEY-09, E2E-JOURNEY-14, E2E-JOURNEY-15).

**Execution ordering recommendation.** Run strict-contract suites first (they fail fast and cheaply). Follow with the journey suites — a passing contract suite is a prerequisite, but journey failures typically reveal glue-layer bugs the endpoint suites cannot see.

**Test-harness notes.** Several journeys (02, 05, 10) require two authenticated sessions. Playwright supports this via `browser.newContext()` per persona. Journeys 07 and 08 require token-expiry manipulation; use a test helper that writes a synthetic `expiresAt` to localStorage and the auth cookie rather than waiting in real time.
