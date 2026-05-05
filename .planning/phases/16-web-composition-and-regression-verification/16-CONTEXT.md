# Phase 16: Web Composition And Regression Verification - Context

**Gathered:** 2026-05-05
**Status:** Ready for planning

<domain>
## Phase Boundary

Verify the milestone boundary work from Phases 13–15 is structurally enforced and all test layers pass cleanly. Specifically:
1. Add an ArchUnit rule guarding that Web component code-behind classes do not directly depend on Domain or Infrastructure types
2. Extend `WebStartupIntegrationTests` to verify all 6 new Web-layer workflow seam services resolve from DI
3. No changes to `Program.cs` or `ServiceCollectionExtension.cs` — composition root is already wiring-focused (WEB-04 satisfied)
4. Run all test suites (build, architecture, host integration, component tests) to confirm WEB-06 regression checks pass

</domain>

<decisions>
## Implementation Decisions

### A: Architecture Rule (ArchUnit)

- **D-01:** Add a new `[Fact]` to `DependencyGuardrailTests.cs` enforcing that types in the `Ray.BiliBiliTool.Web.Components` namespace do NOT directly depend on `Ray.BiliBiliTool.Domain` or `Ray.BiliBiliTool.Infrastructure` (any sub-namespace). This is namespace-level granularity — `ServiceCollectionExtension.cs` and `Program.cs` are outside `Components` and are unaffected.
- **D-02:** Rule granularity is by namespace: `Components.Pages.*` cannot depend on Domain/Infrastructure directly. Not assembly-wide.
- **D-03:** `.Because(...)` text: `"Web component code-behind classes must route Domain and Infrastructure access through Web-layer workflow seams (Phases 13–15)"`

### B: Integration Test Extension

- **D-04:** Extend `WebStartupIntegrationTests.Web_startup_boots_and_exposes_critical_services` to also resolve all 6 Web-layer workflow seam services: `IAuthService`, `ILoginPageStateFactory`, `IAdminPageWorkflow`, `ISchedulerPageWorkflow`, `ILogsDialogWorkflow`, `IHistoryDialogWorkflow`.
- **D-05:** Append as additional `GetRequiredService<IXxx>()` lines in the **existing** test method (not a new method).

### C: Startup / Composition Root

- **D-06:** WEB-04 is already satisfied. `Program.cs` and `ServiceCollectionExtension.cs` require no changes. Phase 16 is verify-only for the composition root. Planner must NOT change startup code unless a genuine gap is found during build/test execution.

### Agent's Discretion

- Planner decides whether the ArchUnit rule uses `Classes()` or `Types()` as the ArchUnit selector — whichever produces a passing rule given the ArchUnit version in use.
- Planner decides exact `ResideInNamespace` predicate (e.g., `"Ray.BiliBiliTool.Web.Components"` with `includeSubNamespaces: true` if the API supports it, or `.Or()` chaining per sub-namespace).
- Order of test suites in the regression run is agent's choice.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

- `.planning/REQUIREMENTS.md` — WEB-04 (composition root) and WEB-06 (regression) definitions
- `.planning/ROADMAP.md` — Phase 16 goal and success criteria
- `test/Ray.BiliBiliTool.ArchitectureTests/DependencyGuardrailTests.cs` — existing ArchUnit rules to extend
- `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs` — integration test to extend
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` — all 6 workflow seam DI registrations (reference only, no changes)
- `test/Ray.BiliBiliTool.Web.ComponentTests/` — component test suite (regression target)

</canonical_refs>

<deferred>
## Deferred Ideas

None raised during this discussion.
</deferred>
