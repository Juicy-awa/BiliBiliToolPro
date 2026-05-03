# Concerns

Mapped: 2026-04-27

## Security Concerns

- `src\Ray.BiliBiliTool.Infrastructure.EF\DbInitializer.cs` seeds a default admin user with username `admin` and password `BiliTool@2233`. Even if intended for first-run convenience, this is a real security concern for any deployed web instance.
- The web and console hosts rely on cookies, secrets, and environment variables across several sources, so accidental leakage through logs or config files is a meaningful operational risk.

## Startup And Reliability Concerns

- `src\Ray.BiliBiliTool.Web\Program.cs` blocks on async database initialization using `.Wait()`, which is a common deadlock and startup-latency smell.
- `builder.Services.AddMudServices()` is called twice in `src\Ray.BiliBiliTool.Web\Program.cs`, which is harmless in many cases but indicates duplicated composition-root wiring.
- Retry policy in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs` only retries once, which may be too shallow for flaky external APIs.

## Error-Handling Concerns

- Generic `Exception` is thrown broadly across the agent and domain-service layers, including `src\Ray.BiliBiliTool.Agent\BiliCookie.cs`, `src\Ray.BiliBiliTool.DomainService\VideoDomainService.cs`, and `src\Ray.BiliBiliTool.DomainService\VipBigPointDomainService.cs`.
- This makes it harder to distinguish recoverable external failures from domain invariants and validation failures.

## Technical-Debt Signals

- Inline TODOs remain in core paths such as:
  - `src\Ray.BiliBiliTool.DomainService\ArticleDomainService.cs`
  - `src\Ray.BiliBiliTool.DomainService\AccountDomainService.cs`
  - `src\Ray.BiliBiliTool.DomainService\LoginDomainService.cs`
  - `src\Ray.BiliBiliTool.Agent\BiliBiliAgent\Dtos\Live\*.cs`
- `src\Ray.BiliBiliTool.Web\Components\Comps\BlazingTrigger.razor.cs` still throws `NotImplementedException`, which is a direct incomplete implementation marker.

## Testing Concerns

- Sampled tests are shallow. `test\DomainServiceTest\ArticleDomainServiceTest.cs` boots the host but does not assert behavior.
- There is limited visible evidence of deterministic integration test infrastructure, fixture reuse, or isolated mocking strategy.

## Repository Hygiene Concerns

- Generated content such as `coveragereport\`, `src\**\obj\`, `src\**\bin\`, and root-level generated HTML files live in the workspace and interfere with searches and reviews.
- This increases the chance of false positives when auditing TODOs, exceptions, or integration references.

## Architecture Concerns

- `src\Ray.BiliBiliTool.Web\Program.cs` has become a dense startup file that mixes configuration, persistence, logging, scheduling, controllers, components, Swagger, and startup migration behavior.
- The console and web hosts duplicate some service-registration responsibilities instead of sharing a more explicit composition abstraction.

## Operational Concerns

- The repo now includes a local GSD installation under `.github\`, but the SDK query path is not available from that local install alone. Future GSD workflows that assume `gsd-sdk` exists may need a global SDK install or runtime-aware fallback.
- `scripts\ut.ps1` installs a global tool on each use, which can be noisy or brittle in constrained environments.

## Recommended First Follow-Ups

- Remove or force-change the seeded default admin credential path in `src\Ray.BiliBiliTool.Infrastructure.EF\DbInitializer.cs`.
- Replace broad `Exception` throws with typed failures in the agent and domain-service layers.
- Make startup initialization fully async in the web host.
- Strengthen one representative test slice with real assertions before expanding the suite.