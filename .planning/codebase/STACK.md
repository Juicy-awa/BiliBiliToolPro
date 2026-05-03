# Stack

Mapped: 2026-04-27

## Runtime And Languages

- Primary language: C# across a multi-project .NET solution in `Ray.BiliBiliTool.sln`.
- Main target framework: `net8.0` in the app hosts and test projects, including `src\Ray.BiliBiliTool.Web\Ray.BiliBiliTool.Web.csproj` and `src\Ray.BiliBiliTool.Console\Ray.BiliBiliTool.Console.csproj`.
- Frontend stack: Blazor with server and WebAssembly interactivity in `src\Ray.BiliBiliTool.Web\Program.cs` plus a separate client app in `src\Ray.BiliBiliTool.Web.Client\Program.cs`.
- Shell and automation assets exist alongside the .NET codebase: PowerShell in `scripts\ut.ps1`, Bash in `docker\`, `qinglong\`, and `tencentScf\`, plus Helm and Podman deployment assets at the repo root.

## Solution Composition

- UI hosts: `src\Ray.BiliBiliTool.Web`, `src\Ray.BiliBiliTool.Web.Client`, `src\Ray.BiliBiliTool.Console`.
- Domain and application layers: `src\Ray.BiliBiliTool.Application`, `src\Ray.BiliBiliTool.Application.Contracts`, `src\Ray.BiliBiliTool.Domain`, `src\Ray.BiliBiliTool.DomainService`.
- Infrastructure layers: `src\Ray.BiliBiliTool.Infrastructure`, `src\Ray.BiliBiliTool.Infrastructure.EF`, `src\Ray.BiliBiliTool.Config`.
- API/integration layer: `src\Ray.BiliBiliTool.Agent`.
- Scheduling libraries: `src\BlazingQuartz.Core`, `src\BlazingQuartz.Jobs`, `src\BlazingQuartz.Jobs.Abstractions`.

## Dependency Management

- NuGet versions are centrally managed in `Directory.Packages.props` with `ManagePackageVersionsCentrally=true`.
- Shared repo metadata is minimal and lives in `common.props` for author/version defaults.
- The solution uses project references heavily instead of package boundaries between internal layers.

## Core Libraries

- Hosting and DI: `Microsoft.Extensions.*` packages from `Directory.Packages.props`.
- ORM and storage: Entity Framework Core, SQLite, and Npgsql packages from `Directory.Packages.props`.
- Scheduling: Quartz packages plus `AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite`.
- HTTP resilience: `Microsoft.Extensions.Http.Polly` and explicit Polly usage in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.
- Typed HTTP clients: `WebApiClientCore`.
- UI packages: `MudBlazor`, Razor Components, and Blazor WebAssembly packages.
- Logging: Serilog core plus multiple custom sink packages.
- API documentation: `Swashbuckle.AspNetCore`.
- Tests: xUnit, `Microsoft.NET.Test.Sdk`, `coverlet.collector`, and `FluentAssertions`.

## Host-Specific Configuration

- Web host uses ASP.NET Core Web SDK and user secrets in `src\Ray.BiliBiliTool.Web\Ray.BiliBiliTool.Web.csproj`.
- Console host is a long-running `Microsoft.Extensions.Hosting` executable with JSON, secrets, env var, command-line, and cookie-file configuration in `src\Ray.BiliBiliTool.Console\Program.cs`.
- Web client is intentionally thin and only registers MudBlazor in `src\Ray.BiliBiliTool.Web.Client\Program.cs`.

## Operational Tooling

- Docker assets: `Dockerfile`, `docker\`, and Podman examples in `podman\`.
- Kubernetes packaging: `helm\`.
- Serverless packaging: `tencentScf\`.
- QingLong automation support: `qinglong\`.
- API collections for manual testing: `bruno\`.

## Build And Quality Automation

- Code scanning: `.github\workflows\codeql-analysis.yml`.
- Coverage generation script: `scripts\ut.ps1` installs `dotnet-reportgenerator-globaltool`, runs `dotnet test`, and writes HTML output to `coveragereport`.
- The repository already contains generated outputs and build artifacts such as `coveragereport\` and `src\**\obj\`, which influence search noise.