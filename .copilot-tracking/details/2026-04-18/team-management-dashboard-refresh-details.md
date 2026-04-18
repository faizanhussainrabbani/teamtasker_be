<!-- markdownlint-disable-file -->
---
title: Team Management Dashboard Refresh Details
description: Phase-by-phase implementation details for auth signal fixes, team feature completion, and dashboard refresh.
author: GitHub Copilot
ms.date: 2026-04-18
ms.topic: reference
---

## Context References

* Plan: [.copilot-tracking/plans/2026-04-18/team-management-dashboard-refresh-plan.instructions.md](../../plans/2026-04-18/team-management-dashboard-refresh-plan.instructions.md)
* Research: [.copilot-tracking/research/2026-04-18/team-management-dashboard-refresh-research.md](../../research/2026-04-18/team-management-dashboard-refresh-research.md)
* Planning log: [.copilot-tracking/plans/logs/2026-04-18/team-management-dashboard-refresh-log.md](../../plans/logs/2026-04-18/team-management-dashboard-refresh-log.md)

## Phase A Details

Target files:

* [teamtasker_fe/lib/api/hooks/useAuth.ts](../../../teamtasker_fe/lib/api/hooks/useAuth.ts)
* [teamtasker_fe/lib/api/client.ts](../../../teamtasker_fe/lib/api/client.ts)

Steps:

1. Make current-user query conditional on token existence.
2. Limit 401 unauthorized event dispatch to authenticated-request contexts.
3. Keep destructive token-clear behavior only for true authenticated 401 cases.

Success checks:

* No session-expired toast on anonymous bootstrap.
* Session-expired flow still occurs for invalid/expired authenticated calls.

## Phase B Details

Target files:

* [teamtasker_fe/lib/api/types/teams.ts](../../../teamtasker_fe/lib/api/types/teams.ts)
* [teamtasker_fe/lib/api/endpoints/teams.ts](../../../teamtasker_fe/lib/api/endpoints/teams.ts)
* [teamtasker_fe/lib/api/hooks/useTeams.ts](../../../teamtasker_fe/lib/api/hooks/useTeams.ts)

Steps:

1. Introduce teams DTO/query/request types.
2. Add endpoint functions for team and member operations.
3. Add/extend React Query hooks for teams list/detail/mutations.
4. Preserve compatibility with existing team workload consumers.

Success checks:

* Team hooks return normalized shape consumable by UI.
* Mutations invalidate/refetch correct cache keys.

## Phase C Details

Target files:

* [teamtasker_fe/components/team/team-overview.tsx](../../../teamtasker_fe/components/team/team-overview.tsx)
* [teamtasker_fe/components/team/team-management-view.tsx](../../../teamtasker_fe/components/team/team-management-view.tsx)
* [teamtasker_fe/components/team/team-dialog.tsx](../../../teamtasker_fe/components/team/team-dialog.tsx)
* [teamtasker_fe/app/team/page.tsx](../../../teamtasker_fe/app/team/page.tsx)

Steps:

1. Add team management view with list/search/filter and detail panel.
2. Add create/update dialog wired to mutations.
3. Add delete and member add/remove controls with admin guardrails.
4. Integrate into Team page tabs while retaining analytics tabs.

Success checks:

* Team view supports create/update/delete from UI.
* Team detail and membership updates render correctly.

## Phase D Details

Target files:

* [teamtasker_fe/app/(dashboard)/dashboard/page.tsx](../../../teamtasker_fe/app/(dashboard)/dashboard/page.tsx)

Steps:

1. Remove Skills Overview card usage.
2. Recompose dashboard layout for My Tasks, Team Workload, Development Goals.
3. Keep loading skeleton and auth-state behavior consistent.

Success checks:

* Dashboard visibly contains only requested three focus cards.
* Responsive behavior remains valid on desktop/mobile.

## Phase E Details

Validation commands:

1. `cd teamtasker_fe && pnpm exec tsc --noEmit --pretty false`
2. `cd teamtasker_fe && pnpm lint`
3. Targeted runtime API and app-route checks for login + teams + dashboard.

Success checks:

* No new errors introduced in touched files.
* Runtime sanity confirms auth behavior and team workflows.
