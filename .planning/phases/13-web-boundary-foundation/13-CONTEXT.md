# Phase 13: Web Boundary Foundation - Context

**Gathered:** 2026-05-05
**Status:** Ready for planning

<domain>
## Phase Boundary

Introduce the first explicit Web-layer coordination seams for the auth/admin slice, add the minimal bUnit/xUnit component-test harness, and establish the pattern that Phases 14 and 15 will follow.

This phase delivers two things: (1) a working Login-page seam (factory + concrete implementation, DI-registered, component using it), and (2) the full `IAdminPageWorkflow` contract with value types ready for Phase 14 to implement. It also adds the component-test project and baseline tests before any refactoring begins.

The phase does NOT move orchestration out of `Admin.razor.cs` — that is Phase 14's job. It only defines the seam Phase 14 will implement.

</domain>

<decisions>
## Implementation Decisions

### Login Seam Shape
- **D-01:** The Login page seam must be a **factory returning an immutable value object**: `ILoginPageStateFactory.Create(Uri) → LoginPageState`. `LoginPageState` is an immutable record with `ReturnUrl` (nullable string) and `HasLoginError` (bool). The component calls it once in `OnInitialized` and reads the returned record. This shape makes bUnit stubbing trivial — mock returns a pre-built `LoginPageState`.

### Admin Page Workflow Contract
- **D-02:** `IAdminPageWorkflow` must define a **full contract** in Phase 13, not a placeholder stub. The interface must expose `Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)`. Both `ChangePasswordRequest` (username, currentPassword, newPassword, confirmPassword) and `ChangePasswordResult` (IsSuccess, ErrorMessage, SuccessMessage) must be defined as value types (records or readonly structs) in Phase 13. Phase 14 provides the implementation; Phase 13 only defines the contract and value types.

### Layering Rule (carries forward to all remaining phases)
- **D-03:** Code-behind (`.razor.cs`) may only inject **Web-layer page services** (e.g., `ILoginPageStateFactory`, `IAdminPageWorkflow`). It must NOT inject Application or Domain layer services directly (e.g., `IAuthService`, `IExecutionLogRepository`). Orchestration logic, validation, timing, and navigation belong in the Web-layer service, not the component. This rule applies to all refactored pages in v4.0.0.6.

### Baseline Component Test Depth
- **Agent's Discretion:** Planner decides the exact test depth (render-only vs. render + interaction). The intent is to pin the current observable behavior before refactoring begins. Tests should be cheap to maintain and targeted enough to catch a refactor that breaks initial state derivation or the auth call path. Interaction tests for the Admin `ChangePasswordAsync` path are preferred if they do not add significant setup complexity, since that is the first thing Phase 14 will change.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Phase Scope and Requirements
- `.planning/PROJECT.md` — milestone goal and Web layer boundary cleanup intent
- `.planning/REQUIREMENTS.md` — WEB-01 (page seams) and WEB-05 (component test harness) are the active requirements for this phase
- `.planning/ROADMAP.md` — Phase 13 goal, plan list, and success criteria
- `.planning/STATE.md` — current project state and continuity notes

### Milestone Research
- `.planning/research/v4.0.0.6-web-layer-boundary-cleanup-RESEARCH.md` — full analysis of current Web layer boundary violations, bUnit recommendations, and recommended direction

### Existing Web-layer Code (read before writing new seams)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Login.razor.cs` — current code-behind; `ILoginPageStateFactory` replaces `OnInitialized` query-string parsing
- `src/Ray.BiliBiliTool.Web/Components/Pages/Admin.razor.cs` — current code-behind; `IAdminPageWorkflow` contract must match the current `ChangePasswordAsync` behavior
- `src/Ray.BiliBiliTool.Web/Services/AuthService.cs` — existing `IAuthService` seam; the new page services sit above this, not beside it
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` — DI registration entry point; new services must be registered here

### Existing Plans (must be reconsidered / updated after context capture)
- `.planning/phases/13-web-boundary-foundation/13-01-PLAN.md` — bUnit harness plan; review against D-01 and Discretion decision on test depth
- `.planning/phases/13-web-boundary-foundation/13-02-PLAN.md` — seam + contract plan; review against D-01 and D-02

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `IAuthService` / `AuthService.cs` — already an established Web-layer abstraction pattern; `ILoginPageStateFactory` and `IAdminPageWorkflow` follow this same pattern and sit above it in the call chain
- `ServiceCollectionExtension.cs` — existing DI registration file; new Web-layer services register here as `AddScoped<Interface, Implementation>()`
- `MudBlazor` components (MudTextField, InputType) — already used in Login and Admin markup; code-behind can stay MudBlazor-agnostic in the new seam

### Established Patterns
- Factory-returns-value-object pattern (D-01): trivially stub in bUnit by configuring the mock to return a fixed `LoginPageState` record
- Result-type pattern (D-02): `ChangePasswordResult` mirrors the discriminated-result pattern used in other application-layer calls; keeps component code-behind free of try/catch blocks
- DI scoped registration: all new Web-layer page services register as `Scoped` (one per request/circuit)

### Integration Points
- `Login.razor.cs` OnInitialized → injects `ILoginPageStateFactory`, calls `.Create(NavigationManager.ToAbsoluteUri(...))`, reads result fields
- `ServiceCollectionExtension.cs` → registers `LoginPageStateFactory` and (stub or no-op) `IAdminPageWorkflow` implementation
- `test/Ray.BiliBiliTool.Web.ComponentTests/*.csproj` → references `Ray.BiliBiliTool.Web.csproj`; uses bUnit + xUnit; mocks `ILoginPageStateFactory` and `IAuthService`

</code_context>

<specifics>
## Specific Ideas

- The layering discussion produced a clear mental model: **code-behind = UI state only; Web-layer service = page orchestration; Application/Domain = business logic**. The analogy is: code-behind is the view-model; the Web-layer service is the presenter.
- For `Admin.razor.cs`: even though Phase 13 does not migrate the orchestration, the component should remain unchanged — Phase 14 does the migration using the contract defined here.
- `ChangePasswordResult` should include enough information for the component to decide what to show (error string vs. success string) without having to catch exceptions.

</specifics>

<deferred>
## Deferred Ideas

- Full migration of `Admin.razor.cs` to use `IAdminPageWorkflow` — Phase 14
- Scheduler page and dialog seams (`ISchedulerLogPresenter`, etc.) — Phase 15
- Broader coverage thresholds or CI enforcement — deferred to future milestone (TEST-05)
- Notification adapter/port boundary — deferred (ARCH-04, pre-existing)

</deferred>
