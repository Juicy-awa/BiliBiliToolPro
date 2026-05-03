# Architecture

Mapped: 2026-04-27

## High-Level Shape

- This is a layered brownfield .NET monorepo with three main executable surfaces: a console worker, an ASP.NET Core Blazor web app, and a Blazor WebAssembly client.
- The core business behavior is organized around Bilibili task automation, with scheduling, typed HTTP clients, domain services, and persistence composed through dependency injection.
- The web host is now the richest composition root; the console host remains a simpler automation-oriented entry point.

## Composition Roots

- Web composition root: `src\Ray.BiliBiliTool.Web\Program.cs`.
- Console composition root: `src\Ray.BiliBiliTool.Console\Program.cs`.
- WebAssembly client bootstrap: `src\Ray.BiliBiliTool.Web.Client\Program.cs`.

## Layer Responsibilities

- `src\Ray.BiliBiliTool.Agent`: outbound HTTP clients, handlers, API DTOs, and QingLong connectivity.
- `src\Ray.BiliBiliTool.Application` and `src\Ray.BiliBiliTool.Application.Contracts`: application service layer and cross-layer contracts.
- `src\Ray.BiliBiliTool.Domain` and `src\Ray.BiliBiliTool.DomainService`: domain entities and task-oriented orchestration logic.
- `src\Ray.BiliBiliTool.Infrastructure`: lower-level support code, helpers, and cookie utilities.
- `src\Ray.BiliBiliTool.Infrastructure.EF`: EF Core persistence and bootstrap initialization.
- `src\Ray.BiliBiliTool.Web`: HTTP API endpoints, Razor components, auth/web services, and Quartz job scheduling.
- `src\BlazingQuartz.*`: scheduling UI and job abstractions layered around Quartz.

## Dependency Wiring Pattern

- Internal layers are wired through extension methods instead of massive startup classes.
- Representative extension points:
  - `src\Ray.BiliBiliTool.Application\Extensions\ServiceCollectionExtension.cs`
  - `src\Ray.BiliBiliTool.DomainService\Extensions\ServiceCollectionExtensions.cs`
  - `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`
  - `src\Ray.BiliBiliTool.Infrastructure.EF\Extensions\ServiceCollectionExtension.cs`
  - `src\Ray.BiliBiliTool.Web\Extensions\ServiceCollectionExtension.cs`

## Runtime Flow

- Console flow: configuration is assembled, logging is initialized, hosted services start, and task execution proceeds via services registered from the agent, app, config, domain-service, and infrastructure layers in `src\Ray.BiliBiliTool.Console\Program.cs`.
- Web flow: configuration optionally overlays SQLite-backed key/value config, services are registered, Quartz jobs are configured, EF migrations run through `DbInitializer.InitializeAsync()`, then Razor components, controllers, Swagger, and request logging are enabled in `src\Ray.BiliBiliTool.Web\Program.cs`.

## Scheduling Architecture

- Quartz is the task scheduler for the web host.
- Job registration is centralized in `src\Ray.BiliBiliTool.Web\Extensions\ServiceCollectionQuartzConfiguratorExtensions.cs`.
- Current jobs include login, daily tasks, manga tasks, VIP privilege tasks, silver-to-coin conversion, charge, live lottery, live fans medal, unfollow batch, and a test job.
- Quartz persistence uses the ADO job store over SQLite in the web composition root.

## Persistence Architecture

- EF Core is introduced via `AddDbContextFactory<BiliDbContext>()` in `src\Ray.BiliBiliTool.Infrastructure.EF\Extensions\ServiceCollectionExtension.cs`.
- Database initialization and seed logic are in `src\Ray.BiliBiliTool.Infrastructure.EF\DbInitializer.cs`.
- The web app performs migration at startup and seeds a default admin user if none exists.

## Integration Architecture

- Outbound service calls are strongly typed and created through `AddHttpApi<TInterface>` in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.
- Cross-cutting request behavior is attached via delegating handlers and Polly retry policy.
- Cookie injection, WBI-related request handling, and optional proxy support are implemented below the domain-service layer rather than inline in task code.

## UI Architecture

- The web app mixes server-side and WebAssembly interactivity using Razor Components in `src\Ray.BiliBiliTool.Web\Program.cs`.
- MudBlazor is the component library across the interactive UI surfaces.
- Swagger-backed controller APIs coexist with the component UI in the same web host.

## Architectural Tension Points

- The web host mixes many responsibilities in a single `Program.cs` and still contains some synchronous startup calls such as `.Wait()` on async initialization.
- The console and web hosts share broad service registration patterns but not a unified host abstraction.
- Some scheduling and UI concerns are blended through the BlazingQuartz packages, so scheduler behavior is not isolated to one thin module.