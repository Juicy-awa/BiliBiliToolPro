# Phase 2: Host Safety Nets - Research

**Date:** 2026-05-03
**Status:** Complete
**Source Mode:** Planned without phase context; based on requirements, codebase maps, repository research, and Phase 1 summaries.

## Question

How should Phase 2 freeze Login and DailyTask behavior, validate host startup paths, and add useful diagnostics without turning this phase into the actual Login/DailyTask refactor?

## Findings

### The current high-risk flow entries are already narrow enough to test directly

- Web scheduling enters the critical flows through `LoginJob` and `DailyJob`, each delegating straight to `ILoginTaskAppService` and `IDailyTaskAppService`.
- Console execution enters through `BiliBiliToolHostedService`, which resolves task types from `TaskTypeFactory` and runs `IAppService` implementations through DI.
- `LoginTaskAppService` and `DailyTaskAppService` remain the clearest current behavior seams for Phase 2 characterization because they capture orchestration order before Phase 3 and Phase 4 move internals.

### Existing diagnostics are useful but not yet an intentional safety net

- `TaskInterceptorAttribute` already emits structured task boundary logs with task names and exception messages.
- `BaseJob<TJob>` already attaches Quartz `FireInstanceId` and sink grouping properties to job execution logs.
- These are good baseline signals, but Phase 2 should turn them into deliberate comparison markers around the critical Login and DailyTask paths so later refactors can be judged against stable traces rather than manual reading.

### Existing tests are too thin to serve as the safety net

- Current test projects are layer-sliced and many tests just bootstrap a host or resolve a service.
- There is no dedicated characterization suite and no dedicated host integration suite.
- Reusing the current projects for this phase would blur intent and make it harder to separate baseline-preservation tests from later slice-refactor tests.

### Recommended test harness split

- Create a dedicated characterization test project for Login and DailyTask baseline tests.
- Create a separate host integration test project for Web and Console startup-path checks.
- Keep these test suites focused on current behavior and startup wiring; do not let them absorb Phase 3 or Phase 4 refactor work.

### Recommended characterization strategy

- Characterize `LoginTaskAppService` and `DailyTaskAppService` at the application-entry boundary, not at private method level.
- Use controlled doubles or narrowly configured seams to freeze observable call ordering, gating behavior, and emitted diagnostic markers.
- For `DailyTaskAppService`, the real observable boundary is `BaseMultiAccountsAppService.DoTaskAsync`, so tests should freeze the current multi-account wrapper behavior as well as one-account execution ordering.

### Recommended host integration scope

- Web host: boot the Web app through an integration harness, verify startup wiring, configuration binding, EF bootstrap seam, scheduler registration, and critical service resolution.
- Console host: boot through `Program.CreateHost`, verify configuration-driven task selection path, critical service resolution, and hosted-service orchestration entry behavior.
- Do not hit real Bilibili APIs in these host integration tests; the goal is startup and seam validation, not remote behavior correctness.

### Recommended minimal host changes

- If needed for test hosting, add the standard ASP.NET Core test hook for the Web `Program` type rather than introducing alternate startup paths.
- Preserve the Phase 1 composition seams and use tests to lock them in place.
- Any additional host cleanup in this phase should serve testability and diagnostics only, not a broader refactor.

## Planning Implications

1. Add dedicated Phase 2 test projects first so characterization and host integration can evolve independently.
2. Add explicit diagnostic markers around Login and DailyTask application-entry execution, and assert them in characterization tests.
3. Add host integration tests that prove Web/Console startup paths remain thin delegates over application services and scheduler entry points.

## Risks To Control

- If characterization tests call real external APIs, they will become flaky and stop serving as a safe baseline.
- If Phase 2 host integration tests try to validate every subsystem deeply, they will become slow and block later waves.
- If diagnostics are added too broadly, this phase will create logging churn rather than a focused comparison baseline.

## Recommended Plan Shape

- **Plan 02-01:** Create the dedicated characterization and host integration test harnesses.
- **Plan 02-02:** Freeze Login and DailyTask behavior with characterization tests and explicit diagnostic markers.
- **Plan 02-03:** Add host startup and delegation integration tests for Web and Console against the new seams from Phase 1.

---

*Phase research completed: 2026-05-03*