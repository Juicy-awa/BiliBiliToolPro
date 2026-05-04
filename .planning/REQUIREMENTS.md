# Requirements: BiliBiliToolPro Brownfield Refactor

**Defined:** 2026-05-04
**Core Value:** Make the existing codebase safe to change: clear boundaries, lower coupling, and testable critical flows.

## v4.0.0.3 Requirements

Migrate the Agent layer HTTP client abstraction from WebApiClientCore to Refit — 18 interfaces, DI registration, and associated infrastructure. Pure internal library swap; all API behavior and existing tests must be preserved.

### Agent Layer

- [ ] **REFIT-01**: Maintainer can see all 17 Bilibili HTTP client interfaces using Refit attributes (`[Get]`, `[Post]`, `[Body]`, `[Query]`, `[Headers]`) instead of WebApiClientCore attributes (`[HttpGet]`, `[HttpPost]`, `[FormContent]`, `[JsonContent]`, `[PathQuery]`)
- [ ] **REFIT-02**: Maintainer can see `IQingLongApi` using Refit attributes instead of WebApiClientCore attributes
- [ ] **REFIT-03**: Maintainer can see DI registration using `AddRefitClient<T>` with the same delegating handlers (`IntervalDelegatingHandler`, `WridEncryptionDelegatingHandler`) and Polly policies as before (read-only vs mutating distinction preserved)
- [ ] **REFIT-04**: Maintainer can see `IBiliBiliApi` common headers (Accept, Accept-Language, Sec-Fetch-Dest, Sec-Fetch-Mode, Sec-Fetch-Site, Connection) injected by `BiliBiliCommonHeadersDelegatingHandler` instead of `AppendHeaderAttribute` (WebApiClientCore-specific)
- [ ] **REFIT-05**: Maintainer can verify WebApiClientCore package is removed from `Directory.Packages.props` and `Ray.BiliBiliTool.Agent.csproj`; `AppendHeaderAttribute.cs`, `AppendHeaderType.cs`, and `LogFilterAttribute.cs` are deleted; `Refit` and `Refit.HttpClientFactory` packages added
- [ ] **REFIT-06**: Maintainer can verify the build passes with 0 errors and existing architecture tests (4/4) and integration tests (7/7) continue to pass

## Deferred Requirements

Captured from prior milestones — not in scope for v4.0.0.3.

### Testing

- **TEST-04**: Maintainer can verify key Web or Blazor components with dedicated component tests
- **TEST-05**: Maintainer can enforce focused coverage thresholds for critical modules in CI

### Flows

- **FLOW-05**: Maintainer can unify Console and Web configuration and startup composition paths where behavior meaningfully overlaps

### Quality

- **QUAL-03**: Maintainer can remove default credential risks and similar obvious safety issues from bootstrap flows
- **QUAL-04**: Maintainer can reduce repository noise from generated outputs so searches and reviews focus on source of truth files

### Architecture

- **ARCH-04**: Notification adapter/port boundary (Serilog sink deferred from v4.0.0.1)

## Out of Scope

| Feature | Reason |
|---------|--------|
| Changing API endpoint behavior | Pure library swap — observable behavior must be preserved |
| New Bilibili task features | This milestone is about internal library migration only |
| Blazor component tests / CI coverage | Deferred to future milestone |
| Console/Web config unification | Deferred to future milestone |
| Notification adapter boundary | Deferred to future milestone |
| Refit response validation / error handling changes | Behavior-preserving migration only; error semantics kept as-is |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| REFIT-04 | Phase 8 | ⬜ Planned |
| REFIT-01 | Phase 9 | ⬜ Planned |
| REFIT-02 | Phase 10 | ⬜ Planned |
| REFIT-03 | Phase 10 | ⬜ Planned |
| REFIT-05 | Phase 10 | ⬜ Planned |
| REFIT-06 | Phase 10 | ⬜ Planned |

**Coverage:**
- v4.0.0.3 requirements: 6 total
- Mapped to phases: 6
- Unmapped: 0 ✓

---
*Requirements defined: 2026-05-04*
*Last updated: 2026-05-04 — milestone v4.0.0.3 started*
