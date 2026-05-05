# Phase 15-01 Summary: ISchedulerPageWorkflow + Schedules Refactor

## What Was Built
- `SchedulerActionResult` sealed record DTO
- `ISchedulerPageWorkflow` interface (web-layer contract for Schedules page)
- `SchedulerPageWorkflow` concrete implementation wrapping `ISchedulerService` + `IExecutionLogService`
- `Schedules.razor.cs` refactored to inject only `ISchedulerPageWorkflow` (removed direct `ISchedulerService`/`IExecutionLogService`)
- DI registrations added in `ServiceCollectionExtension.cs`

## Files Modified
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/SchedulerActionResult.cs` (new)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/ISchedulerPageWorkflow.cs` (new)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/SchedulerPageWorkflow.cs` (new)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/Schedules.razor.cs` (modified)
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` (modified)

## Key Decisions
- D-01: Web-layer workflow seam — `ISchedulerPageWorkflow` wraps BlazingQuartz services
- D-02: Guard checks in workflow for null TriggerName/JobName returning `SchedulerActionResult(false, message)` instead of throwing
- `SchedulerPageWorkflow` uses primary constructor injection

## Build Status
✅ 0 errors, pre-existing warnings only
