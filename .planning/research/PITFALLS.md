# Refactor Pitfalls Research

This research focuses on brownfield refactoring of live .NET modular monoliths where the goal is better boundaries, better testability, and safer gradual delivery rather than a rewrite.

## Technical Pitfalls

### 1. Reorganizing namespaces and projects before isolating behavior
- Why it happens: teams want visible architectural progress fast, so they start by moving files and creating Domain/Application/Infrastructure folders without first identifying stable use-case seams.
- How it shows up in practice: large PRs mostly rename and relocate code, dependencies still point across layers, and the system becomes harder to debug because behavior moved but coupling did not.
- How to avoid it: pick one high-risk vertical slice, freeze its behavior with characterization tests, then introduce a thin application seam around that slice before moving more code.

### 2. Creating abstraction layers that only wrap existing chaos
- Why it happens: cleaner architecture is interpreted as “add interfaces everywhere,” even when the real problem is mixed orchestration, persistence, and HTTP logic in the same flow.
- How it shows up in practice: services named Manager, Helper, or DomainService remain huge, but now every method is called through three interfaces and debugging requires stepping across hollow wrappers.
- How to avoid it: add abstractions only at decision-making seams such as use cases, outbound clients, and repositories you genuinely need to substitute in tests.

### 3. Treating module boundaries as compile-time fiction
- Why it happens: teams draw module diagrams, but keep shared tables, shared DTOs, and direct service calls because changing them feels expensive.
- How it shows up in practice: one module change still forces changes in unrelated modules, transport DTOs leak into business logic, and “modular monolith” becomes just a folder layout.
- How to avoid it: enforce dependency direction with project references and architecture tests, and require cross-module calls to go through explicit contracts or application facades.

## Process Pitfalls

### 4. Running architecture cleanup as a side quest instead of a delivery strategy
- Why it happens: refactoring work is funded separately from feature delivery, so the team builds a parallel architecture effort with weak ties to user-visible outcomes.
- How it shows up in practice: the backlog fills with technical tasks that never finish, business stakeholders lose trust, and the team abandons the refactor halfway through.
- How to avoid it: couple each cleanup step to a real slice of behavior, such as one scheduled task flow, one login flow, or one HTTP integration path, and ship that slice end to end.

### 5. Choosing phase boundaries by subsystem ownership instead of change risk
- Why it happens: work is split by existing team or folder structure rather than by the flows that most often break or block change.
- How it shows up in practice: low-risk areas get cleaned first because they are politically easy, while the real pain points stay untouched and continue to contaminate new code.
- How to avoid it: rank slices by volatility, operational impact, and dependency density; start where better seams will unlock repeated future work.

### 6. Underestimating transitional architecture
- Why it happens: teams assume the new structure should replace the old one immediately and treat adapters, anti-corruption mappings, or temporary facades as waste.
- How it shows up in practice: callers are switched too early, old and new paths diverge unpredictably, and rollback becomes difficult because no stable compatibility layer exists.
- How to avoid it: budget for temporary composition code, dual paths, mapping layers, and strangler seams explicitly; transitional code is part of the plan, not a smell by default.

## Testing Pitfalls

### 7. Starting with unit tests where the real risk is orchestration
- Why it happens: unit tests feel cheaper, so teams test isolated classes while the fragile behavior actually lives in jobs, host startup, EF usage, and HTTP integrations.
- How it shows up in practice: unit coverage looks healthy, but refactors still break daily tasks, login bootstrap, scheduler flows, or configuration-dependent behavior.
- How to avoid it: begin with characterization and integration tests around the most business-critical flows, then add focused unit tests inside newly created seams.

### 8. Mocking infrastructure so heavily that tests stop protecting reality
- Why it happens: once abstractions are introduced, every dependency gets mocked because it is easy and fast.
- How it shows up in practice: tests pass while real DI registration, EF mappings, serialization, retries, auth, or middleware behavior fail in production-like runs.
- How to avoid it: keep a thin but meaningful integration suite with real host bootstrapping, real configuration binding, and realistic database behavior for critical paths.

### 9. Writing brittle golden-path tests that freeze implementation details
- Why it happens: teams new to brownfield testing often assert internal call order, exact log text, or incidental DTO shapes because observable outputs are harder to capture.
- How it shows up in practice: harmless refactors cause test churn, developers stop trusting tests, and the suite gets bypassed during urgent changes.
- How to avoid it: assert outcomes at the module boundary: result objects, persisted state, emitted commands, HTTP responses, or scheduled side effects that actually matter.

## Boundary-Cleanup Pitfalls

### 10. Pulling domain logic out of infrastructure without moving orchestration out of hosts
- Why it happens: teams focus on “domain purity” while controllers, jobs, or Program startup still coordinate half the workflow.
- How it shows up in practice: the Domain project looks cleaner, but real business behavior remains trapped in controllers, Quartz jobs, background services, or static helpers.
- How to avoid it: first make hosts thin. Each controller, job, or startup task should call one application use case that owns the workflow.

### 11. Leaving shared utility code as an escape hatch
- Why it happens: extracting a true module owner for cross-cutting helpers is harder than keeping a common utility package.
- How it shows up in practice: new business rules keep leaking into helper classes, modules depend on shared code for domain decisions, and boundaries erode again within weeks.
- How to avoid it: allow shared utilities only for technical concerns. If a helper contains business branching, move it into the owning module and expose an explicit contract.

### 12. Cleaning boundaries without cleaning data ownership
- Why it happens: code boundaries are easier to change than database ownership, so teams stop at service-layer cleanup.
- How it shows up in practice: modules still read and write each other’s tables directly, test setup remains global and fragile, and “independent” modules cannot evolve safely.
- How to avoid it: document which module owns each aggregate and table, introduce read models where needed, and treat direct cross-module writes as boundary violations.

## Host/Configuration Pitfalls

### 13. Moving logic into DI registration and startup code
- Why it happens: incremental refactors often hide complexity in Program.cs, extension methods, or registration lambdas because that seems less invasive than changing runtime code.
- How it shows up in practice: startup order becomes fragile, service registration has side effects, and tests need full host boot just to validate simple rules.
- How to avoid it: keep composition roots limited to wiring, validated options, and startup tasks. Business decisions should live in application services, not registration code.

### 14. Binding configuration globally instead of per module
- Why it happens: existing systems often centralize configuration in large settings objects or static access patterns, and the refactor preserves that shape for convenience.
- How it shows up in practice: modules depend on settings they do not own, config changes break unrelated areas, and tests need oversized appsettings payloads to run.
- How to avoid it: bind options per module, validate them at startup, and inject only the narrow settings each slice requires.

### 15. Ignoring host-specific behavior while “standardizing” architecture
- Why it happens: teams try to apply one clean architecture pattern uniformly across web endpoints, background jobs, console commands, and Blazor UI without respecting lifecycle differences.
- How it shows up in practice: scoped services leak into singletons, background jobs read request-scoped assumptions, and configuration reload behavior differs across hosts in surprising ways.
- How to avoid it: treat each host as a thin adapter with explicit lifecycle rules, and test the same use case through the host types that actually invoke it.

## Success Signals

- A changed feature can usually be implemented inside one module slice without editing unrelated hosts or shared helpers.
- Controllers, jobs, and startup code mostly delegate to one application use case each instead of orchestrating work inline.
- Critical flows have a small, reliable integration suite that boots the host and catches wiring, config, and persistence regressions.
- New architecture rules are executable through tests or project references, not just written in docs.
- Refactor PRs get smaller over time because seams are improving, not larger because every change still ripples across the system.
- The team can pause after any slice and still ship, rollback, or continue later without a rewrite-sized dependency on unfinished cleanup.