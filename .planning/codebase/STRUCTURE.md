# Structure

Mapped: 2026-04-27

## Repository Layout

- `src\` contains application code and libraries.
- `test\` contains xUnit-based test projects.
- `docs\` contains user-facing documentation and screenshots.
- `docker\`, `podman\`, `helm\`, `qinglong\`, and `tencentScf\` hold deployment or platform-specific assets.
- `scripts\` contains local developer scripts such as `scripts\ut.ps1`.
- `bruno\` contains API request collections for manual exploration.
- `coveragereport\` contains generated HTML coverage output and should be treated as build output, not source.

## Source Tree Breakdown

- `src\Ray.BiliBiliTool.Web` is the primary web host and scheduler entry point.
- `src\Ray.BiliBiliTool.Web.Client` is the Blazor WebAssembly client project.
- `src\Ray.BiliBiliTool.Console` is the CLI/worker host.
- `src\Ray.BiliBiliTool.Agent` houses external API client code, DTOs, handlers, and integration helpers.
- `src\Ray.BiliBiliTool.Application` and `src\Ray.BiliBiliTool.Application.Contracts` contain application-level service logic and public contracts.
- `src\Ray.BiliBiliTool.Domain` and `src\Ray.BiliBiliTool.DomainService` separate entities from task orchestration.
- `src\Ray.BiliBiliTool.Infrastructure` and `src\Ray.BiliBiliTool.Infrastructure.EF` hold utilities, persistence, and startup initialization.
- `src\BlazingQuartz.Core`, `src\BlazingQuartz.Jobs`, and `src\BlazingQuartz.Jobs.Abstractions` support scheduling abstractions and UI.

## Naming Conventions In The Tree

- Projects are mostly prefixed with `Ray.BiliBiliTool.` to reflect layer ownership.
- Extension methods are commonly centralized under `Extensions\ServiceCollectionExtension.cs` or `Extensions\ServiceCollectionExtensions.cs`.
- Tests follow a `{Feature}Test` or `{Feature}DomainServiceTest` naming pattern under `test\`.
- Job classes are named after the task they execute and grouped under the web host.

## Root-Level Control Files

- `Ray.BiliBiliTool.sln` is the main solution entry.
- `Directory.Packages.props` centralizes package versions.
- `common.props` holds shared author/version metadata.
- `Dockerfile` provides container packaging at the repo root.
- `README.md` is both product overview and operations guide.

## Test Tree Breakdown

- `test\AppServiceTest` exercises application service behavior.
- `test\BiliAgentTest` targets API client behavior.
- `test\ConfigTest` covers configuration handling.
- `test\DomainServiceTest` focuses on domain-service orchestration.
- `test\InfrastructureTest` covers infrastructure behavior.
- `test\LogTest` covers logging-related behavior.
- `test\Ray.BiliBiliTool.Agent.FunctionalTests` suggests functional or host-level integration coverage.

## Generated And Noisy Directories

- `src\**\obj\` and `src\**\bin\` are present in the workspace and pollute searches.
- `coveragereport\` contains generated HTML and JS assets.
- Some generated docs and HTML artifacts also exist at the repo root.
- Any future codebase scans should explicitly prefer source and test directories over generated outputs.

## Operational Assets By Area

- Container docs and sample compose files are in `docker\README.md` and `docker\sample\docker-compose.yml`.
- QingLong task defaults live in `qinglong\DefaultTasks\`.
- Tencent SCF packaging is in `tencentScf\serverless.yml` plus helper scripts.
- GitHub automation is under `.github\workflows\` and issue/PR templates under `.github\ISSUE_TEMPLATE\` and `.github\PULL_REQUEST_TEMPLATE.md`.

## Practical Entry Points

- Start from `src\Ray.BiliBiliTool.Web\Program.cs` for the current web architecture.
- Start from `src\Ray.BiliBiliTool.Console\Program.cs` for host-level automation flows and config loading.
- Start from `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs` for integration wiring.
- Start from `src\Ray.BiliBiliTool.Web\Extensions\ServiceCollectionQuartzConfiguratorExtensions.cs` for scheduled task mapping.