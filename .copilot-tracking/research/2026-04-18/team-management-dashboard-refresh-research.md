<!-- markdownlint-disable-file -->
---
title: Team Management and Dashboard Refresh Research
description: Research baseline for team management feature completion, dashboard redesign, and auth-session false alarm fix.
author: GitHub Copilot
ms.date: 2026-04-18
ms.topic: reference
---

## Scope

1. Resolve intermittent unauthorized-session signal during valid login flows.
2. Ensure Team feature is complete across backend and frontend:
   * Team view/list and detail
   * Team creation
   * Team update
   * Team deletion
3. Remove dashboard Skills Overview and reorganize My Tasks, Team Workload, and Development Goals.

## Difficulty Assessment

Challenging.

Reasons:

* Cross-cutting frontend auth/session handling.
* New frontend API surface for teams.
* New team-management interaction model with create/update/delete and member operations.
* Dashboard composition changes plus runtime validation impact.

## Evidence Log

* Backend team routes already exist in [teamtasker_be/src/Presentation/TeamTasker.API/Controllers/TeamsController.cs](../../../teamtasker_be/src/Presentation/TeamTasker.API/Controllers/TeamsController.cs):
  * `GET /api/teams`
  * `GET /api/teams/{id}`
  * `POST /api/teams`
  * `PUT /api/teams/{id}`
  * `DELETE /api/teams/{id}`
  * `POST /api/teams/{id}/members`
  * `DELETE /api/teams/{id}/members/{userId}`
* Runtime probe confirms team payload shape includes `items`, `data`, `pageNumber`, `page`, `pageSize`, `limit`, `totalCount`, `total`, `totalPages`.
* Frontend team integration gap:
  * [teamtasker_fe/lib/api/hooks/useTeams.ts](../../../teamtasker_fe/lib/api/hooks/useTeams.ts) currently exposes only workload-computation hook and no team CRUD APIs.
  * [teamtasker_fe/components/team/team-overview.tsx](../../../teamtasker_fe/components/team/team-overview.tsx) is analytics-only with no management workflow.
* Unauthorized false alarm source:
  * [teamtasker_fe/lib/api/hooks/useAuth.ts](../../../teamtasker_fe/lib/api/hooks/useAuth.ts) always queries current user regardless of token state.
  * [teamtasker_fe/lib/api/client.ts](../../../teamtasker_fe/lib/api/client.ts) dispatches unauthorized flow on non-preauth 401s, causing session-expired signals during unauthenticated probes.
* Dashboard mismatch:
  * [teamtasker_fe/app/(dashboard)/dashboard/page.tsx](../../../teamtasker_fe/app/(dashboard)/dashboard/page.tsx) still renders Skills Overview and Recent Activity layout not aligned with user request.

## Alternatives

### Alternative A: Backend + frontend rebuild for team domain

* Pros: maximal control
* Cons: unnecessary, duplicates existing backend functionality

### Alternative B: Reuse backend team APIs and complete frontend feature surface (selected)

* Pros: fastest path, lower risk, aligns with existing architecture
* Cons: requires careful API-shape normalization in frontend

## Selected Approach

1. Fix auth/session false alarms by gating current-user query on token presence and tightening 401 dispatch conditions.
2. Add frontend team API layer (types + endpoints + hooks) over existing backend routes.
3. Introduce team-management UI with list/detail/create/update/delete and member add/remove controls.
4. Recompose dashboard around only My Tasks, Team Workload, and Development Goals.
5. Validate with targeted lint/type and runtime checks.

## Success Criteria

1. Unauthorized/session-expired messaging no longer appears for no-token bootstrap probes.
2. Team management workflow works on frontend against backend routes.
3. Team page supports view, create, update, and delete.
4. Dashboard no longer renders Skills Overview and prioritizes requested three sections.
5. Targeted validations pass for changed surfaces.
