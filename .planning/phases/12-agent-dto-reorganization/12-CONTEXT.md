# Phase 12: Agent DTO Reorganization - Context

**Gathered:** 2026-05-04
**Status:** Ready for planning

<domain>
## Phase Boundary

Reorganize Agent-layer DTO folders so they follow interface-first ownership boundaries.

The top-level DTO directory should mirror interface boundaries first:
- `Dtos/AccountApi/...` for `IAccountApi`
- `Dtos/NavApi/...` for `INavApi`
- `Dtos/ApiApi/...` for `IApiApi`

For large consolidated interfaces such as `IApiApi`, downstream planning may add a second level of grouping under that interface root (for example `Dtos/ApiApi/UpInfo/...`).

This phase remains limited to DTO moves, namespace updates, and consumer fixes required to restore a green build and the required test suites. It does not include DTO class renames or broader DTO redesign.

</domain>

<decisions>
## Implementation Decisions

### Move And Layout Strategy
- **D-01:** File moves should preserve history and be treated as true renames/moves. Use a move strategy equivalent to `git mv`, not delete-and-recreate when avoidable.
- **D-02:** DTO directory structure is interface-first, not region-first. `AccountApi` maps to `IAccountApi`, `Nav` maps to `INavApi`, and `IApiApi` should have its own interface-level root directory before any sub-grouping.
- **D-03:** For `IApiApi`, do not lock the full subtree structure in this discussion. Downstream planning should refine the subfolder structure under the `Dtos/ApiApi/` root as needed by the interface's internal categories (for example `Dtos/ApiApi/UpInfo/...`).

### Nav DTO Boundary
- **D-04:** `UserInfo`, `WbiImg`, `LevelInfo`, and `Wallet` are treated as one Nav DTO group. If they are ever split into separate files, they should still live under `Dtos/NavApi/`.

### Consumer Repair Rule
- **D-05:** Consumer updates are correctness-driven, not list-driven. The initial plan file list is only a starting set; downstream execution may and should update additional real consumers exposed by the compiler or tests until build and required tests pass.

### the agent's Discretion
- Preserve existing DTO class names unless a later phase explicitly changes that rule. The current requirements say this phase is for moves and namespace changes, not class renames.
- If interface-first hierarchy still introduces namespace/type collisions, downstream execution may use aliases or equivalent non-class-renaming techniques to resolve them.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Phase Scope And Requirements
- `.planning/PROJECT.md` — milestone goal and current Phase 12 intent
- `.planning/REQUIREMENTS.md` — authoritative scope for DTO-01 through DTO-06; class renames remain out of scope
- `.planning/ROADMAP.md` — current phase goal and existing plan inventory
- `.planning/STATE.md` — current project state and continuity notes

### Existing Phase Artifacts
- `.planning/phases/12-agent-dto-reorganization/12-01-PLAN.md` — existing move plan that should be reconsidered under the interface-first directory rule
- `.planning/phases/12-agent-dto-reorganization/12-02-PLAN.md` — existing consumer-fix plan; use as a baseline, not a hard upper bound
- `.planning/phases/12-agent-dto-reorganization/12-01-SUMMARY.md` — what was already executed for the first rename attempt
- `.planning/phases/12-agent-dto-reorganization/12-02-SUMMARY.md` — what consumer fixes were required in practice

### Interface Ownership Anchors
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/IApiApi.cs` — consolidated `api.bilibili.com` interface; DTOs now live under the `ApiApi` root before sub-grouping
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/IAccountApi.cs` — `AccountApi` DTO ownership anchor
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/INavApi.cs` — `NavApi` DTO ownership anchor

### Current DTO Examples
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/ApiApi/UpInfo/UpInfo.cs` — final `IApiApi` UpInfo placement
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/AccountApi/CoinBalance.cs` — final `AccountApi` placement pattern
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/NavApi/UserInfo.cs` — final `NavApi` grouping pattern

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `IApiApi.cs`: already groups many `api.bilibili.com` endpoints in one place, so it can support a nested `Dtos/ApiApi/...` structure instead of one flat DTO folder per former endpoint.
- `IAccountApi.cs` and `INavApi.cs`: clear single-interface ownership anchors for their DTO groups.
- Existing compiler and test suite: solution build, architecture tests, and host integration tests already expose missed namespace consumers quickly.

### Established Patterns
- Earlier refactors consolidated interface boundaries first, then updated dependents to match those ownership boundaries.
- This codebase accepts targeted aliasing to avoid widening scope when a rename/move creates a namespace/type collision.

### Integration Points
- DTO namespace changes propagate into Agent interfaces, DomainService interfaces and implementations, Application services, and tests.
- The compiler is the authoritative detector for plan-underestimated consumers; test suites validate that the namespace migration did not break runtime composition.

</code_context>

<specifics>
## Specific Ideas

- The user explicitly corrected the naming rule from region-first to interface-first.
- `Dtos/UpInfo/...` as a top-level target was the wrong abstraction. The delivered structure uses the `Dtos/ApiApi/...` root, with subfolders under that root when useful.
- The final naming rule is interface name minus leading `I`, so roots are `ApiApi`, `AccountApi`, `NavApi`, `LiveApi`, `LiveTraceApi`, `MangaApi`, `PassportApi`, and `ShowApi`.

</specifics>

<deferred>
## Deferred Ideas

None — discussion stayed within phase scope.

</deferred>

---

*Phase: 12-Agent DTO Reorganization*
*Context gathered: 2026-05-04*