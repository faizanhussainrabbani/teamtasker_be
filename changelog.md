---
title: TeamTasker Backend Changelog
description: Change history for backend and cross-stack integration remediation work.
author: TeamTasker Engineering
ms.date: 2026-04-16
ms.topic: reference
keywords:
  - changelog
  - p0 remediation
  - integration audit
estimated_reading_time: 6
---

## 2026-04-16

### P0 core user journeys remediation

#### Added

* Implemented tasks API vertical slice with list, detail, create, update, status update, progress update, and delete endpoints.
* Added application-layer task commands, queries, validators, and frontend contract mapper.
* Added refresh token interfaces, request contract, distributed cache token service, and refresh endpoint behavior.
* Added controller and mapper tests for tasks and refresh token behavior.
* Added Step 5 and Step 6 execution artifacts for acceptance verification and run validation reporting.

#### Changed

* Extended auth response contract with refresh token and access-token expiry fields.
* Added frontend-compatible pagination aliases on backend paginated responses.
* Applied authorization to project controller routes.
* Consolidated API exception shaping to the global exception middleware path.
* Updated frontend auth hooks, context, and token refresh logic to prevent recursive refresh flow and clear cache on logout.
* Updated frontend error parser to handle problem-details and validation error dictionaries.
* Deactivated communication and activity UI request paths that depend on missing backend APIs.

#### Validation

* Backend solution build passes.
* P0-targeted backend tests pass.
* Full backend suite has two pre-existing legacy failures outside P0 scope.
* Frontend lint and build execution is blocked in this environment because Node toolchain binaries are unavailable.

#### References

* [P0 implementation plan](docs/project/execution/p0_core_user_journeys_implementation_plan.md)
* [P0 plan compliance check](docs/project/execution/p0_core_user_journeys_plan_compliance_check.md)
* [P0 acceptance criteria](docs/project/execution/p0_core_user_journeys_acceptance_criteria.md)
* [P0 acceptance verification](docs/project/execution/p0_core_user_journeys_acceptance_verification.md)
* [P0 run and validation report](docs/project/execution/p0_core_user_journeys_run_validation_report.md)

### P1 correctness and silent UX bugs remediation

#### Added

* Added frontend-compatible `name` aliases in user list and detail backend DTO contracts.
* Added explicit forbidden event handling in frontend API client and global listener for visible 403 feedback.
* Added middleware route-protection logic for protected routes and authenticated-auth-route redirects.
* Added expanded task status and priority support in frontend task type system and task UI surfaces.
* Added Step 5 and Step 6 execution artifacts for P1 acceptance verification and run validation.

#### Changed

* Updated task transport mapping to preserve full backend status vocabulary and critical priority values.
* Applied role-aware authorization attributes on in-scope write endpoints for projects, tasks, and profile update.
* Configured JWT bearer role claim mapping to support role authorization checks.
* Extended auth response contract with role and mapped role through auth response paths.
* Normalized frontend API timeout and base URL configuration usage.
* Updated frontend auth storage helpers to mirror token presence in cookie for middleware checks.
* Removed unsupported frontend user create update delete endpoint wrappers and hooks.
* Updated task mutation hooks to invalidate activity query keys after task changes.
* Updated stale frontend API documentation base URL from legacy mock port to backend runtime port.

#### Removed

* Removed legacy unused `proxy.js` frontend proxy middleware file.

#### Validation

* Backend solution build passes.
* P1-targeted mapper and controller test runs pass.
* Full backend suite still has two pre-existing legacy failures outside P1 scope.
* Frontend lint and build execution remains blocked in this environment due missing Node toolchain binaries.

#### References

* [P1 implementation plan](docs/project/execution/p1_correctness_silent_ux_bugs_implementation_plan.md)
* [P1 plan compliance check](docs/project/execution/p1_correctness_silent_ux_bugs_plan_compliance_check.md)
* [P1 acceptance criteria](docs/project/execution/p1_correctness_silent_ux_bugs_acceptance_criteria.md)
* [P1 acceptance verification](docs/project/execution/p1_correctness_silent_ux_bugs_acceptance_verification.md)
* [P1 run and validation report](docs/project/execution/p1_correctness_silent_ux_bugs_run_validation_report.md)
