---
title: Implemented E2E Test Case Summary
description: Summary of each implemented Playwright end-to-end test case and the behavior it validates in TeamTasker.
author: TeamTasker Engineering
ms.date: 2026-04-17
ms.topic: reference
keywords:
  - e2e tests
  - playwright
  - coverage summary
  - runtime validation
  - contract testing
standards_references:
  - docs/testing/e2e_test_cases.md
  - docs/testing/e2e_test_cases_audit.md
estimated_reading_time: 18
---

## Scope

This document summarizes the currently implemented Playwright E2E test cases in TeamTasker.

Source of truth:

* `teamtasker_fe/e2e/auth.contract.spec.ts`
* `teamtasker_fe/e2e/tasks.contract.spec.ts`
* `teamtasker_fe/e2e/task-dialog.runtime.spec.ts`
* `teamtasker_fe/e2e/surface-contract.runtime.spec.ts`
* `teamtasker_fe/e2e/security.middleware.spec.ts`
* `teamtasker_fe/e2e/user-journeys.runtime.spec.ts`

## Suite Snapshot

| Metric | Count |
|---|---|
| Total defined test cases | 35 |
| Active runnable test cases | 26 |
| Conditional test cases | 6 |
| Fixme test cases | 3 |

Condition notes:

* Conditional tests run only when role and credential prerequisites are met.
* Fixme tests are placeholders intentionally disabled until missing dependencies are available.

## 1. Auth Contract Enforcement

Source: [teamtasker_fe/e2e/auth.contract.spec.ts](../../../teamtasker_fe/e2e/auth.contract.spec.ts)

| Test case | What this test case achieves | Run state |
|---|---|---|
| register returns HTTP 201 with strict auth payload | Validates successful registration returns status 201 and the exact auth payload contract, including user identity fields and role `User`. | Active |
| login accepts username and email paths with strict payload | Confirms login accepts both username and email identifiers and returns a valid strict auth response in both paths. | Active |
| invalid login must return strict unauthorized error shape | Verifies invalid credentials produce HTTP 401 with strict unauthorized error structure and expected error detail semantics. | Active |
| refresh rotates tokens and replayed token is rejected with strict error shape | Confirms refresh token rotation on success and replay protection by rejecting reused tokens with HTTP 401 strict error payload. | Active |
| auth me returns profile with null token fields | Validates `/auth/me` returns profile data while token, refreshToken, and expiresAt fields remain null in the response contract. | Active |

## 2. Task Contract Enforcement

Source: [teamtasker_fe/e2e/tasks.contract.spec.ts](../../../teamtasker_fe/e2e/tasks.contract.spec.ts)

| Test case | What this test case achieves | Run state |
|---|---|---|
| task list enforces paginated shape and uses search query key | Verifies tasks list pagination contract, validates search filtering behavior, and checks compatibility with both `search` and legacy `searchTerm` query paths. | Conditional |
| out-of-range progress values are rejected with strict validation error shape | Confirms progress values outside valid range are blocked with HTTP 400 and strict validation error structure. | Conditional |
| status and progress transitions remain contract-aligned | Validates task status and progress transitions stay consistent, including auto-transition to completed at 100 and reset to todo at 0. | Conditional |
| tasks with null assignee remain valid contract payloads | Ensures unassigned tasks return contract-valid payloads with null `assignee` and null `assigneeId` on read. | Conditional |

Conditional trigger:

* The suite can be skipped if the runtime environment does not allow task creation for new user tokens and no admin credentials are configured.

## 3. Task Dialog Runtime Verification

Source: [teamtasker_fe/e2e/task-dialog.runtime.spec.ts](../../../teamtasker_fe/e2e/task-dialog.runtime.spec.ts)

| Test case | What this test case achieves | Run state |
|---|---|---|
| create and update flows persist through API | Validates UI-driven create and edit flows through the task dialog, confirms corresponding POST and PATCH responses, and verifies updated titles render in the task surface. | Active |

## 4. Runtime Surface Contracts

Source: [teamtasker_fe/e2e/surface-contract.runtime.spec.ts](../../../teamtasker_fe/e2e/surface-contract.runtime.spec.ts)

| Test case | What this test case achieves | Run state |
|---|---|---|
| team page uses users and tasks APIs, not teams API | Confirms `/team` renders successfully using users and tasks APIs only, while ensuring no call is made to the missing `/api/teams` endpoint. | Active |
| development and dashboard routes avoid missing profile subresource endpoints | Verifies `/development` and `/dashboard` render without calling missing profile subresource endpoints. | Active |
| missing profile subresource endpoints return 404 while profile UI remains stable | Confirms missing profile subresource APIs return 404 directly and that `/profile` still mounts without regressions or hidden calls to missing subresources. | Active |

## 5. Security and Middleware Behavior

Source: [teamtasker_fe/e2e/security.middleware.spec.ts](../../../teamtasker_fe/e2e/security.middleware.spec.ts)

| Test case | What this test case achieves | Run state |
|---|---|---|
| unauthenticated request to protected route is redirected to login | Verifies middleware blocks anonymous access to protected routes and issues redirect to `/login`. | Active |
| authenticated cookie request to auth route is redirected to dashboard | Confirms middleware redirects authenticated cookie sessions away from auth screens to `/dashboard`. | Active |
| localStorage-only token does not bypass middleware guard | Ensures localStorage tokens alone cannot bypass middleware checks that depend on cookie-authenticated route protection. | Active |
| user role can create tasks according to RBAC contract | Validates that a registered user token can create tasks under expected RBAC policy in this environment. | Active |
| non-admin user cannot delete task through direct API call | Confirms RBAC enforcement denies non-admin deletion attempts with HTTP 403 against task delete endpoint. | Conditional |
| admin can delete task when admin credentials are provided | Validates admin-only delete capability succeeds with HTTP 204 when seeded admin credentials are available. | Conditional |

Conditional trigger:

* Admin credential dependent tests require `E2E_ADMIN_USERNAME` and `E2E_ADMIN_PASSWORD`.

## 6. User Journey E2E Suite

Source: [teamtasker_fe/e2e/user-journeys.runtime.spec.ts](../../../teamtasker_fe/e2e/user-journeys.runtime.spec.ts)

| Test case | What this test case achieves | Run state |
|---|---|---|
| E2E-JOURNEY-01: new user onboarding registers, lands on dashboard, and creates first task | Validates end-to-end onboarding from registration to authenticated dashboard access and first task creation via UI flow. | Active |
| E2E-JOURNEY-02: team-lead assigns critical task to contributor (requires seeded Admin) | Placeholder for admin-led assignment flow that will validate role-based assignment journey once seeded admin support is available. | Fixme |
| E2E-JOURNEY-03: contributor progresses a task and status auto-transitions | Confirms progress updates drive expected status changes across todo, in-progress, completed, and reset behavior. | Active |
| E2E-JOURNEY-04: kanban status transitions persist through /status endpoint | Verifies sequential status transitions persist correctly through the dedicated status endpoint. | Active |
| E2E-JOURNEY-05: blocked task is recovered by a second contributor session | Validates multi-session collaboration where one user blocks a task and another resumes and advances it with consistent persisted state. | Active |
| E2E-JOURNEY-06: admin cleans up cancelled tasks (requires seeded Admin) | Placeholder for admin cleanup workflow that will verify admin deletion lifecycle once seeded admin support is available. | Fixme |
| E2E-JOURNEY-07: pre-expiry refresh transparently rotates tokens | Confirms refresh flow rotates tokens before expiry and preserves access to protected endpoints with refreshed credentials. | Active |
| E2E-JOURNEY-08: expired token + replayed refresh forces clean re-login | Verifies replayed refresh tokens are rejected and expired browser-side sessions are routed to login for a clean re-authentication path. | Active |
| E2E-JOURNEY-09: password recovery end-to-end (requires mailbox access) | Placeholder for password reset journey that will validate token retrieval and reset completion once mailbox capture support exists. | Fixme |
| E2E-JOURNEY-10: shared-workstation cache isolation across users | Confirms session isolation across separate browser contexts so one user session state does not leak into another on a shared device model. | Active |
| E2E-JOURNEY-11: non-admin delete attempt is denied at the API boundary | Validates RBAC boundary by denying non-admin task deletion and confirming task remains available afterward. | Active |
| E2E-JOURNEY-12: profile update persists across a reload and a re-login | Verifies profile mutation persistence at API level across follow-up reads and profile page access. | Active |
| E2E-JOURNEY-13: calendar view renders tasks across all statuses | Validates seeded tasks across all statuses are retrievable and visible in task surfaces, with calendar view mounting correctly for date-grid interaction. | Active |
| E2E-JOURNEY-14: /development page loads without calling missing profile subresources | Confirms development page runtime remains stable while avoiding network calls to missing profile subresource endpoints. | Active |
| E2E-JOURNEY-15: registration validation rejects bad input and accepts a valid retry | Verifies registration validation behavior for duplicate email and weak password failures, followed by successful retry path. | Active |
| E2E-JOURNEY-16: pagination + filter + search return scoped results | Validates paginated and filtered task retrieval semantics, including status filtering and keyword search scope correctness. | Active |

## Notes

This summary reflects the currently implemented and discoverable Playwright E2E tests in source code.

It does not restate planned test ideas from design-only documents unless a corresponding executable test case exists in the suite.
