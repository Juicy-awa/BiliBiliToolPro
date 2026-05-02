# Phase 1: Boundary Guardrails - Context

**Gathered:** 2026-05-02
**Status:** Ready for planning

<domain>
## Phase Boundary

This phase establishes explicit dependency direction and registration seams around the existing module/layer structure so maintainers can change modules with less fear. The target is not a broad host rewrite; it is a guardrail phase that makes boundary violations visible and enforceable before later slice refactors move behavior.

</domain>

<decisions>
## Implementation Decisions

### 目标分层模型
- **D-01:** `DomainService` remains a distinct layer for now; Phase 1 does not collapse it into `Application`.
- **D-02:** `Application` is the application orchestration and use-case entry layer.
- **D-03:** Business rules and domain judgments should primarily stay in `Domain` and `DomainService`.
- **D-04:** The most important layering outcome is that hosts depend on `Application` entry points rather than reaching into lower layers directly.

### 依赖规则落地
- **D-05:** Dependency guardrails should land first as executable architecture rules/tests, not just conventions.
- **D-06:** Phase 1 may fix a small number of project references, but only when they block guardrail enforcement.
- **D-07:** The first forbidden dependency class to police is host or job code reaching directly into lower layers.
- **D-08:** The key success signal is that new dependency violations cannot enter the mainline.

### 注册入口粒度
- **D-09:** Phase 1 keeps the current technology-layer registration seams rather than switching to business-capability registration.
- **D-10:** Hosts may still compose multiple technology-layer `Add*` entry points directly; a single aggregate entry point is not required yet.
- **D-11:** Web and Console should follow the same registration ordering and grouping conventions.
- **D-12:** New registration entry points are allowed when needed, but they must follow naming and placement conventions instead of adding arbitrary host-level scatter.

### 宿主清理边界
- **D-13:** Phase 1 may improve both host business wiring and startup composition structure, but only incrementally.
- **D-14:** Significant host thinning is not the completion bar for this phase; current overall host shape may remain if boundaries are clearer.
- **D-15:** This phase should not define a hard migration list of logic that must leave the host; it should avoid overcommitting host cleanup before safety nets exist.
- **D-16:** The most important host-side outcome is that host responsibilities are explicitly defined and can later be constrained by tests or rules.

### the agent's Discretion
- Detailed enforcement mechanics for architecture tests, naming checks, and registration ordering are left to research and planning.
- The exact shape of allowed dependency rules between `Agent`, `Application`, `DomainService`, `Infrastructure`, and host projects is left to research and planning, as long as it preserves the decisions above.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Project And Scope
- `.planning/PROJECT.md` — project-level refactor goals, constraints, and out-of-scope boundaries.
- `.planning/REQUIREMENTS.md` — phase-mapped requirements including `ARCH-01`, `ARCH-03`, and `TEST-03` for this phase.
- `.planning/ROADMAP.md` — Phase 1 goal, dependency order, and success criteria.
- `.planning/STATE.md` — current project position and current-phase focus.

### Codebase Maps
- `.planning/codebase/ARCHITECTURE.md` — current composition roots, layer roles, and architectural tension points.
- `.planning/codebase/CONVENTIONS.md` — established `Add*` registration pattern, startup style, and testing conventions.
- `.planning/codebase/CONCERNS.md` — current architecture and startup concerns that justify Phase 1 guardrails.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `src/Ray.BiliBiliTool.Application/Extensions/ServiceCollectionExtension.cs`: existing application-layer registration seam that can remain a first-class host entry point.
- `src/Ray.BiliBiliTool.DomainService/Extensions/ServiceCollectionExtensions.cs`: existing domain-service registration seam that reflects the current layer split the user chose to keep.
- `src/Ray.BiliBiliTool.Agent/Extensions/ServiceCollectionExtension.cs`: existing agent registration seam that already concentrates typed HTTP client setup.
- `src/Ray.BiliBiliTool.Infrastructure.EF/Extensions/ServiceCollectionExtension.cs`: existing persistence registration seam that can be constrained instead of rediscovered.
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs`: web-host extension pattern already exists and can inform consistent grouping.

### Established Patterns
- Host composition is already organized around `Add*` extension methods rather than fully inline service registration.
- Web and Console both act as composition roots, but their grouping and ordering are not yet governed by a shared rule.
- The repo already treats DI extensions as the standard unit of startup composition, so Phase 1 should reinforce that seam rather than invent a new abstraction prematurely.

### Integration Points
- `src/Ray.BiliBiliTool.Web/Program.cs` is the richest host composition root and a likely enforcement target for host-to-lower-layer dependency rules.
- `src/Ray.BiliBiliTool.Console/Program.cs` is the second primary host composition root and should follow the same registration ordering conventions.
- Quartz registration in the web host will need explicit treatment because scheduling code is one of the host-like call sites that can bypass intended boundaries.

</code_context>

<specifics>
## Specific Ideas

- Keep the current technology-layer seams visible in hosts for now instead of forcing an early shift to capability-oriented module registration.
- Prefer a rule set that stops new violations first, then only repairs existing references when they block enforcement.
- Treat host cleanup conservatively in Phase 1; boundary clarity matters more than making startup files visibly shorter.

</specifics>

<deferred>
## Deferred Ideas

None — discussion stayed within phase scope.

</deferred>

---

*Phase: 1-Boundary Guardrails*
*Context gathered: 2026-05-02*