# Phase 6: Integration Boundary And Failure Model - Context

**Gathered:** 2026-05-03
**Status:** Ready for planning

<domain>
## Phase Boundary

Normalize adapter boundaries and typed failure handling around critical integrations so maintainers can distinguish expected business failures, integration failures, and unexpected crashes — and so HTTP, EF, and QingLong dependencies sit behind boundaries that can change without host or flow rewiring.

**In scope:**
- Typed exception hierarchy in `Ray.BiliBiliTool.Domain` (`BiliException` → `BiliBusinessException`, `BiliIntegrationException`, `BiliValidationException`)
- Replace all 22 generic `throw new Exception(...)` in Agent + DomainService layers (except `AuthService.cs`)
- QingLong throws in `LoginDomainService` included in the sweep
- Split Polly resilience policies: read-only clients vs. side-effecting clients
- Explicit 30s timeout on all HTTP clients
- Extract `BiliResiliencePolicies` named class with named constants
- Introduce `IExecutionLogRepository` interface so Razor components and `AuthService` stop reaching `IDbContextFactory<BiliDbContext>` directly
- Targeted unit tests asserting typed exceptions at key throw sites (validates QUAL-01)

**Out of scope:**
- `AuthService.cs` generic throw (Web layer auth — separate concern)
- Notification service boundary (no notification code exists yet)
- Placeholder `INotificationService` interface (deferred to when notifications are built)
- New retry policy behaviors beyond the read-only/side-effecting split

</domain>

<decisions>
## Implementation Decisions

### Failure Type Taxonomy
- **D-01:** Use a **custom exception hierarchy** (not Result\<T\> / discriminated union). Method signatures remain unchanged; callers catch by type.
- **D-02:** Exception types live in `Ray.BiliBiliTool.Domain` — the correct semantic home; accessible from Agent and DomainService without circular references.
- **D-03:** Three-tier hierarchy:
  - `BiliException` — abstract base
  - `BiliBusinessException` — API returned a non-success business code (expected, recoverable)
  - `BiliIntegrationException` — network/external system failure (HTTP errors, timeout, QingLong down)
  - `BiliValidationException` — input/cookie validation failures (bad cookie format, missing required fields)
- **D-04:** **Logging behavior unchanged.** Typed exceptions are for catch-by-type only — do not change log levels or log format. `BiliBusinessException` does NOT get a different log level than current behavior.

### Adapter Normalization Scope
- **D-05:** **Full sweep** — convert all `throw new Exception(...)` in domain service + agent layers. Files: `LoginDomainService`, `AccountDomainService`, `VideoDomainService`, `ArticleDomainService`, `DonateCoinDomainService`, `VipBigPointDomainService`, `LiveDomainService`, `BiliCookie.cs`, `CookieInfo.cs`. Exclude `AuthService.cs`.
- **D-06:** **QingLong throws included** — the two throws in `LoginDomainService` (token fetch failure, env variable query failure) are part of the sweep as `BiliIntegrationException`.
- **D-07:** **Add targeted unit tests** asserting typed exceptions are thrown at key throw sites. Pure validation logic (`BiliCookie.cs`, `CookieInfo.cs`) is the primary target — these are cheapest to test without integration setup.

### Resilience Policy Design
- **D-08:** **Split policies** — two named policies:
  - Read-only/idempotent clients: current retry (1 retry, 2s backoff) retained
  - Side-effecting clients (coin donation, charge, live sign-in): no retry, or retry only on network failure (not on 5xx)
- **D-09:** **Explicit 30s timeout** on all HTTP clients. Replaces the implicit 100s default. A hung request throws diagnosable `TaskCanceledException` instead of silently blocking.
- **D-10:** Extract to **`BiliResiliencePolicies`** static class with named retry count, backoff, and timeout constants. Tests can reference constants to verify policy configuration without spinning up real HTTP endpoints.

### EF Adapter Boundary
- **D-11:** Introduce **`IExecutionLogRepository`** interface. `LogsDialog.razor.cs` and `AuthService` stop depending on `IDbContextFactory<BiliDbContext>` directly — they depend on the interface instead. Implementation in `Infrastructure.EF`.
- **D-12:** **Notifications out of scope** — no notification code exists in the codebase; nothing to normalize. No placeholder interface. Future notification work builds behind a proper boundary from day one.

### the agent's Discretion
- Exact naming of `BiliResiliencePolicies` constants (e.g. `ReadOnlyRetryPolicy`, `MutatingPolicy`) — use clear, self-documenting names.
- Which specific DomainService throws classify as `BiliBusinessException` vs `BiliIntegrationException` — classify based on throw site semantics: API business code failures → Business; network/external failures → Integration; cookie/input failures → Validation.
- Whether `IExecutionLogRepository` needs more than the methods currently called from `LogsDialog.razor.cs` and `AuthService` — keep to minimal needed surface.
- Unit test placement — add to existing `DomainServiceTest` project or create a focused Agent test project as appropriate.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Project And Scope
- `.planning/PROJECT.md` — project-level refactor goals, brownfield constraints, and out-of-scope boundaries.
- `.planning/REQUIREMENTS.md` — requirements for this phase: `ARCH-04`, `FLOW-04`, `QUAL-01`.
- `.planning/ROADMAP.md` — Phase 6 goal, depends-on chain, and success criteria.
- `.planning/STATE.md` — current project position (Phase 5 complete, Phase 6 next).

### Codebase Maps
- `.planning/codebase/ARCHITECTURE.md` — layer responsibilities, dependency wiring pattern, integration architecture section.
- `.planning/codebase/CONVENTIONS.md` — established coding patterns and naming conventions.
- `.planning/codebase/CONCERNS.md` — documents the generic `Exception` throw pattern and retry policy shallowness that Phase 6 addresses.

### Prior Phase Context
- `.planning/phases/01-boundary-guardrails/01-CONTEXT.md` — locked dependency direction rules; exception types in Domain must not violate these.
- `.planning/phases/05-scheduler-shell-cleanup/05-CONTEXT.md` — Quartz jobs are now thin shells; they surface failures from application/domain services without owning them.

### Key Source Files (Agent Boundary)
- `src/Ray.BiliBiliTool.Agent/Extensions/ServiceCollectionExtension.cs` — current HTTP client registration, Polly policy, and `AddBiliBiliClientApi` helper. Phase 6 modifies this file significantly.
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/BiliApiResponse.cs` — current API response model (Code + Message).
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/` — all typed HTTP client interfaces (~17 clients).
- `src/Ray.BiliBiliTool.Agent/BiliCookie.cs` — 6 generic throws targeted for typed replacement.

### Key Source Files (Domain Service Throws)
- `src/Ray.BiliBiliTool.DomainService/LoginDomainService.cs` — 4 throws including 2 QingLong integration failures.
- `src/Ray.BiliBiliTool.DomainService/VideoDomainService.cs` — 2 throws on API non-success codes.
- `src/Ray.BiliBiliTool.DomainService/VipBigPointDomainService.cs` — 3 throws.
- `src/Ray.BiliBiliTool.DomainService/LiveDomainService.cs` — 1 throw.
- `src/Ray.BiliBiliTool.DomainService/AccountDomainService.cs` — 1 throw.
- `src/Ray.BiliBiliTool.DomainService/ArticleDomainService.cs` — 2 throws.
- `src/Ray.BiliBiliTool.DomainService/DonateCoinDomainService.cs` — 1 throw.
- `src/Ray.BiliBiliTool.Infrastructure/Cookie/CookieInfo.cs` — 1 throw (cookie validation).

### Key Source Files (EF Boundary)
- `src/Ray.BiliBiliTool.Web/Components/Comps/LogsDialog.razor.cs` — direct `IDbContextFactory<BiliDbContext>` injection; target for `IExecutionLogRepository`.
- `src/Ray.BiliBiliTool.Web/Services/AuthService.cs` — direct EF injection; target for repository interface.
- `src/Ray.BiliBiliTool.Infrastructure.EF/` — home for `IExecutionLogRepository` implementation.

### Domain Project (Exception Home)
- `src/Ray.BiliBiliTool.Domain/` — current contents: entity files only. Phase 6 adds exception hierarchy here.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `IntervalDelegatingHandler` — already applied to all Bilibili HTTP clients via `AddBiliBiliClientApi`; Phase 6 does not change this handler.
- `WridEncryptionDelegatingHandler` — applied to non-app-path clients; stays as-is.
- `BiliApiResponse<TData>` — current response DTO with `Code` and `Message`; domain services already check `Code` and throw — Phase 6 replaces those throws with typed exceptions.
- Existing Polly `GetRetryPolicy()` private method — Phase 6 extracts and splits this into `BiliResiliencePolicies`.

### Established Patterns
- All 12 Quartz job classes are now thin shells (Phase 5) — they bubble exceptions from `DoExecuteAsync` without catching them; typed exceptions will surface cleanly.
- `AddHttpApi<TInterface>` WebApiClientCore pattern — Phase 6 adds `.ConfigureHttpClient(c => c.Timeout = ...)` inside `AddBiliBiliClientApi`.
- Phase 1 dependency direction: `Agent` → `Domain` is allowed; `DomainService` → `Domain` is allowed; `Web` → `Application.Contracts` is allowed.

### Integration Points
- `BiliResiliencePolicies` referenced from `ServiceCollectionExtension.cs` in `Agent`.
- `BiliException` hierarchy referenced from DomainService throw sites and potentially from `BaseJob` or application services for catch-by-type.
- `IExecutionLogRepository` registered in `Infrastructure.EF` ServiceCollectionExtension; injected into `LogsDialog.razor.cs` and `AuthService`.

</code_context>

<specifics>
## Specific Ideas

- No specific references or examples cited — open to standard .NET exception hierarchy conventions.

</specifics>

<deferred>
## Deferred Ideas

- **Notification service boundary** — `INotificationService` placeholder deferred; build behind a proper boundary when notification code is first introduced.
- **`AuthService.cs` exception** — the `throw new Exception("Current password is incorrect.")` in the Web auth service is excluded from Phase 6 sweep; a separate Web-layer cleanup phase can address it.

</deferred>
