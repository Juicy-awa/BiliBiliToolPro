---
phase: 01-boundary-guardrails
plan: 01
subsystem: testing
tags: [archunitnet, xunit, architecture, dependency-guardrails]
requires: []
provides:
  - Dedicated architecture-test project for Phase 1 boundary enforcement
  - Executable dependency-direction rules for jobs, application, and domain layers
  - Explicit allowlist guard for existing Application-to-Agent DTO leaks
affects: [host-composition, phase-2-safety-nets, login, dailytask]
tech-stack:
  added: [TngTech.ArchUnitNET, TngTech.ArchUnitNET.xUnit]
  patterns: [dedicated architecture test suite, explicit legacy allowlist for transitional boundary leaks]
key-files:
  created:
    - test/Ray.BiliBiliTool.ArchitectureTests/Ray.BiliBiliTool.ArchitectureTests.csproj
    - test/Ray.BiliBiliTool.ArchitectureTests/Usings.cs
    - test/Ray.BiliBiliTool.ArchitectureTests/DependencyGuardrailTests.cs
  modified:
    - Directory.Packages.props
    - Ray.BiliBiliTool.sln
key-decisions:
  - "Used a dedicated architecture-test project instead of mixing guardrails into existing host-boot tests."
  - "Kept Web/Quartz host reach-through rules strict while isolating current Application-to-Agent DTO leaks behind an explicit allowlist."
patterns-established:
  - "Architecture guardrails live in a standalone test project with fast targeted execution."
  - "Existing legacy boundary leaks are captured as explicit allowlists rather than weakening broad rules."
requirements-completed: [ARCH-01, TEST-03]
duration: 30 min
completed: 2026-05-02
---

# Phase 1 Plan 01: Boundary Guardrails Summary

**Dedicated ArchUnitNET guardrails for Quartz jobs, application boundaries, and domain-layer dependency direction**

## Performance

- **Duration:** 30 min
- **Started:** 2026-05-02T23:19:43+08:00
- **Completed:** 2026-05-02T23:49:43+08:00
- **Tasks:** 2
- **Files modified:** 5

## Accomplishments
- Added a dedicated xUnit-based architecture test project and wired it into the solution.
- Introduced executable ArchUnitNET rules for Quartz job reach-through, application-to-web/scheduler dependencies, and domain/domain-service host leakage.
- Captured the current Application-to-Agent DTO leakage as an explicit allowlist so future regressions fail without pretending the legacy state is already clean.

## Task Commits

Each task was committed atomically:

1. **Task 1: Create the dedicated architecture-test project** - Not committed in this session
2. **Task 2: Encode the first boundary contract as executable rules** - Not committed in this session

**Plan metadata:** Not committed in this session

## Files Created/Modified
- `test/Ray.BiliBiliTool.ArchitectureTests/Ray.BiliBiliTool.ArchitectureTests.csproj` - Dedicated architecture-test project with ArchUnitNET xUnit integration.
- `test/Ray.BiliBiliTool.ArchitectureTests/Usings.cs` - Shared xUnit usings for the architecture suite.
- `test/Ray.BiliBiliTool.ArchitectureTests/DependencyGuardrailTests.cs` - First executable dependency guardrail rules and legacy DTO allowlist.
- `Directory.Packages.props` - Central package version entries for ArchUnitNET packages.
- `Ray.BiliBiliTool.sln` - Solution wiring for the new architecture test project.

## Decisions Made
- Used ArchUnitNET as the primary dependency-direction enforcement tool, matching the Phase 1 research direction.
- Preserved a narrow allowlist for known `Application` uses of Agent DTO namespaces rather than weakening the broader boundary rules.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Corrected ArchUnitNET API usage in the new test suite**
- **Found during:** Task 2 (Encode the first boundary contract as executable rules)
- **Issue:** The initial test file used an unsupported `ResideInNamespace` overload and was missing the xUnit extension namespace required for `Check(Architecture)`.
- **Fix:** Switched to the supported overloads and imported `ArchUnitNET.xUnit`.
- **Files modified:** `test/Ray.BiliBiliTool.ArchitectureTests/DependencyGuardrailTests.cs`
- **Verification:** `dotnet test test/Ray.BiliBiliTool.ArchitectureTests/Ray.BiliBiliTool.ArchitectureTests.csproj --filter FullyQualifiedName~DependencyGuardrailTests`
- **Committed in:** Not committed in this session

**2. [Rule 1 - Bug] Narrowed the Application DTO guard to an explicit source allowlist**
- **Found during:** Task 2 (Encode the first boundary contract as executable rules)
- **Issue:** Existing `Application` services still depend on Agent DTO namespaces, causing the broad rule to fail immediately.
- **Fix:** Split the guard into an ArchUnitNET rule for web/scheduler dependencies plus a source-level allowlist test that names the current legacy files explicitly.
- **Files modified:** `test/Ray.BiliBiliTool.ArchitectureTests/DependencyGuardrailTests.cs`
- **Verification:** `dotnet test test/Ray.BiliBiliTool.ArchitectureTests/Ray.BiliBiliTool.ArchitectureTests.csproj`
- **Committed in:** Not committed in this session

---

**Total deviations:** 2 auto-fixed (1 blocking API mismatch, 1 legacy-boundary containment fix)
**Impact on plan:** Both fixes stayed inside the planned boundary-guardrail slice and made the suite truthful without scope creep.

## Issues Encountered
- The first ArchUnitNET implementation used an incorrect API shape and had to be corrected from package documentation.
- The initial Application transport DTO rule exposed a real legacy dependency cluster, which was constrained via an explicit allowlist instead of weakening the whole suite.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness
- Plan 01 is complete and provides a fast architecture suite for subsequent host-composition cleanup.
- Plan 02 can now reshape Web and Console registration seams while reusing this new guardrail suite as regression protection.

---
*Phase: 01-boundary-guardrails*
*Completed: 2026-05-02*