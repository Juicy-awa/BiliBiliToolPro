---
phase: 01-boundary-guardrails
plan: 02
subsystem: infra
tags: [startup, dependency-injection, quartz, composition-root]
requires:
  - phase: 01-boundary-guardrails
    provides: executable architecture guardrails for dependency direction
provides:
  - Host-local grouping seams for Console and Web module registration
  - Named scheduler registration seam in the Web host
  - Named startup-task seam for Web database initialization
affects: [phase-2-safety-nets, scheduler, web-host, console-host]
tech-stack:
  added: []
  patterns: [host-local grouping seam, named web startup task, scheduler registration wrapper]
key-files:
  created:
    - src/Ray.BiliBiliTool.Console/Extensions/ServiceCollectionExtensions.cs
    - src/Ray.BiliBiliTool.Web/Extensions/WebHostStartupExtensions.cs
  modified:
    - src/Ray.BiliBiliTool.Console/Program.cs
    - src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs
    - src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionQuartzConfiguratorExtensions.cs
    - src/Ray.BiliBiliTool.Web/Program.cs
key-decisions:
  - "Kept the existing technology-layer `Add*` seams visible and wrapped them with host-local grouping methods instead of introducing capability-based modules in Phase 1."
  - "Moved Web startup initialization and Quartz bootstrapping behind named seams without changing routes, schedules, or config keys."
patterns-established:
  - "Hosts compose shared core modules through named local extension methods rather than inline registration sequences."
  - "Web startup side effects are invoked through an async startup extension instead of inline scope resolution and blocking waits."
requirements-completed: [ARCH-03, ARCH-01]
duration: 10 min
completed: 2026-05-02
---

# Phase 1 Plan 02: Boundary Guardrails Summary

**Host-local registration grouping and named Web startup seams over the existing `Add*` module registrations**

## Performance

- **Duration:** 10 min
- **Started:** 2026-05-02T23:49:43+08:00
- **Completed:** 2026-05-02T23:59:11+08:00
- **Tasks:** 3
- **Files modified:** 6

## Accomplishments
- Added a Console-specific grouping seam so the host no longer owns the core module registration sequence inline.
- Extended the Web host with grouped core-module registration, named scheduler setup, and an async startup initialization seam.
- Refactored both `Program.cs` files to consume the grouped composition shape while keeping runtime behavior and existing technology-layer seams intact.

## Task Commits

Each task was committed atomically:

1. **Task 1: Add host-local grouping seams around the current `Add*` registrations** - Not committed in this session
2. **Task 2: Move Web startup side effects behind a named startup-task seam** - Not committed in this session
3. **Task 3: Apply the shared composition order to both host programs** - Not committed in this session

**Plan metadata:** Not committed in this session

## Files Created/Modified
- `src/Ray.BiliBiliTool.Console/Extensions/ServiceCollectionExtensions.cs` - Console host-local grouping seam for core module registration.
- `src/Ray.BiliBiliTool.Web/Extensions/WebHostStartupExtensions.cs` - Async Web startup seam for database/bootstrap initialization.
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` - Shared Web core-module grouping method over existing technology-layer seams.
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionQuartzConfiguratorExtensions.cs` - Named scheduler registration wrapper that keeps `AddBiliJobs` as the job seam.
- `src/Ray.BiliBiliTool.Web/Program.cs` - Refactored composition root using grouped registration and named startup tasks.
- `src/Ray.BiliBiliTool.Console/Program.cs` - Refactored Console composition root using the new host-local grouping seam.

## Decisions Made
- Kept the registration shape conservative: hosts still compose technology-layer seams directly, but now through explicit local grouping methods.
- Used an async startup extension to remove inline `DbInitializer` scope resolution and `.Wait()` from the Web host path.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Restored the Quartz SQLite delegate import after moving scheduler setup**
- **Found during:** Task 2 (Move Web startup side effects behind a named startup-task seam)
- **Issue:** `SQLiteDelegate` was no longer in scope after Quartz setup moved from `Program.cs` into the Web extension.
- **Fix:** Added the missing `Quartz.Impl.AdoJobStore` import to the scheduler extension file.
- **Files modified:** `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionQuartzConfiguratorExtensions.cs`
- **Verification:** `dotnet build Ray.BiliBiliTool.sln -c Debug`
- **Committed in:** Not committed in this session

---

**Total deviations:** 1 auto-fixed (1 blocking compile repair)
**Impact on plan:** The fix was a direct consequence of the planned seam extraction and did not widen scope.

## Issues Encountered
- Moving Quartz bootstrapping into an extension initially dropped one namespace import required by the SQLite job-store setup.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness
- Phase 1 now has executable dependency guardrails plus a clearer host composition shape.
- Phase 2 can add startup and integration safety-net tests against named host seams instead of rediscovering inline wiring.

---
*Phase: 01-boundary-guardrails*
*Completed: 2026-05-02*