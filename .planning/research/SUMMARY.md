# Research Summary

## Recommended Direction

This initiative should stay a phased refactor of the existing .NET 8 modular monolith, not a rewrite and not a framework swap. Keep the current host model, EF Core, Quartz, typed HttpClient usage, and Serilog, then improve the system by tightening module boundaries, moving orchestration into module-level application flows, and making dependency direction executable.

The target shape is thin hosts, explicit module contracts, inward dependencies, and replaceable adapters around Bilibili HTTP, persistence, and notifications. The first slices should center on high-risk flows such as login/session bootstrap, daily task execution, and scheduler-triggered work because those paths touch configuration, scheduling, HTTP integration, and persistence at once.

Planning should assume transitional architecture is required. Introduce seams, facades, mapping layers, and side-by-side paths where needed so each slice is reversible. The program succeeds if delivery gets safer and faster after each slice, not if the repository looks cleaner all at once.

## Table Stakes For This Refactor

- Define milestone scorecards around change safety: lead time, regression rate, hotspot reduction, and time to validate critical flows.
- Map the current runtime paths for login, scheduled task execution, outbound API calls, and persistence before moving code.
- Add characterization tests around the most failure-prone orchestration flows before extraction.
- Enforce dependency rules early so hosts depend on module contracts instead of infrastructure internals.
- Treat adapters and temporary facades as planned assets with rollback and removal criteria.
- Add observability on refactor paths before switching callers or schedules to new code.
- Keep routes, config keys, scheduler identities, and external DTO contracts stable unless a change is intentional and covered.

## High-Leverage Early Wins

- Introduce one module registration entry point per slice so Program startup stops wiring business details directly.
- Create an application facade for DailyTask or Login first; both exercise the most cross-cutting behavior.
- Move Agent and EF usage behind module-owned ports without replacing the underlying implementations yet.
- Add architecture tests to block Web, Quartz jobs, and hosts from reaching into infrastructure internals.
- Add a thin integration suite with real host bootstrapping and realistic configuration binding for critical flows.
- Add HTTP resilience selectively to the outbound clients that have known retry, timeout, or rate-limit risk.
- Use hotspot analysis to rank the next slices instead of following current folder or team ownership.

## Major Risks To Control

- Superficial project or namespace reorganization before behavior seams exist will create noise without reducing coupling.
- Interface-heavy wrappers around existing orchestration will preserve the same complexity with more indirection.
- Module boundaries will remain fiction unless cross-module calls, shared DTO leakage, and direct data access are blocked.
- A unit-test-first strategy will miss the real regression risk in jobs, startup flow, EF mappings, configuration, and HTTP integration.
- Transitional code can become permanent unless each adapter, toggle, or dual path has an exit condition.
- Host startup and DI registration can become the new dumping ground if business decisions are not moved into application use cases.
- Boundary cleanup will stall if data ownership stays global and modules still write across each other's persistence concerns.

## Suggested Planning Principles

- Plan in thin vertical slices that preserve behavior and can ship independently.
- Make hosts thinner before making the domain purer.
- Add seams only where they improve testing, observation, or traffic redirection.
- Prefer logical modularization inside the current solution before splitting more assemblies.
- Pair every structural change with a focused validation path: characterization, integration, or architecture test.
- Sequence work as map critical flow -> freeze behavior -> add seam -> redirect caller -> enforce boundary -> retire legacy path.
- Favor branch-by-abstraction, side-by-side verification, and rollback-ready cutovers over one-step replacements.
- Use temporary architecture deliberately, but record owners and teardown triggers at the time it is introduced.
- Keep refactor work attached to user-visible or operationally meaningful slices so the program remains fundable.

## Planning Implication

The first roadmap phases should establish scorecards, flow maps, characterization coverage, module registration seams, and dependency enforcement, then refactor one high-churn vertical slice end to end. DailyTask and Login are the strongest starting candidates because they provide the best signal on architecture boundaries, code quality, and testability with relatively low-risk incremental delivery.
