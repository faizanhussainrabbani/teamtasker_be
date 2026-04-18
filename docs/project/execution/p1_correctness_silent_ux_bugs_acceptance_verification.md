---
title: P1 Correctness and Silent UX Bugs Acceptance Verification
description: Step 5 criterion-by-criterion verification outcomes for P1 remediation with evidence.
author: TeamTasker Engineering
ms.date: 2026-04-16
ms.topic: reference
keywords:
  - acceptance verification
  - p1 remediation
  - quality gate
estimated_reading_time: 12
---

## Verification Method

Verification used static code inspection, diagnostics checks, and executable backend build and test runs.

Status definitions:

* Pass: criterion is satisfied with direct implementation and or executable evidence
* Fail: criterion failed and required remediation before completion

## Criterion Results

| ID | Status | Evidence | Notes |
|---|---|---|---|
| AC-P1-01 | ✅ Pass | `UserDto` and `UserDetailDto` include `Name`, and `UsersController` maps it from `FullName` in list and detail responses. | Preserves existing full-name fields while restoring frontend `user.name` compatibility. |
| AC-P1-02 | ✅ Pass | `TaskContractMapper.ToClientStatus` now maps `Blocked`, `OnHold`, and `Cancelled` to explicit client status strings; parser supports all client status values. | Prevents hidden status coercion. |
| AC-P1-03 | ✅ Pass | `TaskContractMapper.ToClientPriority` maps `Critical` to `critical` and parser supports it. | Priority no longer collapses `critical` into `high`. |
| AC-P1-04 | ✅ Pass | Frontend `TaskStatus` and `TaskPriority` unions were expanded in task API types. | Type layer accepts backend transport values. |
| AC-P1-05 | ✅ Pass | Task list filters and kanban columns now include `blocked`, `on-hold`, and `cancelled`. | Expanded states render without being dropped from views. |
| AC-P1-06 | ✅ Pass | API client dispatches `FORBIDDEN_EVENT` on HTTP 403. | Centralized forbidden event enables UI feedback. |
| AC-P1-07 | ✅ Pass | API error listener subscribes to forbidden event and shows destructive access-denied toast. | Removes silent role-denied failures. |
| AC-P1-08 | ✅ Pass | API timeout normalized to 15000 in `config.ts`, `constants/app.ts`, and axios client now sources base URL and timeout from `API_CONFIG`. | Single effective source-of-truth behavior for runtime client config. |
| AC-P1-09 | ✅ Pass | `BE APIs/README.md` now references port `5220`; `proxy.js` was removed. | Legacy mock/proxy confusion removed. |
| AC-P1-10 | ✅ Pass | Role-aware authorization attributes added on in-scope write endpoints for tasks and projects, plus explicit role attribute on profile update endpoint. | Backend write paths now explicitly express role policy. |
| AC-P1-11 | ✅ Pass | JWT bearer validation sets `RoleClaimType = "role"` to match minted claim. | Enables `Authorize(Roles=...)` evaluation against token role. |
| AC-P1-12 | ✅ Pass | Auth response model includes `Role`, and auth controller populates role in auth responses including `me` path. | Frontend can enforce role-aware UI decisions. |
| AC-P1-13 | ✅ Pass | Task list delete action is now rendered only for users with admin role from auth context. | Admin-only UI action is gated client-side. |
| AC-P1-14 | ✅ Pass | Middleware now performs auth-route and protected-route redirects using auth cookie presence checks. | Reduces unauthenticated route flash behavior. |
| AC-P1-15 | ✅ Pass | Unsupported user create update delete endpoint wrappers and hooks were removed from frontend API hooks surface. | Frontend no longer exposes non-existent backend write paths. |
| AC-P1-16 | ✅ Pass | Task mutation hooks now invalidate activity query keys in addition to task keys. | Activity-related surfaces can refresh after task mutations. |
| AC-P1-17 | ✅ Pass | `expiresAt` remains in backend auth response and frontend auth types, preserving proactive expiry support. | No regression to token-expiry contract introduced. |
| AC-P1-18 | ✅ Pass | Targeted mapper tests passed: `dotnet test ... --filter "FullyQualifiedName~TaskContractMapperTests"` returned 20 passed, 0 failed. | Includes expanded status and priority mapping assertions. |
| AC-P1-19 | ✅ Pass | `dotnet build TeamTasker.sln` succeeded. | Build completed with warnings only. |
| AC-P1-20 | ✅ Pass | Plan, compliance check, acceptance criteria, verification, and run-validation documents were produced with standards-aligned structure and traceability. | Documentation requirements satisfied. |

## Gate Summary

* Passed: 20 of 20
* Failed: 0 of 20

All P1 acceptance criteria are satisfied.
