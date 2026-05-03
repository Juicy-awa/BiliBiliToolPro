---
phase: 02-host-safety-nets
plan: 02
subsystem: application-testing
tags: [characterization, diagnostics, login, dailytask, xunit]
requires:
  - 02-host-safety-nets-01-SUMMARY.md
provides:
  - Explicit application-entry diagnostic markers for Login and DailyTask flows
  - Login characterization tests that freeze current orchestration order
  - DailyTask characterization tests that freeze enabled-path sequencing and multi-account continuation behavior
affects: [login, dailytask, phase-2-safety-nets, diagnostics]
tech-stack:
  added: []
  patterns: [logger-based flow markers, hand-written domain-service doubles, direct app-service characterization tests]
key-files:
  created:
    - src/Ray.BiliBiliTool.Application/Diagnostics/TaskFlowDiagnosticScope.cs
    - test/Ray.BiliBiliTool.CharacterizationTests/LoginTaskCharacterizationTests.cs
    - test/Ray.BiliBiliTool.CharacterizationTests/DailyTaskCharacterizationTests.cs
  modified:
    - src/Ray.BiliBiliTool.Application/LoginTaskAppService.cs
    - src/Ray.BiliBiliTool.Application/DailyTaskAppService.cs
key-decisions:
  - "Added a minimal logger-based diagnostic helper instead of introducing a tracing framework or changing flow contracts."
  - "Characterized Login and DailyTask by directly instantiating application services with hand-written doubles rather than booting full hosts or calling remote APIs."
  - "Captured DailyTask multi-account continuation behavior explicitly so later refactors cannot accidentally stop after one failing account."
patterns-established:
  - "Critical flow baselines live in the characterization suite with controlled doubles and explicit diagnostic assertions."
  - "Application-entry flow markers use structured logger scopes plus FlowStart/FlowCompleted/FlowFailed messages."
requirements-completed: [TEST-01, QUAL-02]
duration: 20 min
completed: 2026-05-03
---

# Phase 2 Plan 02: Host Safety Nets Summary

**Characterization tests and explicit diagnostics for the current Login and DailyTask application-entry behavior**

## Performance

- **Duration:** 20 min
- **Started:** 2026-05-03T00:20:00+08:00
- **Completed:** 2026-05-03T00:40:00+08:00
- **Tasks:** 2
- **Files modified:** 5

## Accomplishments
- Added a shared `TaskFlowDiagnosticScope` helper that emits `FlowStart`, `FlowCompleted`, and `FlowFailed` markers around critical application-entry execution.
- Updated `LoginTaskAppService` and `DailyTaskAppService` to emit explicit, testable flow diagnostics without changing their control-flow behavior.
- Added characterization tests that freeze Login orchestration order, DailyTask enabled-path sequencing, and DailyTask multi-account continuation after a failing account.

## Task Commits

Each task was committed atomically:

1. **Task 1: Add explicit diagnostic markers for Login and DailyTask application-entry execution** - Not committed in this session
2. **Task 2: Freeze Login and DailyTask orchestration with characterization tests** - Not committed in this session

**Plan metadata:** Not committed in this session

## Files Created/Modified
- `src/Ray.BiliBiliTool.Application/Diagnostics/TaskFlowDiagnosticScope.cs` - Minimal logger-based flow marker helper for application-entry baselines.
- `src/Ray.BiliBiliTool.Application/LoginTaskAppService.cs` - Wrapped Login execution in explicit flow diagnostics.
- `src/Ray.BiliBiliTool.Application/DailyTaskAppService.cs` - Wrapped DailyTask per-account execution in explicit flow diagnostics.
- `test/Ray.BiliBiliTool.CharacterizationTests/LoginTaskCharacterizationTests.cs` - Freezes Login flow step ordering and asserts task and flow markers.
- `test/Ray.BiliBiliTool.CharacterizationTests/DailyTaskCharacterizationTests.cs` - Freezes DailyTask sequencing, enabled-path behavior, and multi-account continuation semantics.

## Decisions Made
- Used direct app-service instantiation with hand-written doubles to keep the characterization suite deterministic and isolated from host startup and remote APIs.
- Kept the new diagnostics deliberately small and logger-native so they complement the existing `TaskInterceptorAttribute` output instead of replacing it.
- Treated DailyTask multi-account continuation as part of the observable baseline because `BaseMultiAccountsAppService` currently swallows per-account exceptions and proceeds.

## Deviations from Plan

### Auto-fixed Issues

None.

---

**Total deviations:** 0
**Impact on plan:** Plan completed inside the intended slice with no scope creep into host integration tests or refactor work.

## Issues Encountered
- The repository does not include a mocking library, so the characterization suite uses hand-written doubles to keep assertions explicit and dependency-free.
- `TaskInterceptorAttribute` depends on `Global.ServiceProviderRoot`, so the tests had to provide a minimal logging service provider to capture both task-boundary and flow-level diagnostics.

## User Setup Required

None - no external service configuration required.

## Verification
- `dotnet test test/Ray.BiliBiliTool.CharacterizationTests/Ray.BiliBiliTool.CharacterizationTests.csproj`

## Next Phase Readiness
- Plan 02 is complete and locks in the current Login and DailyTask orchestration baseline with explicit diagnostic markers.
- Plan 03 can now focus solely on Web and Console startup/delegation safety checks using the host harness created in Plan 01.

---
*Phase: 02-host-safety-nets*
*Completed: 2026-05-03*