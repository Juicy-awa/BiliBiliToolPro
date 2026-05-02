# Refactor Architecture Research

## Target Shape

Keep the product as a modular monolith with two thin composition roots: Web for HTTP, Blazor, and Quartz; Console for command-line or worker execution. Do not split into microservices and do not add a new framework layer.

Use feature modules as the main unit of change: Account, DailyTask, Manga, Live, Charge, Admin, and Scheduler orchestration. Each module should expose application use cases and internalize its implementation details.

Within each module, target this shape:
- Domain: entities, value objects, invariants, domain policies.
- Application: use-case handlers, orchestration, transaction boundary, ports.
- Adapters: EF repositories, Agent HTTP clients, config readers, notifications.

For the current solution, this means:
- Keep `Ray.BiliBiliTool.Web` and `Ray.BiliBiliTool.Console` as hosts only.
- Treat `Ray.BiliBiliTool.Agent` as an outbound adapter layer, not business orchestration.
- Move use-case coordination out of host code and out of generic cross-cutting services into module-level application services.
- Stop growing `Ray.BiliBiliTool.DomainService` as a catch-all. Keep only true domain policies there, then either fold them into Domain or keep them as module-internal policy services.

Prefer logical modularization before physical explosion. Start by organizing namespaces, folders, and DI registration by module inside the existing projects. Add or split assemblies only when boundaries are stable and enforced.

## Boundary Rules

- Hosts may depend on module registration and application contracts, but not directly on EF or Bilibili client details.
- Quartz jobs, controllers, and Razor components should call one application use case each. They should not compose multiple infrastructure services inline.
- Application code may depend on Domain and on port interfaces, but not on concrete Agent, EF, or web types.
- Domain code must not depend on Web, Quartz, EF, HTTP clients, configuration providers, or UI DTOs.
- Agent DTOs stay in the adapter boundary. Map them to application or domain models before business logic uses them.
- Shared helpers are allowed only for technical concerns. If a helper encodes business rules, move it into the owning module.
- Cross-module calls should go through explicit contracts or application facades, not by reaching into another module's service collection or data access code.

## Dependency Direction

Target dependency flow should be inward:

`Web/Console/Quartz -> Application -> Domain <- Infrastructure/Agent implementations`

Build-order implications:
- Lowest-level projects should be Domain and Application.Contracts.
- Application depends on Domain and Contracts.
- Infrastructure.EF and Agent depend on Application contracts or domain abstractions they implement.
- Web and Console depend on Application plus adapter registrations, but avoid direct references to low-level implementation details when possible.

Practical rule: if changing an EF or HTTP client class forces recompilation of large parts of application logic, the boundary is still wrong. The build graph should isolate adapters from use-case code.

Do not try to fix this by creating many new projects immediately. First remove forbidden references and introduce ports. Then split assemblies only where the build graph proves the seam is real.

## Host Composition Guidance

The current startup density in `Program.cs` should be reduced to four concerns only: configuration, module registration, platform middleware, and startup tasks.

Recommended composition pattern:
- Each module exposes one registration entry point such as `AddDailyTaskModule()`.
- Registration methods wire the module's application services and adapter implementations together.
- Hosts select which modules to load, but do not know the module internals.
- Startup side effects such as database migration, seed data, scheduler bootstrap, and config backfills should run through explicit startup tasks, not inline `.Wait()` or ad hoc service resolution.

For this repo, keep Quartz in the Web host, but move job registration and job-to-use-case mapping behind a scheduler module boundary. The host should know schedules and hosted service wiring, not task business rules.

To avoid destabilizing runtime behavior, preserve existing configuration keys, scheduler identities, HTTP routes, and host startup order while moving internals behind module facades.

## Execution Flow Seams

Create seams around the flows that currently mix orchestration and IO:
- Login/session bootstrap
- Daily task execution
- Manga and live task execution
- Charge and reward flows
- Scheduler-triggered execution

Target flow per use case:
1. Inbound adapter receives trigger: controller, Razor action, console command, or Quartz job.
2. Application handler loads required state through ports.
3. Domain logic decides what should happen.
4. Adapters perform HTTP, persistence, or notification side effects.
5. Application returns a result object suitable for logging and tests.

This creates stable seams for refactor: jobs become thin triggers, Agent calls become replaceable adapters, and persistence stops leaking into orchestration code.

## Testing Seams

Protect behavior before moving code. For this codebase, the best seams are observable flows, not individual private methods.

- Characterization tests for current daily task, login, and scheduler-driven flows.
- Application-level tests against use-case handlers with fake ports.
- Adapter tests for Agent clients and EF repositories.
- Host-level smoke tests for Web and Console composition.
- Architecture tests that fail when Web references forbidden infrastructure internals or when Application depends on web or agent types.

Weak test seams usually come from ambient state, static helpers, and broad DI resolution. Replace those with explicit request objects, result objects, and injected ports one flow at a time.

## Suggested Migration Order

1. Freeze runtime behavior with characterization tests around the highest-risk flows and current Quartz jobs.
2. Introduce module-level application facades for one vertical slice, starting with DailyTask or Login because they exercise scheduler, config, HTTP, and persistence together.
3. Redirect the corresponding job, controller, or console entry point to the new application facade without changing observable behavior.
4. Pull Agent and EF usage behind ports owned by that module; keep the old concrete implementations underneath.
5. Remove direct host-to-infrastructure wiring for that slice and replace it with a single module registration call.
6. Add architecture tests and project-reference cleanup so the new boundary cannot regress.
7. Repeat slice by slice for Manga, Live, Charge, and Admin.
8. After multiple slices are stable, shrink or retire the parts of DomainService that are only acting as orchestration glue.

Stability rules during migration:
- One slice at a time.
- Keep public routes, schedules, config keys, and DTO contracts stable unless a change is intentional and covered.
- Prefer parallel seams over big renames: add the new application path, switch callers, then delete the old path.
- Do not move multiple hosts and multiple modules in the same step.

The winning migration shape here is not a clean-slate layered rewrite. It is a controlled refactor toward thin hosts, module-owned application flows, inward dependencies, and build-enforced seams that let Web, Console, Blazor, Agent, EF, and Quartz keep working while the internals become easier to change.