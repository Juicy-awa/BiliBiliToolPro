# Phase 1: Boundary Guardrails - Research

**Date:** 2026-05-02
**Status:** Complete
**Scope:** Phase-specific planning research for executable dependency rules and registration seams.

## Question

How should Phase 1 make dependency direction and module registration seams executable without turning this into a broad host rewrite?

## Findings

### Existing seams are already technology-layer based

- The codebase already exposes DI seams through `AddAppServices`, `AddDomainServices`, `AddBiliBiliClientApi`, `AddEF`, `AddWebServices`, and `AddAuthServices`.
- This matches the user's locked decision to keep technology-layer registration seams visible in Phase 1 rather than switching immediately to capability-based module entry points.
- Web and Console currently use those seams in different shapes and orders, so the immediate opportunity is to standardize grouping and host-facing composition rather than invent a new module model.

### Boundary enforcement is currently missing

- The repo has multiple test projects, but sampled tests mainly boot hosts or resolve services; there is no existing architecture-test project and no sign of ArchUnitNET or NetArchTest usage.
- Project references and host composition show the current boundary risk directly: `Ray.BiliBiliTool.Web` references `Infrastructure.EF`, `Ray.BiliBiliTool.Console` references `Agent`, `DomainService`, and `Infrastructure`, and `Ray.BiliBiliTool.Application` references `Agent`, `DomainService`, and `Infrastructure`.
- Phase 1 therefore needs a dedicated executable guardrail asset first, not just documentation.

### Recommended enforcement tool

- Use ArchUnitNET as the primary executable architecture-rule mechanism for this phase.
- This aligns with the existing research stack recommendation and fits the repo's xUnit-based test posture without forcing a new runtime abstraction.
- Prefer architecture rules over string-based grep gates for dependency direction. Grep can be a supplemental verification tool for host composition ordering, but not the primary guardrail.

### Recommended Phase 1 boundary contract

- Host and Quartz entry code should call application-facing services or explicit registration seams, not concrete lower-layer service implementations.
- `Application` remains the orchestration layer and should not take new dependencies on web, Blazor, Quartz, or concrete transport DTO namespaces.
- `Domain` and `DomainService` remain the business-logic side and should not grow web, EF, or HTTP-client concerns.
- Adapter seams in `Agent` and `Infrastructure.EF` can stay visible to the hosts for registration in Phase 1, but their usage should be concentrated in explicit host composition groups rather than scattered calls.

### Host cleanup should stay conservative

- The user explicitly does not want Phase 1 judged by visibly smaller host files.
- The practical target is a clearer composition shape: configuration, grouped registration, platform wiring, and explicit startup tasks.
- Quartz should remain in the Web host, but its registration should stay behind the existing `AddBiliJobs` seam, with startup side effects like database initialization moved behind a named host extension instead of inline service resolution.

## Planning Implications

1. Plan Wave 1 should create a dedicated architecture-test project and encode the first batch of boundary rules from the user decisions.
2. The next plan should normalize Web and Console composition around explicit host-local grouping methods that wrap the current technology-layer `Add*` seams.
3. Host startup cleanup should be limited to composition clarity and named startup tasks; it should not attempt the broader host-thinning goals reserved for Phase 2.
4. Verification should center on `dotnet test` for the architecture suite and `dotnet build` for the solution, with grep/ripgrep used only for narrow composition-shape assertions.

## Risks To Control

- If ArchUnit rules try to enforce the final target architecture too early, Phase 1 will expand into a broad assembly-graph rewrite.
- If host grouping methods hide too much, they will conflict with the user's decision to keep technology-layer seams visible.
- If host cleanup touches routes, schedules, config keys, or runtime behavior, Phase 1 will take on Phase 2/3 risk too early.

## Recommended Plan Shape

- **Plan 01:** Add executable architecture rules and make dependency direction fail fast.
- **Plan 02:** Standardize host registration seams and startup-task boundaries across Web and Console while preserving current behavior.

---

*Phase research completed: 2026-05-02*