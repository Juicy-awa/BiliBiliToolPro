# Plan 06-01 Summary: BiliException Hierarchy + BiliResiliencePolicies

**Phase:** 06-integration-boundary-and-failure-model
**Plan:** 01
**Completed:** 2026-05-03
**Commit:** feat(06-01): add BiliException hierarchy and BiliResiliencePolicies with split policies and 30s timeout

## What Was Built

### Exception Hierarchy (Domain layer)
Created `src/Ray.BiliBiliTool.Domain/Exceptions/` with four files:
- `BiliException.cs` — abstract base class
- `BiliBusinessException.cs` — API non-success business code failures
- `BiliIntegrationException.cs` — network/external system failures (QingLong, HTTP)
- `BiliValidationException.cs` — input/cookie validation failures

### Project References
- Added `Domain` reference to `DomainService.csproj` — throw sites in Wave 2 can now use typed exceptions
- Added `Domain` reference to `Agent.csproj` — BiliCookie.cs in Plan 04 can use BiliValidationException

### BiliResiliencePolicies (Agent layer)
Created `src/Ray.BiliBiliTool.Agent/BiliResiliencePolicies.cs` with:
- `ReadOnlyRetryCount = 1`, `ReadOnlyRetryBackoff = 2s`, `HttpTimeout = 30s` constants
- `ReadOnlyPolicy()` — retries once on transient HTTP errors (for idempotent clients)
- `MutatingPolicy()` — retries only on network failures, not 5xx (for side-effecting clients)

### ServiceCollectionExtension Updates
- Added `c.Timeout = BiliResiliencePolicies.HttpTimeout` (30s) to both `config` and `configApp` lambdas
- Updated private `AddBiliBiliClientApi` helper to accept optional `IAsyncPolicy<HttpResponseMessage>? policy`
- Switched 4 side-effecting clients to `MutatingPolicy()`: `IChargeApi`, `IVideoApi`, `ILiveApi`, `IMallApi`
- Updated QingLong client to include 30s timeout + `ReadOnlyPolicy()`
- Removed `GetRetryPolicy()` — replaced by `BiliResiliencePolicies`

## Verification Results
- `dotnet build Ray.BiliBiliTool.sln`: 0 errors, 102 pre-existing warnings
- All 8 files committed via pre-commit Husky + CSharpier format

## Decisions Made
- Per D-08/D-09/D-10: split policies, explicit 30s timeout, named constants
- `MutatingPolicy` uses `HandleTransientHttpError()` without `OrResult` — retries network failures, not business-level 4xx

## Files Modified
- `src/Ray.BiliBiliTool.Domain/Exceptions/BiliException.cs` (created)
- `src/Ray.BiliBiliTool.Domain/Exceptions/BiliBusinessException.cs` (created)
- `src/Ray.BiliBiliTool.Domain/Exceptions/BiliIntegrationException.cs` (created)
- `src/Ray.BiliBiliTool.Domain/Exceptions/BiliValidationException.cs` (created)
- `src/Ray.BiliBiliTool.Agent/BiliResiliencePolicies.cs` (created)
- `src/Ray.BiliBiliTool.Agent/Extensions/ServiceCollectionExtension.cs` (modified)
- `src/Ray.BiliBiliTool.Agent/Ray.BiliBiliTool.Agent.csproj` (modified)
- `src/Ray.BiliBiliTool.DomainService/Ray.BiliBiliTool.DomainService.csproj` (modified)
