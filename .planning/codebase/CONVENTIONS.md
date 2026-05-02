# Conventions

Mapped: 2026-04-27

## Service Registration Style

- Dependency injection is the dominant composition mechanism.
- Each layer tends to expose one `Add*` extension method from an `Extensions` folder rather than registering services inline everywhere.
- Examples:
  - `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`
  - `src\Ray.BiliBiliTool.Infrastructure.EF\Extensions\ServiceCollectionExtension.cs`
  - `src\Ray.BiliBiliTool.Web\Extensions\ServiceCollectionExtension.cs`

## Configuration Style

- Configuration is layered and environment-aware.
- Console host conventions in `src\Ray.BiliBiliTool.Console\Program.cs` load JSON, environment-specific JSON, user secrets, prefixed env vars, full env vars, command-line args, and `cookies.json`.
- Web host conventions in `src\Ray.BiliBiliTool.Web\Program.cs` add `config/cookies.json` and optional SQLite-backed configuration records.
- Option binding appears to be used for operational settings such as security and QingLong configuration.

## Naming Style

- Namespaces and projects follow PascalCase and are usually aligned to directory names.
- Types describing actions and tasks are explicit: `LoginJob`, `DailyJob`, `DbInitializer`, `ArticleDomainServiceTest`.
- Test projects and files use suffixes like `Test`, `Tests`, or `DomainServiceTest`.
- Interface names follow standard `I*` conventions for API clients and services.

## HTTP Client Pattern

- Typed HTTP clients are created via `AddHttpApi<TInterface>` instead of raw `HttpClient` use.
- Delegating handlers are discovered by assembly scanning.
- Retry policy is applied centrally with Polly rather than ad hoc at each call site.
- Host-specific request headers, especially user agents, are injected during client configuration.

## Error Handling Patterns

- There is a strong reliance on throwing generic `Exception` with message payloads in domain and agent code, for example in `src\Ray.BiliBiliTool.DomainService\VideoDomainService.cs` and `src\Ray.BiliBiliTool.Agent\BiliCookie.cs`.
- Startup code wraps the main host lifecycle in a top-level `try/catch` and logs fatal failures in both `src\Ray.BiliBiliTool.Web\Program.cs` and `src\Ray.BiliBiliTool.Console\Program.cs`.
- Some known issues are left as inline `todo` comments in source rather than tracked through explicit typed exceptions or backlog markers.

## Async And Startup Style

- Async is used broadly in hosts and EF initialization, but the web host still blocks on startup with `dbInitializer.InitializeAsync().Wait()` in `src\Ray.BiliBiliTool.Web\Program.cs`.
- Console entry uses `async Task<int>` and clean async shutdown with `Log.CloseAndFlushAsync()`.

## Logging And Observability Style

- Serilog is the standard logging mechanism.
- Request logging is enabled in the web host with `app.UseSerilogRequestLogging()`.
- SQLite logging is added for the web app in `src\Ray.BiliBiliTool.Web\Program.cs`.
- Multiple remote notification sinks are wired as operational outputs rather than separate observability services.

## Test Conventions

- xUnit is the test framework across sampled test projects.
- Coverage is collected through Coverlet and transformed via ReportGenerator using `scripts\ut.ps1`.
- Sample tests are currently lightweight; `test\DomainServiceTest\ArticleDomainServiceTest.cs` only boots the console host in its constructor and contains no assertions.

## Documentation And Language Mix

- The repository mixes Chinese and English in source comments, README content, and identifiers.
- Comments are pragmatic and often operations-focused rather than heavily architectural.
- User-facing docs live close to the code and deployment assets rather than in a separate docs site.