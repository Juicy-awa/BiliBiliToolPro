# Stack Research

## Recommended Baseline

Keep the platform centered on .NET 8, ASP.NET Core, the generic host, built-in DI, options binding, EF Core, Quartz, and IHttpClientFactory. For this codebase, the refactor stack should reduce coupling by sharpening module boundaries and adding tests around critical flows, not by replacing the runtime model.

Recommended baseline:

| Area | Recommendation | Why |
|---|---|---|
| Runtime/hosts | Keep .NET 8 for Console, Web, and Blazor WebAssembly | Stable baseline already in use; refactor risk stays low |
| Composition | Keep HostApplicationBuilder/WebApplicationBuilder + built-in DI | Standard Microsoft path; enough for a modular monolith |
| Configuration | Keep Microsoft.Extensions.Options with validated options per module | Makes boundary contracts explicit without new framework weight |
| Data access | Keep EF Core and current provider choices | Refactor repositories/services first; do not churn persistence stack yet |
| Scheduling | Keep Quartz | Existing scheduling behavior is already a core capability |
| HTTP integrations | Keep typed HttpClient-based integration clients; add Microsoft.Extensions.Http.Resilience where needed | Modernizes resiliency without replacing all client code |
| Logging | Keep Serilog as the log sink layer | Already present and compatible with richer diagnostics |
| API contracts | Prefer explicit request/response DTO contracts in Application.Contracts | Helps separate use-case orchestration from transport/integration details |

Add selectively:

- Microsoft.Extensions.Http.Resilience for outbound Bilibili API clients that need retries, timeout, circuit breaker, and rate limiting.
- FluentValidation only if input/config validation is currently scattered and repetitive. Do not add it just to validate simple options objects.
- Scrutor for module registration scanning only at composition boundaries. Do not let scanning hide important dependencies.

## Keep/Replace Decisions

Keep:

- Keep ASP.NET Core, Generic Host, Options, ILogger abstractions, and IHttpClientFactory. These are the standard host-level building blocks in 2025/2026 and fit both Console and Web hosts.
- Keep Quartz rather than replacing it with a mediator/job framework. The problem is boundary clarity around jobs, not the scheduler itself.
- Keep EF Core and current DbContext-based persistence unless a specific persistence hotspot proves otherwise.
- Keep Serilog for sinks/enrichment if operations already depend on it.

Replace or phase out:

- Phase out cross-layer service calls that let Web or jobs reach directly into infrastructure details. Route through application use-case services/handlers instead.
- Phase out broad static helpers and ambient access patterns in critical flows; prefer injected ports/interfaces per module.
- Phase out catch-all exception handling in orchestration code; use typed failure/result patterns at module boundaries where refactors are risky.
- For new or heavily touched outbound clients, prefer standard HttpClient + resilience handlers over framework-specific client magic. Existing WebApiClientCore code can stay until each client is touched; do not rewrite all clients up front.

## Testing-Enabler Tooling

Recommended minimum test stack:

| Need | Tooling | Recommendation |
|---|---|---|
| Unit tests | xUnit + FluentAssertions | Keep current direction; avoid test framework churn first |
| Test doubles | NSubstitute | Lightweight and readable for service-boundary tests |
| Integration tests with real infra | Testcontainers for .NET | Best fit for reproducible DB/API-adjacent tests on Windows/Linux/macOS |
| Database reset between tests | Respawn | Fast clean-state resets for EF-backed integration tests |
| Snapshot/golden-master protection for legacy flows | Verify | Useful for stabilizing complex DTO, JSON, and command/result shapes during refactor |
| Blazor component tests | bUnit | Standard choice for targeted Razor component tests |
| Web endpoint tests | WebApplicationFactory/TestServer | Stay on Microsoft test host primitives for ASP.NET Core |

Testing posture:

- Start with characterization tests on the highest-risk flows: login/session bootstrap, daily task orchestration, scheduler-triggered execution, and core typed HTTP clients.
- Add integration tests at module seams, not just class-level unit tests. Brownfield safety comes from testing observable behavior.
- Use Verify sparingly for payload-heavy or legacy behavior where dozens of brittle assertions would slow refactoring.
- Prefer Testcontainers only for flows that actually benefit from realistic database behavior. Do not force every test through containers.

## Architectural Enforcement Options

Recommended first choice:

- ArchUnitNET for executable architecture rules between assemblies/namespaces. It is active, supports xUnit integrations, and is well-suited to modular-monolith boundary tests.

Good complementary enforcement:

- Roslyn analyzers already in the SDK plus nullable reference types and warnings-as-errors for new/changed projects.
- Directory.Build.props and project-reference rules to make dependency direction visible in the build.
- Optional NetArchTest only if the team wants very lightweight convention tests, but ArchUnitNET is the better long-term primary option.

Rules to encode early:

- Web and Quartz job projects may depend on Application/Application.Contracts, but not directly on Infrastructure internals.
- Application should not depend on Web, Blazor, or concrete HTTP client implementations.
- Infrastructure may implement ports from Application or Domain-facing abstractions, but not own orchestration.
- Agent/integration code should not leak transport DTOs into Domain-facing APIs unless intentionally mapped.

## Observability/Diagnostics Support

Recommended support stack:

- Keep Serilog for structured logs.
- Add OpenTelemetry.Extensions.Hosting with ASP.NET Core, HttpClient, and runtime instrumentation for traces and metrics.
- Add health endpoints in the Web host with readiness/liveness separation where useful.
- Add correlation IDs/activity propagation through scheduled jobs and outbound HTTP calls.
- Use the Aspire dashboard only as an optional local diagnostics aid if the team wants it; do not adopt full Aspire orchestration for this refactor.

Why this matters here:

- Refactors fail quietly when job execution, retries, and external API behavior are opaque.
- Traces around scheduler -> application use case -> HTTP client -> persistence are more valuable than adding another abstraction framework.

## What Not To Add

- Do not introduce a full CQRS/mediator rewrite just to look cleaner. Add command/query handlers only where they clarify a messy use case.
- Do not switch to microservices, distributed messaging, or a new scheduler as part of architecture cleanup.
- Do not add a second DI container.
- Do not add a large modular framework such as ABP, Orleans, or actor/workflow infrastructure for this problem.
- Do not adopt full Aspire AppHost orchestration for a single brownfield modular monolith unless deployment topology changes materially.
- Do not replace every HTTP client library in one pass.

## Recommended Adoption Order

1. Freeze behavior with characterization and integration tests around the most fragile flows.
2. Introduce explicit module contracts and tighten project references before moving code.
3. Add ArchUnitNET rules for forbidden dependencies and keep them in CI.
4. Standardize typed HttpClient registration and add Microsoft.Extensions.Http.Resilience only to the clients that need it.
5. Add OpenTelemetry + health checks so refactor regressions show up quickly in logs, traces, and probes.
6. Extract or rename services only after tests and dependency rules are in place.
7. Migrate legacy clients/helpers incrementally when each area is already under test.

Bottom line: keep the existing Microsoft/.NET host stack, keep Quartz, keep EF Core, keep Serilog, and add a thin ring of enforcement and test tooling around the current system. The winning stack for this repo is conservative platform alignment plus executable boundaries, not framework replacement.