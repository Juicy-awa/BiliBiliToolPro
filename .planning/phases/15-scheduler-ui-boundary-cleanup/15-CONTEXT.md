# Phase 15: Scheduler UI Boundary Cleanup - Context

**Gathered:** 2026-05-05
**Status:** Ready for planning

<domain>
## Phase Boundary

Refactor `Schedules.razor.cs`, `LogsDialog.razor.cs`, and `HistoryDialog.razor.cs` so that repository-backed log polling, execution-history loading, and scheduler-event coordination are moved out of the component code-behind classes. Components may only inject Web-layer workflow services — not Domain repositories or infrastructure services that own coordination logic.

WEB-02 requirement scope: only the three components listed above. `*TaskConfig` components are out of scope for this phase.
</domain>

<canonical_refs>
## Canonical References

- `.planning/REQUIREMENTS.md` — WEB-02 definition
- `.planning/ROADMAP.md` — Phase 15 goal and success criteria
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/Schedules.razor.cs` — main target
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/LogsDialog.razor.cs` — Domain boundary violation (`IExecutionLogRepository` injected directly)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/HistoryDialog.razor.cs` — history loading target
- `src/Ray.BiliBiliTool.Domain/IExecutionLogRepository.cs` — Domain interface currently injected by Web component (violation)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/IAdminPageWorkflow.cs` — established pattern to follow
- `test/Ray.BiliBiliTool.Web.ComponentTests/AdminPageWorkflowTests.cs` — established test pattern to follow
</canonical_refs>

<decisions>
## Decisions

### D-01: Schedules page — extraction scope (log loading only)
Only `UpdateScheduleModelsLastExecution` (the `IExecutionLogService` call that populates `PreviousTriggerTime` and `ExceptionMessage`) is extracted to `ISchedulerPageWorkflow`. The 8 Quartz event listener registrations/handlers (`RegisterEventListeners`, `UnRegisterEventListeners`, and all `SchedulerListenerSvc_On*` methods) remain in the component — they manage UI state (`ObservableCollection<ScheduleModel>`) and do not constitute repository-backed coordination.

### D-02: Schedules page — scheduler actions delegated to workflow
Scheduler action methods (`OnResumeScheduleJob`, `OnPauseScheduleJob`, and any trigger/run operations) are also moved into `ISchedulerPageWorkflow`. The component does not directly inject or call `ISchedulerService`.

### D-03: `ISchedulerPageWorkflow` method signatures — Agent's discretion
The planner chooses the method signatures and return types for `ISchedulerPageWorkflow`. Must cover: (a) enriching schedule models with last-execution data, (b) resume trigger, (c) pause trigger. Follow the `AdminPasswordChangeResult` pattern where applicable.

### D-04: Error handling in `ISchedulerPageWorkflow` — Agent's discretion
The planner decides how errors from scheduler operations surface to the component (`ISnackbar` stays in the component; workflow returns a result object or throws — Agent chooses).

### D-05: LogsDialog — introduce `ILogsDialogWorkflow`
`LogsDialog.razor.cs` must stop injecting `IExecutionLogRepository` (Domain interface). A new Web-layer interface `ILogsDialogWorkflow` is introduced to encapsulate all repository and log-service calls. The component injects only `ILogsDialogWorkflow`.

### D-06: LogsDialog — Timer polling placement — Agent's discretion
The 3-second `Timer` that drives log refresh may stay in the component or be moved into the workflow. The planner decides based on what produces cleaner testable code.

### D-07: `ILogsDialogWorkflow` method signatures — Agent's discretion
The planner decides whether to expose one combined method or two separate ones (get instance ID + get logs). Both `IExecutionLogRepository.GetLatestRunInstanceIdAsync` and `GetLogsForRunAsync` must be behind the workflow boundary.

### D-08: LogsDialog — `ClearDisplay()` stays in component
`ClearDisplay()` clears `_logs` (UI state only, no repository call) and remains in the component. No workflow involvement.

### D-09: Test coverage — full two-layer
Three workflow unit-test classes + bUnit component tests, mirroring the Phase 14 Admin pattern:
- `SchedulerPageWorkflowTests` (xUnit unit tests for `ISchedulerPageWorkflow` implementation)
- `LogsDialogWorkflowTests` (xUnit unit tests for `ILogsDialogWorkflow` implementation)
- `HistoryDialogWorkflowTests` (xUnit unit tests for `IHistoryDialogWorkflow` implementation)
- bUnit component tests for `Schedules`, `LogsDialog`, `HistoryDialog` using `FakeXxx` stubs

### D-10: HistoryDialog — introduce `IHistoryDialogWorkflow`
Even though `HistoryDialog.razor.cs` only injects `IExecutionLogService` (no Domain boundary violation), a `IHistoryDialogWorkflow` is introduced for consistency with the established pattern and to enable stub-based bUnit testing.

### D-11: bUnit component test depth — Agent's discretion
The planner decides how deeply to test Schedules event-listener behavior in bUnit (shallow: only rendering/action-button states; or deeper: event-trigger → UI state transitions).

### D-12: Test file location — `Schedules/` sub-directory
All Phase 15 test files go into `test/Ray.BiliBiliTool.Web.ComponentTests/Schedules/` sub-directory. Same project as Phase 14 tests, organized by feature folder.

</decisions>

<agent_discretion>
## Agent's Discretion

- `ISchedulerPageWorkflow` method signatures and return types (D-03)
- Error surfacing pattern for scheduler operations (D-04)
- Timer polling placement in `LogsDialog` (D-06)
- `ILogsDialogWorkflow` method granularity (one vs two methods) (D-07)
- bUnit test depth for Schedules event-listener behavior (D-11)
</agent_discretion>

<deferred_ideas>
## Deferred Ideas

- Refactoring `*TaskConfig` components (ChargeTaskConfig, LiveFansMedalTaskConfig, etc.) — not in WEB-02 scope
- Replacing Quartz event-listener pattern with a reactive/SignalR push model
- Full test coverage of all Quartz event handler transitions
</deferred_ideas>
