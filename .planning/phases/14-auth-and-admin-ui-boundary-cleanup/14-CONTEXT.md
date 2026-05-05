# Phase 14: Auth And Admin UI Boundary Cleanup - Context

**Gathered:** 2026-05-05
**Status:** Ready for planning

<domain>
## Phase Boundary

Move the password-change workflow's validation, service call, and success-flow handling out of `Admin.razor.cs` and into `AdminPageWorkflow : IAdminPageWorkflow`. The Login page is already clean after Phase 13 — this phase is exclusively an Admin concern.

After this phase:
- `Admin.razor.cs` injects `IAdminPageWorkflow` only (not `IAuthService` directly)
- All business validation (password mismatch, empty password, current-password verification) lives in `AdminPageWorkflow`
- Component reads `AdminPasswordChangeResult` and handles only rendering decisions (show error message, show success message + Logout button, navigate on Logout click)
- `IAdminPageWorkflow` is registered in DI and the concrete `AdminPageWorkflow` is the only non-test implementation

</domain>

<decisions>
## Implementation Decisions

### Success Flow — Navigation and Timing
- **D-01:** On `AdminPasswordChangeResult.IsSuccess = true`, the **Component** shows the success message AND renders a **Logout button**. The user clicks the button to trigger `NavigationManager.NavigateTo("/auth/logout", true)`. There is **no `Task.Delay`** — the delay is eliminated entirely; the Logout button replaces the automatic timed redirect.
- **D-02:** The Workflow (`AdminPageWorkflow.ChangePasswordAsync`) returns `AdminPasswordChangeResult` **immediately** after success — no timing logic inside the Workflow. The Workflow is responsible for validation and calling `IAuthService.ChangePasswordAsync`; it is not responsible for navigation timing.

### Field Clearing on Success
- **D-03:** On success, the Component does **not** clear the password fields. The user's next action is to click Logout and leave the page — field state at that point is irrelevant.

### Layering Rule (carried from Phase 13 D-03)
- **D-04:** `Admin.razor.cs` may only inject **Web-layer page services**. After this phase it must inject `IAdminPageWorkflow` (and `IAuthService` for `GetAdminUserNameAsync` if needed — see Discretion). It must NOT inject `IAuthService` for the password-change call.

### Test Strategy
- **D-05:** Phase 14 produces **two layers of tests**:
  1. **`AdminPageWorkflowTests`** — xUnit unit tests for `AdminPageWorkflow` using a mock `IAuthService`. Covers: validation failure paths (empty password, mismatch), correct-password success path, incorrect-current-password failure path (service throws).
  2. **Updated `AdminPageTests`** — existing bUnit component tests updated to inject a `FakeAdminPageWorkflow` stub instead of `FakeAuthService`. Tests confirm: Component renders error from `AdminPasswordChangeResult.ErrorMessage`, Component renders success + Logout button from `AdminPasswordChangeResult.IsSuccess=true`, username still loads on init.

### Agent's Discretion
- **GetAdminUserNameAsync ownership:** `Admin.razor.cs` currently uses `IAuthService.GetAdminUserNameAsync()` in `OnInitializedAsync`. The planner may choose to keep this direct injection (it is a query, not orchestration, and adding it to `IAdminPageWorkflow` would add unnecessary surface area) OR route it through the Workflow. Either approach is valid as long as D-04 is respected.
- **`AdminPageWorkflow` namespace and file location:** Follow the pattern established in Phase 13 — `Services/Pages/Admin/AdminPageWorkflow.cs` under `Ray.BiliBiliTool.Web`.
- **Exact Logout button markup:** Planner chooses the MudBlazor component (e.g., `MudButton` with `OnClick`) that triggers navigation. Must match existing button style conventions.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Phase Scope and Requirements
- `.planning/PROJECT.md` — milestone goal and Web layer boundary cleanup intent
- `.planning/REQUIREMENTS.md` — WEB-03 is the active requirement for this phase
- `.planning/ROADMAP.md` — Phase 14 goal and success criteria
- `.planning/STATE.md` — current project state

### Milestone Research
- `.planning/research/v4.0.0.6-web-layer-boundary-cleanup-RESEARCH.md` — full analysis of current Web layer boundary violations and recommended direction

### Phase 13 Outputs (contracts defined here, Phase 14 implements)
- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/IAdminPageWorkflow.cs` — the interface Phase 14 must implement
- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/AdminPasswordChangeResult.cs` — request and result records (locked shape)
- `.planning/phases/13-web-boundary-foundation/13-02-SUMMARY.md` — Phase 13 decisions and what was shipped

### Existing Web-layer Code (read before writing)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Admin.razor.cs` — current code-behind; this is what Phase 14 refactors
- `src/Ray.BiliBiliTool.Web/Components/Pages/Admin.razor` — markup; the submit button wires to `ChangePasswordAsync`, success message display, may need Logout button added
- `src/Ray.BiliBiliTool.Web/Services/AuthService.cs` — `IAuthService.ChangePasswordAsync` and `GetAdminUserNameAsync`; `AdminPageWorkflow` wraps these
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` — DI registration; `AdminPageWorkflow` must be registered here

### Existing Tests (read before updating)
- `test/Ray.BiliBiliTool.Web.ComponentTests/AdminPageTests.cs` — Phase 13 baseline tests; Phase 14 updates these to use `FakeAdminPageWorkflow`

</canonical_refs>

<code_context>
## Existing Code Insights

### What AdminPageWorkflow must replicate from Admin.razor.cs
Current `ChangePasswordAsync` logic to move into Workflow:
1. Clear error/success messages (internal state — Workflow does not manage UI state; it returns a result)
2. Validation: if `newPassword != confirmPassword` → return error result
3. Validation: if `string.IsNullOrWhiteSpace(newPassword)` → return error result
4. Call `await AuthService.ChangePasswordAsync(username, currentPassword, newPassword)` in try/catch
5. On success: return `AdminPasswordChangeResult(IsSuccess: true, SuccessMessage: "Update Successful, you will be logged out in 2 seconds")`
6. On exception: return `AdminPasswordChangeResult(IsSuccess: false, ErrorMessage: e.Message)`

Note: Steps 2–6 map directly to the result shape already defined in `AdminPasswordChangeResult`.

### What stays in Admin.razor.cs after refactoring
- `[Inject] IAdminPageWorkflow AdminPageWorkflow` (replaces `IAuthService` for change-password)
- `OnInitializedAsync` → `_username = await AuthService.GetAdminUserNameAsync()` (may stay or move per Discretion)
- `ChangePasswordAsync()` → calls `AdminPageWorkflow.ChangePasswordAsync(new AdminPasswordChangeRequest(...))`, reads result, sets `_errorMessage` or `_successMessage`, sets `_showLogoutButton = true` on success
- Logout button click handler → `NavigationManager.NavigateTo("/auth/logout", true)`
- All password visibility toggle logic (unchanged)

### Established Patterns
- Result type pattern: `AdminPasswordChangeResult` already defined — no changes to its shape
- DI registration: `services.AddScoped<IAdminPageWorkflow, AdminPageWorkflow>()` in `ServiceCollectionExtension.AddWebServices()`
- bUnit test pattern: `FakeLoginPageStateFactory` in Phase 13 is the model for `FakeAdminPageWorkflow`

</code_context>

<deferred>
## Deferred Ideas

- Scheduler page and dialog seams — Phase 15 (WEB-02)
- Post-Redirect-Get pattern for success notification (pass `?reason=passwordChanged` through logout → login) — elegant but out of Phase 14 scope
- Broader DI registration audit or startup composition cleanup — Phase 16 (WEB-04)
- Notification adapter/port boundary — deferred (ARCH-04, pre-existing)

</deferred>
