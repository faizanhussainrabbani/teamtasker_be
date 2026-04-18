---
title: P0 Core User Journeys Acceptance Criteria
description: Step 3 measurable acceptance criteria for P0 remediation with traceability to the implementation plan.
author: TeamTasker Engineering
ms.date: 2026-04-15
ms.topic: reference
keywords:
  - acceptance criteria
  - p0 validation
  - pass fail checks
estimated_reading_time: 9
---

## Acceptance Criteria

| ID | Criterion | Plan traceability | Validation method |
|---|---|---|---|
| AC-01 | `GET /api/tasks` returns HTTP 200 and a paginated payload containing `data`, `total`, `page`, `limit`, and `totalPages` | WS1, WS2, Measurable Outcomes | API integration call and payload inspection |
| AC-02 | `GET /api/tasks/{id}` returns HTTP 200 for an existing id and HTTP 404 for a missing id | WS1, WS8 | API integration call for success and not-found edge case |
| AC-03 | `POST /api/tasks` creates a task that is retrievable from `GET /api/tasks/{id}` | WS1, WS8 | API create-then-read flow |
| AC-04 | `PATCH /api/tasks/{id}` updates title, description, priority, and due date fields correctly | WS1, WS8 | API update call followed by detail read |
| AC-05 | `PATCH /api/tasks/{id}/status` accepts frontend status vocabulary and persists mapped domain state | WS1, WS8 | API call and detail verification |
| AC-06 | `PATCH /api/tasks/{id}/progress` clamps and persists progress values according to domain rules | WS1, WS8 | API call with boundary values and detail verification |
| AC-07 | `DELETE /api/tasks/{id}` returns success and subsequent detail call returns not found | WS1, WS8 | API delete-then-read edge-case check |
| AC-08 | Frontend task list and kanban views load without 404 errors and render rows from API `data` | WS1, WS2 | Runtime UI validation |
| AC-09 | Frontend error toasts render non-empty messages when backend returns problem-details or validation errors | WS3, Measurable Outcomes | Runtime error-path validation |
| AC-10 | `POST /api/auth/refresh-token` returns a new access token and a rotated refresh token for valid input | WS4 | API integration call with token comparison |
| AC-11 | Refresh with invalid or expired refresh token is rejected and does not issue new tokens | WS4, WS8 | API negative test |
| AC-12 | Frontend request flow attempts pre-expiry refresh when token is near expiry and keeps session active | WS4, Measurable Outcomes | Runtime session validation |
| AC-13 | Anonymous requests to project create, update, and delete endpoints are rejected by authorization middleware | WS5, Measurable Outcomes | API authorization check |
| AC-14 | Logout clears the full React Query cache so user-specific cached data is unavailable after relogin as another user | WS6, Measurable Outcomes | Runtime cache isolation test |
| AC-15 | Communication and activity UI surfaces no longer issue requests to missing backend endpoints in normal navigation | WS7 | Runtime navigation and network inspection |
| AC-16 | Backend unit and controller tests added for new task and auth refresh behavior pass in CI-equivalent local run | WS8 | `dotnet test` result |
| AC-17 | Frontend build and lint steps complete without new blocking errors introduced by this change set | WS8 | frontend validation command run |
| AC-18 | Implementation and reporting artifacts comply with standards in engineering, FAANG documentation, and testing strategy references | WS8, Completion Definition | Document review against standards |

## Pass Criteria for Completion

All acceptance criteria from AC-01 through AC-18 must pass. Any failed criterion must be fixed and re-validated before closure.
