---
phase: 18-account-crud-operations
plan: 02
subsystem: web-bili-account-reorder
tags: [blazor, sqlite, mudblazor, reorder, configuration]
key_files:
  modified:
    - src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/IBiliAccountPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/BiliAccount.razor
metrics:
  tasks_completed: 1
  commits: 1
  files_changed: 3
---

## Commits

| # | Hash | Description |
|---|------|-------------|
| 1 | 007a8de | feat(18-02): add account reorder with atomic key swap and up/down arrow buttons |

## What Was Built

Added account reordering (up/down) to the Bili Account page:

1. **IBiliAccountPageWorkflow** — Added `Task ReorderAsync(int fromIndex, int toIndex)` to the interface.

2. **BiliAccountPageWorkflow.ReorderAsync** — Validates indices, reads current cookie list, swaps the two `BiliBiliCookies__N` keys atomically via `BatchSet`, then calls `ReloadConfiguration()`.

3. **BiliAccount.razor** — Added up/down `MudIconButton` controls to each MudTable row:
   - Up button disabled on first row (`context.Index == 0`)
   - Down button disabled on last row (`context.Index == _accounts.Count - 1`)
   - `MoveUp`/`MoveDown` methods call `Workflow.ReorderAsync` then refresh the list

## Deviations

None.

## Self-Check

- [x] IBiliAccountPageWorkflow.cs contains `Task ReorderAsync(int fromIndex, int toIndex)`
- [x] BiliAccountPageWorkflow.cs contains `provider.BatchSet(swapDict)` in ReorderAsync
- [x] BiliAccountPageWorkflow.cs contains `ReloadConfiguration()` in ReorderAsync
- [x] BiliAccountPageWorkflow.cs contains argument validation for fromIndex/toIndex
- [x] BiliAccount.razor contains `Icons.Material.Filled.KeyboardArrowUp`
- [x] BiliAccount.razor contains `Icons.Material.Filled.KeyboardArrowDown`
- [x] BiliAccount.razor contains `MoveUp` and `MoveDown`
- [x] BiliAccount.razor contains `Disabled="@(context.Index == 0)"`
- [x] BiliAccount.razor contains `Workflow.ReorderAsync`
- [x] Full solution build: 0 errors
