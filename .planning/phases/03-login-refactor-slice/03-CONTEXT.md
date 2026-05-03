# Phase 3: Login Refactor Slice - Context

**Gathered:** 2026-05-03
**Status:** Ready for planning

<domain>
## Phase Boundary

Move the **automation task Login flow** (QR code-based authentication triggered by LoginJob) behind a clearer application boundary that isolates orchestration from domain operations. The refactor preserves all observable behavior frozen by Phase 2 characterization tests while improving the testability and clarity of the Login slice.

**In scope:**
- LoginJob → ILoginTaskAppService → LoginTaskAppService orchestration chain
- QR code login workflow (QrCodeLoginAsync → SetCookiesAsync → SaveCookieAsync)
- Cookie persistence to QingLong or JSON file based on platform
- Integration with ILoginDomainService for domain operations

**Out of scope:**
- Web-based authentication flows (manual cookie entry, OAuth, etc.)
- Other task app services (DailyTask, VipPrivilege, etc.) — handled in Phase 4+
- Changes to ILoginDomainService implementation (domain layer remains unchanged)
- Quartz job scheduling logic cleanup (deferred to Phase 5)

</domain>

<decisions>
## Implementation Decisions

### Scope Boundary
- **D-01:** Login refactor focuses exclusively on the **automation task login flow** triggered by `LoginJob` (QR code authentication). Web-based authentication paths are explicitly out of scope for Phase 3.

### Application Boundary Shape
- **D-02:** **Keep the existing `ILoginTaskAppService` contract** as the application boundary. Existing callers (LoginJob, potential Console commands) continue using this stable contract. Phase 3 work focuses on cleaning up the internal implementation behind this contract, not replacing it.

### the agent's Discretion

The following areas are delegated to planning and implementation agents to decide based on codebase evidence:

- **D-03 (Internal granularity):** Whether to use a single unified flow method vs. multiple explicit step methods within LoginTaskAppService. Decision should be based on current code complexity and characterization test structure.

- **D-04 (Compatibility preservation):** The level of behavioral preservation during refactor (exact compatibility vs. functional equivalence). Agent should be guided by the existing characterization tests from Phase 2 — if tests freeze specific error messages and logging patterns, preserve them; if tests only validate outcomes, functional equivalence is acceptable.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Architecture & Boundaries
- `.planning/codebase/ARCHITECTURE.md` — Module dependency rules, layer boundaries, and host composition patterns from Phase 1
- `.planning/codebase/CONVENTIONS.md` — Coding patterns, naming conventions, and established idioms
- `.planning/codebase/TESTING.md` — Testing approaches, xUnit + FluentAssertions patterns, characterization test guidelines

### Prior Phase Context
- `.planning/phases/01-boundary-guardrails/01-CONTEXT.md` — Locked decisions from Phase 1: dependency direction (D-01), thin hosts (D-02), module registration (D-03), startup tasks (D-06), TaskInterceptor preservation (D-11)
- `.planning/phases/02-host-safety-nets/02-CONTEXT.md` — Locked decisions from Phase 2: characterization test scope (D-01), diagnostic markers (D-03), thin-host delegation (D-04)

### Test Artifacts
- `test/Ray.BiliBiliTool.CharacterizationTests/LoginTaskCharacterizationTests.cs` — Frozen Login behavior baseline established in Phase 2 Plan 02
- `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs` — Host startup validation for Login flow
- `test/Ray.BiliBiliTool.ArchitectureTests/` — Boundary guardrails that must continue passing

</canonical_refs>

<code_context>
## Existing Code Insights

### Current Login Flow Chain

**Trigger:** `src/Ray.BiliBiliTool.Web/Jobs/LoginJob.cs`
- Quartz job that calls `ILoginTaskAppService.DoTaskAsync()`
- Thin delegation shell (Phase 1 established pattern)

**Application Boundary:** `src/Ray.BiliBiliTool.Application.Contracts/ILoginTaskAppService.cs`
- Contract: `interface ILoginTaskAppService : IAppService`
- Current implementation inherits base `DoTaskAsync()` pattern

**Orchestration:** `src/Ray.BiliBiliTool.Application/LoginTaskAppService.cs`
- Three-step workflow:
  1. `QrCodeLoginAsync()` — Gets QR code and waits for scan
  2. `SetCookiesAsync()` — Validates and sets cookie context
  3. `SaveCookieAsync()` — Persists to QingLong env or JSON file based on platform
- Decorated with `[TaskInterceptor]` attributes for logging
- Wrapped in `TaskFlowDiagnosticScope` (Phase 2 addition)

**Domain Operations:** `ILoginDomainService`
- Provides: `LoginByQrCodeAsync()`, `SetCookieAsync()`, `SaveCookieToQinLongAsync()`, `SaveCookieToJsonFileAsync()`
- Domain layer remains untouched in Phase 3

### Established Patterns

**TaskInterceptor pattern** (Phase 1 D-11)
- Logging and telemetry via `[TaskInterceptor("label", TaskLevel)]` attributes
- Must be preserved during refactor

**Diagnostic scopes** (Phase 2 D-03)
- `TaskFlowDiagnosticScope.ExecuteAsync()` wraps critical flows
- Enables before/after comparison of Login behavior

**Platform-aware persistence** (existing pattern)
- `PlatformType.QingLong` → saves to青龙 environment variables
- Other platforms → saves to JSON file
- Configuration-driven branching

### Integration Points

- **Host → Application:** LoginJob delegates to ILoginTaskAppService
- **Application → Domain:** LoginTaskAppService calls ILoginDomainService
- **Configuration:** Reads `PlatformType` from IConfiguration
- **Logging:** ILogger<LoginTaskAppService> with structured logging

</code_context>

<specifics>
## Specific Expectations

- **Characterization tests must continue passing:** The Login refactor cannot change observable behavior frozen by `LoginTaskCharacterizationTests.cs` in Phase 2.
- **Diagnostic markers remain in place:** TaskFlowDiagnosticScope integration must survive the refactor so maintainers can still trace Login flow execution.
- **TaskInterceptor attributes preserved:** These are relied upon for operational telemetry (Phase 1 D-11 lock).
- **No domain service changes:** ILoginDomainService and its implementations remain untouched; Phase 3 only refactors the application orchestration layer.

</specifics>

<deferred>
## Deferred Ideas

- **Web authentication flows:** Manual cookie entry, OAuth, or browser-based login flows are not part of this phase. If web auth needs refactoring, it belongs in a separate phase.
- **Other task app services:** VipPrivilegeTaskAppService, MangaTaskAppService, Silver2CoinTaskAppService also use ILoginDomainService for cookie operations but are out of scope for Phase 3 (addressed in Phase 4+).
- **Quartz job shell cleanup:** Thinning LoginJob further or standardizing job delegation patterns is deferred to Phase 5.
- **Domain service refactoring:** Changes to ILoginDomainService boundaries or implementations are deferred to Phase 6.

</deferred>

---

*Phase: 3-Login Refactor Slice*
*Context gathered: 2026-05-03*
