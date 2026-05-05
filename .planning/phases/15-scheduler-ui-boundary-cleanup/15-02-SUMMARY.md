# Phase 15-02 Summary: Dialog Workflow Interfaces + Refactors

## What Was Built
- `ILogsDialogWorkflow` interface (web-layer contract for LogsDialog)
- `LogsDialogWorkflow` implementation wrapping `IExecutionLogRepository`
- `IHistoryDialogWorkflow` interface (web-layer contract for HistoryDialog)
- `HistoryDialogWorkflow` implementation wrapping `IExecutionLogService`
- `LogsDialog.razor.cs` refactored to inject only `ILogsDialogWorkflow`
- `HistoryDialog.razor.cs` refactored to inject only `IHistoryDialogWorkflow`
- DI registrations for all three workflows

## Files Modified
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/ILogsDialogWorkflow.cs` (new)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/LogsDialogWorkflow.cs` (new)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/IHistoryDialogWorkflow.cs` (new)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Schedules/HistoryDialogWorkflow.cs` (new)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/LogsDialog.razor.cs` (modified)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/HistoryDialog.razor.cs` (modified)
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` (modified)

## Key Decisions
- D-05: `ILogsDialogWorkflow` wraps `IExecutionLogRepository` (not `IExecutionLogService`)
- D-10: `IHistoryDialogWorkflow` wraps `IExecutionLogService` for history paging
- `ExecutionLog` is in `Ray.BiliBiliTool.Domain` namespace (not BlazingQuartz)
- Timer callback and `ClearDisplay()` kept in `LogsDialog.razor.cs` (D-06, D-08)

## Build Status
✅ 0 errors, 1 pre-existing warning (CS0649 on BlazingJob)
