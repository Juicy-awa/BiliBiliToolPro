# Phase 14-01 Summary: Admin Page Workflow Seam

## What Was Built

Introduced `IAdminPageWorkflow` as the web-layer workflow seam for the Admin page, replacing
direct injection of infrastructure/domain services into the component.

## Artifacts Created / Modified

- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/IAdminPageWorkflow.cs` — interface contract
- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/AdminPasswordChangeRequest.cs` — request DTO
- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/AdminPasswordChangeResult.cs` — result DTO
- `src/Ray.BiliBiliTool.Web/Services/Pages/Admin/AdminPageWorkflow.cs` — concrete implementation
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` — DI registration added
- `src/Ray.BiliBiliTool.Web/Components/Pages/Admin.razor.cs` — success flow fixed (D-01)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Admin.razor` — success alert + conditional Logout button

## Key Decisions Implemented

- **D-01**: On success, show success MudAlert + Logout button (no dialog, no auto-navigate)
- **D-02**: Workflow returns result immediately — no timing/delay logic
- **D-03**: Password fields are NOT cleared on success
- **D-04**: `Admin.razor.cs` injects only `IAdminPageWorkflow` + `IAuthService` (username query)

## Success Flow (per D-01)

After `ChangePasswordAsync` returns `IsSuccess = true`:
- `_successMessage` set from result
- `_showLogoutButton = true`
- Markup swaps Submit button → Logout button
- User clicks Logout → `NavigationManager.NavigateTo("/auth/logout", true)`
