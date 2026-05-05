---
phase: 13-web-boundary-foundation
plan: 02
subsystem: web-seam
tags: [web, seam, login, admin, di]
key-files:
  created:
    - src/Ray.BiliBiliTool.Web/Services/Pages/Login/LoginPageState.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/Login/ILoginPageStateFactory.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/Login/LoginPageStateFactory.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/Admin/IAdminPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/Admin/AdminPasswordChangeResult.cs
  modified:
    - src/Ray.BiliBiliTool.Web/Components/Pages/Login.razor.cs
    - src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs
metrics:
  files_created: 5
  files_modified: 2
  tests_added: 0
---

# Phase 13-02 Summary — Login Page-State Seam and Auth/Admin Workflow Contract

## What Was Built

Introduced the first explicit Web-layer coordination seams for Phase 13:

1. **`ILoginPageStateFactory` / `LoginPageState`** — An immutable record returned by a Web-owned factory that encapsulates query-string parsing (`returnUrl`, `error`). Login component code-behind no longer reads `NavigationManager.Uri` inline; it injects the factory and reads the returned state.

2. **`LoginPageStateFactory`** — Concrete implementation; centralises `QueryHelpers.ParseQuery` so there is one audited place for returnUrl / error-flag derivation.

3. **`IAdminPageWorkflow` / `AdminPasswordChangeRequest` / `AdminPasswordChangeResult`** — Full contract (per D-02) for the Admin page change-password workflow. Three result branches modelled: validation failure, service failure, success. Phase 14 provides the implementation.

4. **DI registration** — `ILoginPageStateFactory` registered as Scoped in `ServiceCollectionExtension.AddWebServices()`. `IAdminPageWorkflow` is not registered (implementation deferred to Phase 14).

## Commits

| Task | Commit | Description |
|------|--------|-------------|
| Task 1 + 2 | 139257c | feat(13-02): extract Login page-state seam and define admin workflow contract |

## Deviations

None. Implementation matched all decisions from CONTEXT.md (D-01, D-02, D-03).

## Self-Check: PASSED

- `dotnet build src\Ray.BiliBiliTool.Web\Ray.BiliBiliTool.Web.csproj --no-restore -v minimal` → 0 errors
- `ILoginPageStateFactory` is defined and registered in DI
- `Login.razor.cs` no longer imports `Microsoft.AspNetCore.WebUtilities`; it injects `ILoginPageStateFactory` (per D-03)
- `IAdminPageWorkflow`, `AdminPasswordChangeRequest`, `AdminPasswordChangeResult` are defined
- D-01: Factory returns immutable record ✓
- D-02: Full workflow contract with request/result value types ✓
- D-03: Component code-behind injects only Web-layer page service ✓
