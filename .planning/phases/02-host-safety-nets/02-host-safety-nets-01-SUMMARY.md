---
phase: 02-host-safety-nets
plan: 01
subsystem: testing
tags: [xunit, host-integration, characterization, webapplicationfactory, smoke-tests]
requires: []
provides:
  - Dedicated characterization test project for Phase 2 baseline tests
  - Dedicated host integration test project for Web and Console startup-path checks
  - Reusable in-memory log collector and minimal host boot fixtures for later Phase 2 plans
affects: [phase-2-safety-nets, login, dailytask, host-composition, web-startup, console-startup]
tech-stack:
  added: [Microsoft.AspNetCore.Mvc.Testing]
  patterns: [dedicated test harnesses, WebApplicationFactory fixture, console host wrapper, smoke-test-first scaffold]
key-files:
  created:
    - test/Ray.BiliBiliTool.CharacterizationTests/Ray.BiliBiliTool.CharacterizationTests.csproj
    - test/Ray.BiliBiliTool.CharacterizationTests/Usings.cs
    - test/Ray.BiliBiliTool.CharacterizationTests/Support/TestLogCollector.cs
    - test/Ray.BiliBiliTool.CharacterizationTests/CharacterizationHarnessSmokeTests.cs
    - test/Ray.BiliBiliTool.Host.IntegrationTests/Ray.BiliBiliTool.Host.IntegrationTests.csproj
    - test/Ray.BiliBiliTool.Host.IntegrationTests/Usings.cs
    - test/Ray.BiliBiliTool.Host.IntegrationTests/Support/ConsoleHostBuilder.cs
    - test/Ray.BiliBiliTool.Host.IntegrationTests/Support/WebHostFactory.cs
    - test/Ray.BiliBiliTool.Host.IntegrationTests/HostHarnessSmokeTests.cs
  modified:
    - Directory.Packages.props
    - Ray.BiliBiliTool.sln
    - src/Ray.BiliBiliTool.Web/Program.cs
key-decisions:
  - "Created separate characterization and host integration projects instead of extending legacy test folders so Phase 2 safety nets have explicit ownership."
  - "Added a minimal `public partial Program` to the Web host to support `WebApplicationFactory` without introducing an alternate startup path."
  - "Seeded both new projects with smoke tests so `--list-tests` validates real discovery instead of an empty harness."
patterns-established:
  - "Phase 2 baseline work lands in dedicated test projects with minimal reusable support fixtures."
  - "Host startup checks can target the real Console and Web entry paths through thin wrappers rather than ad hoc boot code."
requirements-completed: []
duration: 20 min
completed: 2026-05-03
---

# Phase 2 Plan 01: Host Safety Nets Summary

**Dedicated characterization and host-integration test harnesses for Login, DailyTask, Web startup, and Console startup safety nets**

## Performance

- **Duration:** 20 min
- **Started:** 2026-05-03T00:00:00+08:00
- **Completed:** 2026-05-03T00:20:00+08:00
- **Tasks:** 2
- **Files modified:** 12

## Accomplishments
- Added a dedicated characterization test project with a reusable in-memory log collector and a smoke test to validate harness discovery.
- Added a dedicated host integration test project with Web and Console support fixtures plus smoke tests for DI/service-resolution and Web host factory construction.
- Wired both projects into the solution and central package management, and enabled the Web host to participate in `WebApplicationFactory` through a minimal partial `Program` declaration.

## Task Commits

Each task was committed atomically:

1. **Task 1: Create the dedicated characterization test project** - Not committed in this session
2. **Task 2: Create the dedicated host integration test project** - Not committed in this session

**Plan metadata:** Not committed in this session

## Files Created/Modified
- `test/Ray.BiliBiliTool.CharacterizationTests/Ray.BiliBiliTool.CharacterizationTests.csproj` - Dedicated xUnit project for Phase 2 characterization tests.
- `test/Ray.BiliBiliTool.CharacterizationTests/Support/TestLogCollector.cs` - Reusable in-memory log collector for future diagnostic assertions.
- `test/Ray.BiliBiliTool.CharacterizationTests/CharacterizationHarnessSmokeTests.cs` - Smoke test proving the characterization harness is discovered.
- `test/Ray.BiliBiliTool.Host.IntegrationTests/Ray.BiliBiliTool.Host.IntegrationTests.csproj` - Dedicated xUnit project for host startup and delegation safety checks.
- `test/Ray.BiliBiliTool.Host.IntegrationTests/Support/ConsoleHostBuilder.cs` - Thin wrapper over `Program.CreateHost` for Console integration tests.
- `test/Ray.BiliBiliTool.Host.IntegrationTests/Support/WebHostFactory.cs` - `WebApplicationFactory` scaffold for the real Web startup path.
- `test/Ray.BiliBiliTool.Host.IntegrationTests/HostHarnessSmokeTests.cs` - Smoke tests proving the host harness is discovered and can resolve critical application services.
- `Directory.Packages.props` - Added centralized versioning for `Microsoft.AspNetCore.Mvc.Testing`.
- `src/Ray.BiliBiliTool.Web/Program.cs` - Added a minimal partial `Program` declaration for integration test host bootstrapping.
- `Ray.BiliBiliTool.sln` - Added both new test projects to the solution.

## Decisions Made
- Kept Plan 01 strictly focused on harness creation and smoke-test validation rather than prematurely adding full characterization or startup assertions.
- Preserved the existing Web startup path and exposed it to tests with the smallest possible hook.
- Prepared later diagnostic assertions by adding a reusable log collector now, before any flow-specific tests exist.

## Deviations from Plan

### Auto-fixed Issues

None.

---

**Total deviations:** 0
**Impact on plan:** Plan completed as intended without widening into Phase 2 behavior-freezing or host-delegation assertions.

## Issues Encountered
- The repository did not yet have a centralized version entry for `Microsoft.AspNetCore.Mvc.Testing`, so that had to be added to support the Web host test fixture.
- The Web host used top-level statements only, so a minimal partial `Program` declaration was required for `WebApplicationFactory` compatibility.

## User Setup Required

None - no external service configuration required.

## Verification
- `dotnet test test/Ray.BiliBiliTool.CharacterizationTests/Ray.BiliBiliTool.CharacterizationTests.csproj --list-tests`
- `dotnet test test/Ray.BiliBiliTool.Host.IntegrationTests/Ray.BiliBiliTool.Host.IntegrationTests.csproj --list-tests`

## Next Phase Readiness
- Plan 01 is complete and gives Phase 2 dedicated harnesses for baseline-freezing and host startup assertions.
- Plan 02 can now add explicit Login and DailyTask characterization tests plus diagnostics without revisiting solution/package wiring.
- Plan 03 can now add Web and Console startup-path integration assertions on top of the new host fixtures.

---
*Phase: 02-host-safety-nets*
*Completed: 2026-05-03*