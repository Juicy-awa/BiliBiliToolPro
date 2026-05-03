---
phase: 02-host-safety-nets
plan: 03
subsystem: host-integration
tags: [host-integration, startup, delegation, quartz, console, web]
requires:
  - 02-host-safety-nets-01-SUMMARY.md
  - 02-host-safety-nets-02-SUMMARY.md
provides:
  - Web startup integration tests for critical service, scheduler, and EF/bootstrap seam resolution
  - Console startup integration tests for task-selection and security-option binding
  - Thin-host delegation assertions for Console hosted service and Quartz Login/Daily jobs
affects: [web-startup, console-startup, scheduler, login, dailytask, phase-2-safety-nets]
tech-stack:
  added: []
  patterns: [WebApplicationFactory startup checks, console host boot checks, reflection-based Quartz delegation assertions, in-process hosted-service doubles]
key-files:
  created:
    - test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs
    - test/Ray.BiliBiliTool.Host.IntegrationTests/ConsoleStartupIntegrationTests.cs
    - test/Ray.BiliBiliTool.Host.IntegrationTests/HostDelegationSafetyTests.cs
  modified: []
key-decisions:
  - "Kept Web startup assertions focused on service and seam resolution instead of expanding this plan into log-store initialization coverage."
  - "Validated Console delegation by exercising `BiliBiliToolHostedService.StartAsync` with in-process doubles and real task-code selection."
  - "Validated Quartz thin-shell behavior by asserting `LoginJob` and `DailyJob` delegate to application services without introducing a full Quartz scheduler harness."
patterns-established:
  - "Host safety nets can verify startup and delegation behavior without touching real Bilibili APIs."
  - "Thin-host regression checks live beside startup integration tests in the dedicated host integration suite."
requirements-completed: [TEST-02, ARCH-02]
duration: 20 min
completed: 2026-05-03
---

# Phase 2 Plan 03: Host Safety Nets Summary

**Host startup and delegation integration tests for Web, Console, and Quartz job shells**

## Performance

- **Duration:** 20 min
- **Started:** 2026-05-03T00:40:00+08:00
- **Completed:** 2026-05-03T01:00:00+08:00
- **Tasks:** 2
- **Files modified:** 3

## Accomplishments
- Added Web startup integration coverage for critical app-service resolution, scheduler availability, option binding, and EF/bootstrap seam availability.
- Added Console startup integration coverage for `RunTasks` configuration, `SecurityOptions` binding, hosted-service registration, and critical app-service resolution.
- Added thin-host delegation assertions proving `BiliBiliToolHostedService` routes selected tasks through application services and that `LoginJob` and `DailyJob` remain thin shells over `DoTaskAsync()`.

## Task Commits

Each task was committed atomically:

1. **Task 1: Enable real-host integration testing against the Web and Console startup paths** - Not committed in this session
2. **Task 2: Add thin-host delegation safety assertions** - Not committed in this session

**Plan metadata:** Not committed in this session

## Files Created/Modified
- `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs` - Web startup-path tests for service resolution, Quartz registration, options, and EF/bootstrap seam availability.
- `test/Ray.BiliBiliTool.Host.IntegrationTests/ConsoleStartupIntegrationTests.cs` - Console host startup tests for task-selection configuration, security option binding, hosted-service registration, and critical service resolution.
- `test/Ray.BiliBiliTool.Host.IntegrationTests/HostDelegationSafetyTests.cs` - Thin-host delegation tests for the Console hosted service and Quartz Login/Daily jobs.

## Decisions Made
- Kept Web startup verification scoped to startup seams and registrations after `WebApplicationFactory` revealed unrelated log-store side effects when forcing deeper DB initialization.
- Used reflection to invoke the Quartz job shell delegate methods directly because the jobs are intentionally thin and do not consume scheduler context.
- Treated `RunTasks=Login&Daily` execution through `BiliBiliToolHostedService` as the clearest executable proof that Console host logic still delegates through `TaskTypeFactory` and app-service resolution.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Completed the host-environment stub for hosted-service delegation tests**
- **Found during:** Targeted validation of `HostDelegationSafetyTests`
- **Issue:** The initial `FakeHostEnvironment` test double did not implement `ContentRootFileProvider`, causing compilation to fail.
- **Fix:** Added `ContentRootFileProvider` using a `PhysicalFileProvider` rooted at the test app base directory.
- **Files modified:** `test/Ray.BiliBiliTool.Host.IntegrationTests/HostDelegationSafetyTests.cs`
- **Verification:** `dotnet test test/Ray.BiliBiliTool.Host.IntegrationTests/Ray.BiliBiliTool.Host.IntegrationTests.csproj --filter FullyQualifiedName~HostDelegationSafetyTests`
- **Committed in:** Not committed in this session

**2. [Rule 1 - Bug] Narrowed Web startup assertions to seam availability instead of unrelated sink side effects**
- **Found during:** Targeted validation of `WebStartupIntegrationTests`
- **Issue:** Forcing deeper DB initialization under the test host surfaced unrelated SQLite sink table creation failures (`bili_logs`) that are outside this plan's startup/delegation scope.
- **Fix:** Narrowed the Web startup test to validate critical service resolution, Quartz availability, option binding, and EF/bootstrap seam availability by resolving `DbInitializer` and `BiliDbContext` without forcing unrelated log-store setup.
- **Files modified:** `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs`
- **Verification:** `dotnet test test/Ray.BiliBiliTool.Host.IntegrationTests/Ray.BiliBiliTool.Host.IntegrationTests.csproj --filter FullyQualifiedName~Startup`
- **Committed in:** Not committed in this session

---

**Total deviations:** 2 auto-fixed
**Impact on plan:** Both fixes stayed inside the intended host-safety-net slice and kept assertions focused on startup and delegation regressions.

## Issues Encountered
- `WebApplicationFactory` did not automatically support the deeper DB/log-store initialization path the first test version tried to exercise, so startup assertions were narrowed to the explicit seams this plan is meant to protect.
- The hosted-service delegation test needed a fuller `IHostEnvironment` stub than expected to satisfy current framework requirements.

## User Setup Required

None - no external service configuration required.

## Verification
- `dotnet test test/Ray.BiliBiliTool.Host.IntegrationTests/Ray.BiliBiliTool.Host.IntegrationTests.csproj`
- `dotnet test test/Ray.BiliBiliTool.ArchitectureTests/Ray.BiliBiliTool.ArchitectureTests.csproj`

## Next Phase Readiness
- Phase 2 is complete: characterization baselines, diagnostics, and host safety nets are all in place.
- Phase 3 can now refactor Login against stable characterization, host startup, and thin-host delegation protections.

---
*Phase: 02-host-safety-nets*
*Completed: 2026-05-03*