# Testing

Mapped: 2026-04-27

## Test Frameworks

- Unit and functional tests use xUnit, as shown in `test\DomainServiceTest\DomainServiceTest.csproj` and `test\Ray.BiliBiliTool.Agent.FunctionalTests\Ray.BiliBiliTool.Agent.FunctionalTests.csproj`.
- Coverage is collected with `coverlet.collector`.
- `FluentAssertions` is present at least in `test\Ray.BiliBiliTool.Agent.FunctionalTests\Ray.BiliBiliTool.Agent.FunctionalTests.csproj`.

## Test Project Inventory

- `test\AppServiceTest\AppServiceTest.csproj`
- `test\BiliAgentTest\BiliAgentTest.csproj`
- `test\ConfigTest\ConfigTest.csproj`
- `test\DomainServiceTest\DomainServiceTest.csproj`
- `test\InfrastructureTest\InfrastructureTest.csproj`
- `test\LogTest\LogTest.csproj`
- `test\Ray.BiliBiliTool.Agent.FunctionalTests\Ray.BiliBiliTool.Agent.FunctionalTests.csproj`

## Current Test Style

- Tests appear to be project-sliced by layer rather than grouped under one omnibus suite.
- Some tests use host bootstrapping as setup instead of narrower unit seams.
- Example: `test\DomainServiceTest\ArticleDomainServiceTest.cs` constructs the console host in the test class constructor and currently contains no assertions.
- Functional tests reference the console host project directly, which suggests end-to-end or integration-style setup rather than isolated mocking.

## Tooling And Execution

- `scripts\ut.ps1` is the primary local testing helper.
- The script installs `dotnet-reportgenerator-globaltool`, runs `dotnet test`, gathers coverage files recursively, and writes HTML output to `coveragereport`.
- The script ends by opening `coveragereport/index.htm`, which is convenient locally but not CI-oriented.

## CI Coverage

- The sampled GitHub workflow file `.github\workflows\codeql-analysis.yml` covers static analysis rather than test execution.
- No dedicated test workflow was inspected in this pass, so automated CI test enforcement is not obvious from the core files read.

## Strengths

- The repository has multiple test projects rather than a single neglected test folder.
- Standard .NET tooling is used, making local execution straightforward.
- Coverage artifacts already exist in the repo, which indicates testing has been run recently enough to leave generated reports behind.

## Gaps And Risks

- Sampled tests are very thin and may serve more as smoke checks than behavior verification.
- Heavy host bootstrapping can make tests slower and blur failure causes.
- The presence of generated `TestResults\` and coverage output in the tree suggests cleanup boundaries are loose.
- No strong evidence of mocking conventions, fixtures, or integration test harness patterns emerged from the sampled files.

## Good Entry Points For Expanding Tests

- Domain orchestration: `test\DomainServiceTest\` paired with `src\Ray.BiliBiliTool.DomainService\`.
- HTTP client behavior: `test\BiliAgentTest\` paired with `src\Ray.BiliBiliTool.Agent\`.
- Persistence and initialization: `test\InfrastructureTest\` paired with `src\Ray.BiliBiliTool.Infrastructure.EF\`.