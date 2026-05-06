---
phase: 18-account-crud-operations
plan: 01
subsystem: web-bili-account-crud
tags: [blazor, sqlite, mudblazor, crud, configuration]
key_files:
  created:
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/AddAccountDialog.razor
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/EditAccountDialog.razor
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/DeleteAccountDialog.razor
  modified:
    - src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/IBiliAccountPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/BiliAccount.razor
metrics:
  tasks_completed: 2
  commits: 2
  files_changed: 6
---

## Commits

| # | Hash | Description |
|---|------|-------------|
| 1 | 750c292 | feat(18-01): extend BiliAccountPageWorkflow with Add/Update/Delete CRUD operations using SqliteConfigurationProvider |
| 2 | 6784f5e | feat(18-01): add CRUD dialogs (Add/Edit/Delete) and action buttons on BiliAccount page |

## What Was Built

Extended the Bili Account page with full CRUD operations:

1. **IBiliAccountPageWorkflow** — Added `AddAsync(string cookieStr)`, `UpdateAsync(int index, string cookieStr)`, `DeleteAsync(int index)` to the interface.

2. **BiliAccountPageWorkflow** — Changed constructor from `IConfiguration` to `IConfigurationRoot` to enable `Reload()`. Implemented all three CRUD methods:
   - `AddAsync`: Reads current BiliBiliCookies count, writes next index to SQLite
   - `UpdateAsync`: Sets BiliBiliCookies__{index} with new cookie string (INSERT OR REPLACE)
   - `DeleteAsync`: Re-keys all higher indices down by 1 via BatchSet, then deletes the old last key
   - Private helpers: `GetSqliteProvider()` finds the provider from configurationRoot.Providers; `ReloadConfiguration()` calls `configurationRoot.Reload()`

3. **MudDialog components** — Three new dialogs in `Components/Pages/BiliAccount/`:
   - `AddAccountDialog.razor` — textarea for pasting cookie string
   - `EditAccountDialog.razor` — textarea pre-filled with current cookie
   - `DeleteAccountDialog.razor` — confirmation dialog with user ID display

4. **BiliAccount.razor** — Added "Add Account" button, Edit/Delete icon buttons per table row, injected `IDialogService`, wired all dialog callbacks to workflow methods.

## Deviations

- Moved `BiliAccount.razor` from `Components/Pages/` into `Components/Pages/BiliAccount/` subfolder to resolve namespace conflict (both the page and the subfolder generated a `BiliAccount` class in the same namespace).

## Self-Check

- [x] IBiliAccountPageWorkflow.cs contains AddAsync, UpdateAsync, DeleteAsync
- [x] BiliAccountPageWorkflow.cs constructor uses IConfigurationRoot
- [x] GetSqliteProvider() helper exists
- [x] ReloadConfiguration() calls configurationRoot.Reload()
- [x] DeleteAsync re-keys higher indices
- [x] AddAccountDialog.razor exists
- [x] EditAccountDialog.razor exists
- [x] DeleteAccountDialog.razor exists
- [x] BiliAccount.razor has Add button, Edit/Delete per row
- [x] Build passes with 0 errors
