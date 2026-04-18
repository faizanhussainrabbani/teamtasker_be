<!-- markdownlint-disable-file -->
---
title: E2E Audit Continuation Details
description: Execution details by phase for the E2E audit continuation work.
author: GitHub Copilot
ms.date: 2026-04-17
ms.topic: reference
---

## Context References

* Plan: [.copilot-tracking/plans/2026-04-17/e2e-audit-continuation-plan.instructions.md](../../plans/2026-04-17/e2e-audit-continuation-plan.instructions.md)
* Research: [.copilot-tracking/research/2026-04-17/e2e-audit-continuation-research.md](../../research/2026-04-17/e2e-audit-continuation-research.md)
* Planning log: [.copilot-tracking/plans/logs/2026-04-17/e2e-audit-continuation-log.md](../../plans/logs/2026-04-17/e2e-audit-continuation-log.md)

## Phase A Details

Files targeted:

* [teamtasker_be/docs/testing/e2e_test_cases.md](../../../teamtasker_be/docs/testing/e2e_test_cases.md)

Steps:

1. Apply audited status changes and stale-marker wording.
2. Insert new case blocks from continuation set with sequential IDs.
3. Update matrix and priority sections.

Success checks:

* Document structure remains valid markdown with frontmatter.
* IDs are unique and traceability references are coherent.

## Phase B Details

Files targeted:

* [teamtasker_be/src/Presentation/TeamTasker.API/Controllers/AuthController.cs](../../../teamtasker_be/src/Presentation/TeamTasker.API/Controllers/AuthController.cs)

Steps:

1. Normalize negative-path response payloads.
2. Keep existing successful responses unchanged.

Success checks:

* Build compiles for affected project.
* Relevant auth negatives emit structured payloads.

## Phase C Details

Files targeted:

* [teamtasker_fe/components/dashboard/development-goals-card.tsx](../../../teamtasker_fe/components/dashboard/development-goals-card.tsx)
* [teamtasker_fe/lib/api/hooks/useUsers.ts](../../../teamtasker_fe/lib/api/hooks/useUsers.ts)
* [teamtasker_fe/lib/api/endpoints/users.ts](../../../teamtasker_fe/lib/api/endpoints/users.ts)
* [teamtasker_fe/components/team/team-workload-view.tsx](../../../teamtasker_fe/components/team/team-workload-view.tsx)

Steps:

1. Remove/disable runtime fetch paths to missing profile subresource routes.
2. Rebuild team workload view around users/tasks hooks and computed workload.

Success checks:

* Frontend type-check/lint passes for changed files.
* No `/api/teams` dependency added.

## Phase D Details

Validation commands (targeted):

1. Backend build or targeted tests for auth controller changes.
2. Frontend lint/type checks.
3. `pnpm exec playwright test e2e/task-dialog.runtime.spec.ts`.
4. `pnpm exec playwright test e2e/auth.contract.spec.ts`.
5. `pnpm exec playwright test e2e/surface-contract.runtime.spec.ts`.

Success checks:

* Runtime evidence captured as pass for task dialog, auth strict contract, and team/development/profile surface coverage.

## Phase E Details

Files targeted (if needed):

* [teamtasker_fe/package.json](../../../teamtasker_fe/package.json)
* [teamtasker_fe/.github/workflows](../../../teamtasker_fe/.github/workflows)

Steps:

1. Confirm strict E2E command availability.
2. Add minimal CI entry if missing.

Success checks:

* CI path exists and references strict contract suite.
