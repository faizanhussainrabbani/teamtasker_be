<!-- markdownlint-disable-file -->
---
title: Team Management Dashboard Refresh Plan
description: Execution plan for team CRUD frontend completion, auth-session stabilization, and dashboard composition update.
author: GitHub Copilot
ms.date: 2026-04-18
ms.topic: how-to
---

## User Requests

1. Resolve observed unauthorized-session behavior that appears during login attempts.
2. Add Team feature support from frontend and backend perspective with view, creation, updation, and deletion workflows.
3. Remove Skills Overview from dashboard and reorganize My Tasks, Team Workload, and Development Goals.
4. Continue all selected work (`continue=all`) for this newly identified scope.

## Overview

Use existing backend team APIs and implement missing frontend integration and UX workflows, while fixing auth false positives and dashboard composition.

## Objectives

1. Prevent false unauthorized alerts when no authenticated session exists yet.
2. Expose complete team API consumption layer in frontend.
3. Deliver end-to-end Team management UI in frontend.
4. Update dashboard layout to requested content model.
5. Validate changed behavior with targeted checks.

## Context Summary

Primary references:

* [.copilot-tracking/research/2026-04-18/team-management-dashboard-refresh-research.md](../../research/2026-04-18/team-management-dashboard-refresh-research.md)
* [teamtasker_fe/lib/api/client.ts](../../../teamtasker_fe/lib/api/client.ts)
* [teamtasker_fe/lib/api/hooks/useAuth.ts](../../../teamtasker_fe/lib/api/hooks/useAuth.ts)
* [teamtasker_be/src/Presentation/TeamTasker.API/Controllers/TeamsController.cs](../../../teamtasker_be/src/Presentation/TeamTasker.API/Controllers/TeamsController.cs)

Discovered instructions:

* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/commit-message.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/commit-message.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/git-merge.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/git-merge.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/markdown.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/markdown.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/writing-style.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/hve-core/writing-style.instructions.md)
* [/.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/shared/hve-core-location.instructions.md](../../../../.vscode/extensions/ise-hve-essentials.hve-core-3.2.2/.github/instructions/shared/hve-core-location.instructions.md)

## Implementation Checklist

### Phase A: Auth Signal Stabilization <!-- parallelizable: false -->

- [x] Gate current-user fetch behind token presence.
- [x] Prevent unauthorized event dispatch for unauthenticated bootstrap calls.
- [x] Preserve existing true session-expiry behavior.

### Phase B: Team API Layer Completion <!-- parallelizable: true -->

- [x] Add teams API types.
- [x] Add teams endpoint client functions.
- [x] Extend teams hooks for list/detail/create/update/delete/member add/remove.
- [x] Keep existing team workload hook behavior compatible.

### Phase C: Team Management UI <!-- parallelizable: false -->

- [x] Build Team management view with list/detail/search/filter.
- [x] Add create/edit team dialog.
- [x] Add team delete action and member add/remove controls.
- [x] Wire UI into team page tabs/navigation while preserving workload analytics views.

### Phase D: Dashboard Composition Refresh <!-- parallelizable: true -->

- [x] Remove Skills Overview card usage from dashboard page.
- [x] Reorganize dashboard layout around My Tasks, Team Workload, Development Goals.
- [x] Keep loading skeletons and auth guards coherent with new layout.

### Phase E: Validation and Tracking <!-- parallelizable: false -->

- [x] Run targeted frontend checks for changed files/surfaces.
- [x] Run targeted runtime sanity checks for auth + teams + dashboard load.
- [x] Update changes log and review log.

## Planning Log

* [.copilot-tracking/plans/logs/2026-04-18/team-management-dashboard-refresh-log.md](../logs/2026-04-18/team-management-dashboard-refresh-log.md)

## Dependencies

* No applicable skill package required.
* Runtime dependencies: backend API on `:5220`, frontend app on `:3000`.

## Success Criteria

1. Team CRUD is usable in frontend using backend teams routes.
2. Unauthorized false positives are removed without regressing real session-expiry handling.
3. Dashboard no longer includes Skills Overview and reflects requested three sections.
4. Validation commands for modified surfaces pass or are clearly reported with residual non-scope issues.
