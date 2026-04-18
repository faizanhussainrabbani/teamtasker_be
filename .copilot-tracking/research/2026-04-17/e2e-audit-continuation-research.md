<!-- markdownlint-disable-file -->
---
title: E2E Audit Continuation Research
description: Research baseline for continuing all follow-up actions from the E2E audit update.
author: GitHub Copilot
ms.date: 2026-04-17
ms.topic: reference
---

## Scope

Continue all follow-up items from the E2E audit context, mapped to actionable implementation:

1. Land new E2E cases and status flips into [teamtasker_be/docs/testing/e2e_test_cases.md](teamtasker_be/docs/testing/e2e_test_cases.md).
2. Re-verify E2E-TASK-10 runtime behavior before demoting FAIL.
3. Harmonize auth error shape drift in backend auth flows.
4. Resolve missing profile-subresource runtime calls or remove dependent frontend hooks.
5. Wire team workload view to real users/tasks APIs.
6. Ensure strict E2E CI gating exists.

## Assumptions

* `continue=all` refers to the prior actionable follow-up set in the current session context.
* Both repos are active and dirty; edits must be surgical and non-destructive.
* Runtime validation can be attempted because `node`, `npm`, and `pnpm` are available.

## Evidence Log

* [teamtasker_be/docs/testing/e2e_test_cases.md](teamtasker_be/docs/testing/e2e_test_cases.md): master E2E suite still at pre-audit state.
* [teamtasker_be/docs/testing/e2e_test_cases_audit.md](teamtasker_be/docs/testing/e2e_test_cases_audit.md): contains new case drafts and status update recommendations.
* [teamtasker_be/src/Presentation/TeamTasker.API/Controllers/AuthController.cs](teamtasker_be/src/Presentation/TeamTasker.API/Controllers/AuthController.cs): mixed error shape responses remain on some negative paths.
* [teamtasker_be/src/Presentation/TeamTasker.API/Controllers/UsersController.cs](teamtasker_be/src/Presentation/TeamTasker.API/Controllers/UsersController.cs): no `/profile/skills` or `/profile/development-goals` routes.
* [teamtasker_fe/components/tasks/task-dialog.tsx](teamtasker_fe/components/tasks/task-dialog.tsx): create/update wiring is present.
* [teamtasker_fe/components/team/team-workload-view.tsx](teamtasker_fe/components/team/team-workload-view.tsx): still uses static mock data.
* [teamtasker_fe/components/dashboard/development-goals-card.tsx](teamtasker_fe/components/dashboard/development-goals-card.tsx): still calls missing backend goals endpoint.
* Runtime toolchain check: `node v25.9.0`, `npm 11.12.1`, `pnpm 10.33.0` available.

## Alternatives Evaluated

### Alternative A: Docs-only continuation

* Pros: low risk, fast.
* Cons: does not actually reduce broken runtime flows.

### Alternative B: Full cross-layer continuation (selected)

* Pros: aligns with `continue=all`, addresses docs + code + runtime verification.
* Cons: broader surface and validation complexity.

## Selected Approach

Execute full cross-layer continuation with explicit ordering:

1. Update master E2E cases doc first to align canonical expectations.
2. Fix backend auth error-shape inconsistencies.
3. Remove/neutralize frontend calls to known-missing profile subresource endpoints.
4. Wire team workload to live APIs.
5. Attempt runtime verification for E2E-TASK-10 and run targeted frontend checks.
6. Add/confirm strict contract CI gate.

## Next Actions

* Create implementation plan/checklist.
* Implement in small, reviewable patches per repository.
* Validate with targeted commands and capture outcomes.
