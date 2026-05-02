# Refactor Workstreams Research

Context: brownfield refactor roadmap for a mature modular-monolith-style automation system.
Goal: reduce cost of change without destabilizing existing production-like behavior.

## Table Stakes

These are the baseline workstreams. If they are missing, the program usually turns into architecture churn or a stalled rewrite.

| Workstream / Deliverable | Why it is table stakes | Safe staging guidance |
|---|---|---|
| Outcome alignment and success metrics | Brownfield refactors fail when teams optimize for "cleaner code" instead of faster, safer change. Define target outcomes such as lead time, defect rate, hot spots, and time-to-test. | Lock a small scorecard before moving code. Review it every milestone. |
| Current-state architecture map | Mature systems usually hide coupling in schedulers, shared helpers, configuration, and data access. A thin dependency map and runtime flow map are mandatory. | Map only the critical flows first: login/session, scheduled task execution, external API calls, persistence. |
| Seam identification | Successful programs create seams before they extract modules. Typical seams are interfaces, adapters, event/router points, repository boundaries, and composition roots. | Introduce seams with behavior-preserving refactors first. No logic moves in the same change if avoidable. |
| Characterization test suite for critical flows | Weak tests are the main brownfield tax. Characterization tests freeze behavior so refactors can proceed safely. | Start with observable end-to-end or slice tests around the highest-risk flows, not broad unit-test campaigns. |
| Dependency rule enforcement | Boundary rules must become executable or they decay immediately. | Add architecture tests or build checks early; fail new violations first, then ratchet down debt. |
| Transitional architecture plan | Temporary adapters, facades, and anti-corruption layers are normal in successful refactors. Pretending they are unnecessary drives big-bang changes. | Track each temporary component with a removal condition and owner. |
| Incremental rollout and fallback strategy | Every major change needs a reversible release path. | Prefer branch-by-abstraction, feature toggles, side-by-side implementations, and small cutovers. |
| Observability for refactor paths | Teams need to see whether the new path behaves like the old one. Logs, traces, counters, and diff checks are mandatory. | Instrument before switching traffic. Keep old vs new comparison data during rollout. |
| Delivery operating model | Refactor work cannot be separated from normal feature delivery for long. It needs explicit capacity and governance. | Reserve a stable percentage of each milestone for enabling work and debt retirement. |
| Exit criteria per slice | Each slice needs a definition of done stronger than "code moved." | Require tests, dependency compliance, observability, docs, and a rollback path before declaring a slice complete. |

## High-Leverage Improvements

These are not the absolute minimum, but they strongly increase the odds that a modular-monolith refactor finishes with lasting gains.

| Improvement / Deliverable | Why it pays off | Suggested use |
|---|---|---|
| Change hotspot analysis | Helps choose the first refactor slices based on churn, incidents, and dependency pain rather than intuition. | Use commit history and bug history to rank flows before planning milestones. |
| Module scorecards | Makes "clearer boundaries" measurable through dependency count, public surface area, test coverage on critical paths, and ownership clarity. | Review per module at milestone boundaries. |
| Golden-master or snapshot protection for legacy payloads | Useful when outbound API payloads, config shapes, or scheduler orchestration are too awkward for many small assertions. | Apply only to payload-heavy or especially fragile flows. |
| Side-by-side verification harness | Lets old and new implementations run in parallel and compare results before full cutover. | Use for external API clients, orchestration services, and calculation-heavy paths. |
| Module ownership and decision log | Boundary clarity is organizational as much as technical. Teams need to know who owns which contracts. | Record module intent, allowed dependencies, and open exceptions. |
| Refactor-safe developer workflow | Fast local test loops, targeted integration tests, and consistent review checklists reduce regression risk. | Standardize a small review checklist for seam changes and dependency changes. |
| Deferred cleanup backlog tied to seams | Transitional code tends to linger unless cleanup is planned at the time it is introduced. | Create explicit teardown tickets whenever temporary adapters or toggles are added. |
| Fitness functions in CI | Prevents backsliding after the first cleanup wave. | Automate checks for layering rules, forbidden references, and key test suites. |

## Anti-Features

These repeatedly show up in failed brownfield programs and should be excluded from the roadmap.

| Anti-feature | Why to avoid it | Better alternative |
|---|---|---|
| Big-bang rewrite milestone | Maximizes scope, delays feedback, and removes rollback options. | Break work into thin vertical slices with live coexistence. |
| Full feature parity as a prerequisite | Teams spend months reproducing accidental legacy behavior and hidden edge cases. | Preserve only behavior required by critical flows and observable contracts. |
| Framework swap as the main objective | Replacing the stack rarely fixes coupling or weak tests by itself. | Keep platform choices stable while improving boundaries and tests first. |
| Repository-wide test rewrite | Consumes time without protecting the riskiest areas soon enough. | Start with characterization and slice tests around high-value flows. |
| Massive namespace/project reshuffle upfront | Produces noise and merge pain before boundaries are enforceable. | Introduce dependency rules, then move one slice at a time. |
| Shared "common" expansion | Central utility layers often become new coupling magnets. | Prefer explicit module contracts and narrowly scoped adapters. |
| Long-lived parallel architecture with no retirement plan | Temporary code becomes permanent and doubles maintenance cost. | Add removal triggers and deadline-based cleanup reviews. |
| Microservices split during boundary cleanup | Distributed complexity hides the real problem and slows delivery. | Prove modular boundaries inside the monolith first. |
| Refactor-only branch that diverges from trunk | Delays validation against real change pressure. | Keep changes incremental on trunk with toggles or abstraction seams. |

## Dependency Notes

- Suggested dependency order: outcomes and metrics -> flow map -> seam creation -> characterization tests -> dependency enforcement -> first extraction/cutover.
- Observability should precede any traffic switch. If old and new paths cannot be compared, the rollout is not ready.
- Transitional architecture is a dependency, not a smell, when it creates safe coexistence and rollback.
- Organizational ownership must follow boundary work. A module without a clear owner usually regresses.
- Each new seam should unlock at least one of three things: testing, observation, or traffic redirection.
- Critical reports, scheduled jobs, and external integrations often depend on hidden data paths; treat them as first-class discovery items before extraction.
- In a production-like codebase, "done" means reversible in deployment terms, not only compilable in source terms.

## Suggested First Milestone Outcomes

Aim for one milestone that proves the refactor program can deliver safer change, not one that claims to finish the architecture.

1. A published refactor scorecard with 3-5 measurable outcomes tied to cost of change.
2. A current-state map of the top 2-3 critical flows and their hidden dependencies.
3. Characterization tests around at least one critical orchestration path and one external integration path.
4. One enforced dependency rule set that blocks new boundary violations in CI.
5. One explicit seam introduced in a high-churn area with no intended behavior change.
6. Basic observability added for the chosen slice so old vs new execution can be compared.
7. A thin transitional architecture decision for the first extraction path, including rollback and removal criteria.
8. A ranked backlog of follow-on slices based on hotspot data, operational risk, and team readiness.

Bottom line: the roadmap should treat tests, seams, dependency rules, observability, and rollout safety as product-grade deliverables. For a brownfield modular monolith, those are the actual features that make future delivery faster.

Sources informing this summary: Martin Fowler on Strangler Fig, Legacy Seam, Branch by Abstraction, and Thoughtworks' Patterns of Legacy Displacement, especially transitional architecture and incremental delivery.