---
title: P1 Correctness and Silent UX Bugs Acceptance Criteria
description: Step 3 measurable acceptance criteria for P1 correctness and silent UX remediation with plan traceability.
author: TeamTasker Engineering
ms.date: 2026-04-16
ms.topic: reference
keywords:
  - acceptance criteria
  - p1 validation
  - pass fail checks
estimated_reading_time: 9
---

## Acceptance Criteria

| ID | Criterion | Plan traceability | Validation method |
|---|---|---|---|
| AC-P1-01 | User list and user detail API payloads include `name` in addition to existing full-name fields | WS1 | DTO and controller mapping inspection plus user endpoint contract check |
| AC-P1-02 | Task transport maps domain statuses to explicit client strings that include all backend states (`todo`, `in-progress`, `completed`, `blocked`, `on-hold`, `cancelled`) | WS2 | Mapper inspection and unit tests |
| AC-P1-03 | Task transport maps domain priorities to explicit client strings including `critical` | WS2 | Mapper inspection and unit tests |
| AC-P1-04 | Frontend task status and priority types accept all transport values returned by backend | WS2 | Type definition inspection and diagnostics check |
| AC-P1-05 | Task list and kanban views render tasks with expanded status values without dropping records | WS2 | UI logic inspection and status-grouping check |
| AC-P1-06 | API client dispatches a dedicated forbidden event for HTTP 403 responses | WS4 | Interceptor code inspection |
| AC-P1-07 | Global API error listener surfaces a visible toast for forbidden events | WS4 | Listener code inspection |
| AC-P1-08 | API base URL and timeout values are normalized across frontend config layers | WS5 | Config and client inspection |
| AC-P1-09 | Stale mock reference to port `3002` is removed from shipped frontend API README and legacy `proxy.js` is removed | WS5 | File content and file existence check |
| AC-P1-10 | Backend write endpoints in scope include explicit role-aware authorization attributes | WS3 | Controller attribute inspection |
| AC-P1-11 | JWT bearer configuration explicitly maps role claim type used for role-based authorization checks | WS3 | Infrastructure auth configuration inspection |
| AC-P1-12 | Auth response contract includes role context used by frontend for role-gated UI actions | WS3 | Auth response model and auth context mapping inspection |
| AC-P1-13 | Admin-only frontend actions are gated client-side based on authenticated user role | WS3 | Task UI action rendering logic inspection |
| AC-P1-14 | Middleware enforces protected-route redirect for unauthenticated requests and auth-page redirect for authenticated requests | WS4 | Middleware logic inspection |
| AC-P1-15 | Unsupported frontend user create update delete endpoint wrappers and hooks are removed from exported API hooks surface | WS6 | Users endpoint and hook inspection |
| AC-P1-16 | Task mutation hooks invalidate activity query keys so activity-dependent views can refresh after task changes | WS6 | Task hook mutation success-path inspection |
| AC-P1-17 | Token-expiry contract support (`expiresAt`) remains available end-to-end after P1 changes | WS6, WS7 | Auth model and usage inspection |
| AC-P1-18 | Targeted backend tests for task mapper behavior pass with updated enum expectations | WS7 | `dotnet test` targeted run result |
| AC-P1-19 | Backend solution builds successfully after P1 implementation | WS7 | `dotnet build` result |
| AC-P1-20 | All P1 execution artifacts comply with required standards references and trace to this plan | WS7 | Document review against standards |

## Pass Criteria for Completion

All acceptance criteria from AC-P1-01 through AC-P1-20 must pass. Any failed criterion must be fixed and re-validated before closure.
