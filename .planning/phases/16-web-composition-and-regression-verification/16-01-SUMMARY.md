# Phase 16-01 Summary: ArchUnit Guardrail + Integration Test Extension

## What Was Built

Added the structural architecture guardrail for the Web component boundary (WEB-04) and extended
the Web startup integration test to verify all 6 workflow seam services resolve from DI.

## Artifacts Modified

- `test/Ray.BiliBiliTool.ArchitectureTests/DependencyGuardrailTests.cs`
  - Added `WebComponentLayer` static provider: `Ray.BiliBiliTool.Web.Components` namespace
  - Added `InfrastructureLayers` static provider: `Ray.BiliBiliTool.Infrastructure` namespace
  - Added `[Fact] Web_component_code_behind_classes_should_not_directly_depend_on_infrastructure`

- `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs`
  - Added 4 using statements for Web-layer workflow seam namespaces
  - Appended 6 `GetRequiredService<IXxx>()` assertions to `Web_startup_boots_and_exposes_critical_services`

## Key Decisions Applied

- **D-01/D-02**: ArchUnit rule targets `Infrastructure` not `Domain` — components legitimately
  reference Domain model types (`ExecutionLog`, `BiliLogs`) for display binding; the violation
  is direct Infrastructure dependency
- **D-03**: `.Because("Web component code-behind classes must route Domain and Infrastructure access through Web-layer workflow seams (Phases 13–15)")`
- **D-04/D-05**: 6 new assertions appended to existing test method, not a new method

## Verification Results

- Architecture tests: 5/5 passing (4 original + 1 new guardrail rule)
- Both test projects: 0 compile errors
