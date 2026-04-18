<!-- markdownlint-disable-file -->
---
title: E2E Audit Continuation Plan
description: Plan to continue all E2E audit follow-up work items across docs, backend, frontend, and validation.
author: GitHub Copilot
ms.date: 2026-04-17
ms.topic: how-to
---

## User Requests

1. Continue with all previously suggested work items (`continue=all`) from prompt invocation.
2. Keep audit corrections accurate for confirmed broken flows and task-dialog re-verification handling.

## Overview

This plan executes the full continuation set by combining documentation updates, backend error-contract hardening, frontend broken-flow remediation, runtime verification, and CI-gate alignment.

## Objectives

1. Make [teamtasker_be/docs/testing/e2e_test_cases.md](teamtasker_be/docs/testing/e2e_test_cases.md) reflect the audited continuation set.
2. Remove known auth error-shape drift for relevant auth failure paths.
3. Eliminate runtime calls to missing profile subresource endpoints unless backend support exists.
4. Wire team workload to real `/api/users` and `/api/tasks` data.
5. Re-verify E2E-TASK-10 runtime behavior and record evidence.
6. Ensure strict E2E CI gate command path is present.

## Context Summary

Instruction files applied:

* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/commit-message.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/commit-message.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/git-merge.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/git-merge.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/markdown.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/markdown.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/writing-style.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/writing-style.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/shared/hve-core-location.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/shared/hve-core-location.instructions.md)

Research reference:

* [.copilot-tracking/research/2026-04-17/e2e-audit-continuation-research.md](../../research/2026-04-17/e2e-audit-continuation-research.md)

## Implementation Checklist

### Phase A: Master E2E Doc Continuation <!-- parallelizable: false -->

- [x] Apply status flips (E2E-AUTH-04, E2E-AUTH-07, E2E-EDGE-11).
- [x] Insert new continuation cases with sequential IDs.
- [x] Update known gaps, traceability matrix, and execution priority.
- [x] Align E2E-TASK-10 wording to runtime re-verification status.

### Phase B: Backend Auth Error Contract <!-- parallelizable: true -->

- [x] Update inconsistent auth negative-path responses to strict shape where applicable.
- [x] Keep status/title/detail semantics aligned with E2E expectations.

### Phase C: Frontend Broken-Flow Remediation <!-- parallelizable: true -->

- [x] Resolve runtime calls to missing `/api/users/profile/development-goals` and `/api/users/profile/skills` surfaces.
- [x] Wire team workload view to `/api/users` and `/api/tasks`.

### Phase D: Validation and Runtime Re-Verification <!-- parallelizable: false -->

- [x] Execute runtime re-verification for E2E-TASK-10 (`e2e/task-dialog.runtime.spec.ts` pass on 2026-04-17).
- [x] Run targeted frontend checks for contract-related surfaces.
- [x] Extend runtime verification to team/development/profile surfaces (`e2e/surface-contract.runtime.spec.ts` pass on 2026-04-17).

### Phase E: CI Gate Alignment <!-- parallelizable: false -->

- [x] Ensure strict E2E command path exists for CI usage.
- [x] Add/update workflow as needed with minimal blast radius.

## Planning Log

* [.copilot-tracking/plans/logs/2026-04-17/e2e-audit-continuation-log.md](../logs/2026-04-17/e2e-audit-continuation-log.md)

## Dependencies

* No additional skill package required for this continuation.
* Runtime prerequisites: backend + frontend services and Playwright dependencies.

## Success Criteria

1. Master E2E doc includes all continuation updates and remains internally consistent.
2. Backend auth failure responses required by continuation return strict structured payloads.
3. Missing profile-subresource runtime calls are removed or neutralized.
4. Team workload view is API-backed and no `/api/teams` dependency is introduced.
5. E2E-TASK-10 runtime verification is executed or explicitly documented as blocked with evidence.
6. CI gate path for strict E2E is present.
