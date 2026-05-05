# Phase 13: Web Boundary Foundation — Discussion Log

**Date:** 2026-05-05
**Status:** Complete

## Areas Discussed

### 1. Seam pattern for Login

**Question asked:** Factory returning immutable value object vs. scoped service called in OnInitialized?

**Options presented:**
1. Factory → value object: `ILoginPageStateFactory.Create(Uri) → LoginPageState`
2. Scoped service with properties set on `Initialize(Uri)`
3. Agent's discretion

**User selection:** Option 1 — Factory returning immutable value object

**Notes:** Easy to stub in bUnit. Component calls once in `OnInitialized`, reads immutable record.

---

### 2. Current layering and call chain analysis (user-initiated)

**User question:** Is the current layering reasonable? What is the recommended layering?

**Discussion summary:**
- Analyzed `LogsDialog.razor.cs` and `HistoryDialog.razor.cs` — both directly inject `IExecutionLogRepository`, own `Timer`, `CancellationTokenSource`, and paging state
- Analyzed `Admin.razor.cs` — owns validation, `Task.Delay(2000)`, and `NavigationManager.NavigateTo` inside `ChangePasswordAsync`
- Established three-tier mental model: **code-behind = UI state only; Web-layer page service = page orchestration; Application/Domain = business logic**
- Rule locked as D-03: code-behind may only inject Web-layer page services, not Application or Domain layer services directly

---

### 3. IAdminPageWorkflow contract shape

**Question asked:** Thin placeholder vs. full contract with result type in Phase 13?

**Options presented:**
1. Full contract: `Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest)` with value types defined now
2. Thin placeholder stub; Phase 14 fills in signature
3. Agent's discretion

**User selection:** Option 1 — Full contract with result type

**Notes:** Phase 14 inherits a complete interface. Avoids designing contract under pressure during Phase 14 execution.

---

### 4. Baseline component test depth

**Question asked:** Render + state tests only, or render + interaction tests?

**Options presented:**
1. Render + state tests (query-string → field state, username load)
2. Render + interaction tests (also simulate button clicks and service call verification)
3. Agent's discretion

**User selection:** Option 3 — Agent's discretion

**Notes:** Planner decides based on what protects the refactor most cheaply. Interaction tests preferred if setup cost is low, since `ChangePasswordAsync` is the first thing Phase 14 changes.

---

## Decisions Summary

| ID | Decision |
|----|----------|
| D-01 | Login seam = `ILoginPageStateFactory.Create(Uri) → LoginPageState` immutable record |
| D-02 | `IAdminPageWorkflow` defines full contract + value types in Phase 13; Phase 14 implements |
| D-03 | Code-behind may only inject Web-layer page services; no direct Application/Domain injection |
| — | Baseline test depth → agent's discretion |

## Deferred Ideas

- Full Admin.razor.cs migration to IAdminPageWorkflow → Phase 14
- Scheduler seams → Phase 15
- CI coverage thresholds → future milestone
