# Phase 6: Integration Boundary And Failure Model - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-05-03
**Phase:** 06-integration-boundary-and-failure-model
**Areas discussed:** Failure type taxonomy, Adapter normalization scope, Resilience policy design, EF and notification adapter boundaries

---

## Failure Type Taxonomy

### Q1: Failure model approach

| Option | Description | Selected |
|--------|-------------|----------|
| A | Custom exception hierarchy (`BiliBusinessException`, `BiliIntegrationException`) | ✓ |
| B | Result\<T\> / discriminated union — methods return Result, callers pattern-match | |
| C | Agent discretion | |

**User's choice:** A — custom exception hierarchy  
**Notes:** Keeps method signatures unchanged. Stays idiomatic .NET.

### Q2: Where exception types live

| Option | Description | Selected |
|--------|-------------|----------|
| A | `Ray.BiliBiliTool.Agent` | |
| B | `Ray.BiliBiliTool.Application.Contracts` | |
| C | `Ray.BiliBiliTool.Domain` | ✓ |
| D | Agent discretion | |

**User's choice:** C — Domain project  
**Notes:** Correct semantic home; accessible from both Agent and DomainService without circular references.

### Q3: Exception hierarchy shape

| Option | Description | Selected |
|--------|-------------|----------|
| A | Flat — two base types: `BiliBusinessException`, `BiliIntegrationException` | |
| B | Three-tier — `BiliException` base → `BiliBusinessException`, `BiliIntegrationException`, `BiliValidationException` | ✓ |
| C | Agent discretion based on throw site analysis | |

**User's choice:** B — three-tier hierarchy  
**Notes:** `BiliValidationException` captures the cookie/input validation failures in `BiliCookie.cs` and `CookieInfo.cs` which are semantically distinct from both business and integration failures.

### Q4: Log level differentiation

| Option | Description | Selected |
|--------|-------------|----------|
| A | Yes — `BiliBusinessException` → Warning, `BiliIntegrationException` → Error, `BiliValidationException` → Error | |
| B | No — logging unchanged, typed exceptions for catch-by-type only | ✓ |
| C | Agent discretion | |

**User's choice:** B — logging stays identical  
**Notes:** Avoids any behavioral change. Clean separation of concerns — typing is for programmatic handling, not log format changes.

---

## Adapter Normalization Scope

### Q1: Which throw sites to convert

| Option | Description | Selected |
|--------|-------------|----------|
| A | Critical paths only (Login + DailyTask call chains) | |
| B | Full sweep — all domain service + agent throws except `AuthService.cs` | ✓ |
| C | Agent discretion based on characterization test coverage | |

**User's choice:** B — full sweep  
**Notes:** 22 total generic throws; all except `AuthService.cs` (Web layer) will be converted.

### Q2: QingLong adapter scope

| Option | Description | Selected |
|--------|-------------|----------|
| A | Yes — include QingLong throws (already in `LoginDomainService`) | ✓ |
| B | No — defer QingLong to a dedicated integration phase | |
| C | Agent discretion | |

**User's choice:** A — include QingLong throws  
**Notes:** Both QingLong throws are inside `LoginDomainService` which is already in scope; natural to convert them.

### Q3: Tests for typed exceptions

| Option | Description | Selected |
|--------|-------------|----------|
| A | Yes — add targeted unit tests asserting typed exceptions at key throw sites | ✓ |
| B | No — mechanical replacement is enough | |
| C | Agent discretion — tests only where cheap (pure validation logic) | |

**User's choice:** A — add targeted unit tests  
**Notes:** Makes QUAL-01 verifiable. Primary target: pure validation logic in `BiliCookie.cs` and `CookieInfo.cs`.

---

## Resilience Policy Design

### Q1: Differentiate retry policies by client type

| Option | Description | Selected |
|--------|-------------|----------|
| A | Yes — split: read-only gets retry, side-effecting gets conservative/no retry | ✓ |
| B | No — keep single shared policy | |
| C | Agent discretion | |

**User's choice:** A — split policies  
**Notes:** Prevents double-execution risk on side-effecting calls (coin donation, charge, live sign-in).

### Q2: Explicit timeouts

| Option | Description | Selected |
|--------|-------------|----------|
| A | Yes — explicit 30s timeout, diagnosable `TaskCanceledException` | ✓ |
| B | No — leave at 100s default | |
| C | Agent discretion | |

**User's choice:** A — 30s explicit timeout  
**Notes:** 100s silent hang is hard to diagnose in scheduled automation context.

### Q3: Policy verifiability

| Option | Description | Selected |
|--------|-------------|----------|
| A | Yes — extract to `BiliResiliencePolicies` class with named constants | ✓ |
| B | No — policy correctness verified by code review only | |
| C | Agent discretion | |

**User's choice:** A — `BiliResiliencePolicies` named class  
**Notes:** Tests reference constants to verify policy assignment without real HTTP endpoints.

---

## EF And Notification Adapter Boundaries

### Q1: EF adapter boundary approach

| Option | Description | Selected |
|--------|-------------|----------|
| A | Introduce `IExecutionLogRepository` — Web/UI stops reaching `IDbContextFactory` directly | ✓ |
| B | No new interfaces — just move logic to existing `ExecutionLogService` | |
| C | Fix only `LogsDialog.razor.cs` direct EF reach | |
| D | Agent discretion | |

**User's choice:** A — `IExecutionLogRepository` interface  
**Notes:** `LogsDialog.razor.cs` and `AuthService` both inject `IDbContextFactory<BiliDbContext>` directly; both move to the repository interface.

### Q2: Notification boundary

| Option | Description | Selected |
|--------|-------------|----------|
| A | Out of scope — no notification code exists | ✓ |
| B | Define placeholder `INotificationService` now | |
| C | Agent discretion | |

**User's choice:** A — out of scope  
**Notes:** Nothing to normalize. Future notification work builds behind a proper boundary from day one.

---

## the agent's Discretion

- Exact naming of `BiliResiliencePolicies` constants
- Classification of individual throw sites as Business vs Integration vs Validation (use semantic judgment from throw site context)
- Minimal surface of `IExecutionLogRepository` (only what `LogsDialog` and `AuthService` currently need)
- Unit test placement (existing `DomainServiceTest` project or new Agent test project)

## Deferred Ideas

- `INotificationService` placeholder — when notification code is first introduced, build it behind a proper boundary
- `AuthService.cs` generic throw — Web-layer auth concern, excluded from Phase 6 sweep
