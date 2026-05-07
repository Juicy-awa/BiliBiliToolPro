# Phase 18: Account CRUD Operations - Context

**Gathered:** 2026-05-07
**Status:** Ready for planning

<domain>
## Phase Boundary

Enable adding, editing, deleting, and reordering Bili accounts through the Web UI. All mutations write to SQLite `bili_appsettings` via `SqliteConfigurationProvider` and reload `IConfigurationRoot` so `CookieStrFactory` picks up changes.

Phase 18 builds on Phase 17's read-only Bili Account page (`BiliAccount.razor`) and workflow seam (`IBiliAccountPageWorkflow`).

</domain>

<decisions>
## Implementation Decisions

### A: Write Path — SQLite Configuration Provider

- **D-01:** All CRUD operations write to SQLite via `SqliteConfigurationProvider.Set(key, value)` for single operations and `SqliteConfigurationProvider.BatchSet(dict)` for batch operations (re-key on delete). Cookies are stored as `BiliBiliCookies__0`, `BiliBiliCookies__1`, etc. in the `bili_appsettings` table.
- **D-02:** After all writes, call `IConfigurationRoot.Reload()` to propagate changes to in-memory config. `SqliteConfigurationProvider.Load()` alone is insufficient — the config root must also reload.
- **D-03:** The workflow seam `IBiliAccountPageWorkflow` is extended with mutation methods: `AddAsync(string cookieStr)`, `UpdateAsync(int index, string cookieStr)`, `DeleteAsync(int index)`, and `ReorderAsync(int fromIndex, int toIndex)`.

### B: Add Account (ACCT-03)

- **D-04:** Add reads the current `BiliBiliCookies` section count, determines the next available index, writes `BiliBiliCookies__N` to SQLite, then reloads configuration.
- **D-05:** UI: An "Add Account" button on the BiliAccount page opens a MudDialog with a textarea for pasting the cookie string. On confirm, calls `Workflow.AddAsync(cookieStr)`.

### C: Edit Account (ACCT-04)

- **D-06:** Edit uses `SqliteConfigurationProvider.Set("BiliBiliCookies__N", newCookieStr)` — same key, new value (INSERT OR REPLACE). The index does not change.
- **D-07:** UI: An edit icon button per row opens a MudDialog pre-filled with the current cookie string. On save, calls `Workflow.UpdateAsync(index, newCookieStr)`.

### D: Delete Account (ACCT-05)

- **D-08:** Delete removes the target key and re-keys all higher-index accounts to maintain contiguous `BiliBiliCookies__0..N` numbering. Uses `BatchSet` for the re-keyed accounts, then deletes the old (now-duplicated) last key.
- **D-09:** UI: A delete icon button per row opens a confirmation MudDialog. On confirm, calls `Workflow.DeleteAsync(index)`.

### E: Reorder Accounts (ACCT-06)

- **D-10:** Reorder uses up/down arrow buttons per row. Swapping index A and B writes both keys atomically via `BatchSet`, then reloads configuration.
- **D-11:** UI: Each row in the MudTable gets up/down `MudIconButton` controls. The first row has no "up" button; the last row has no "down" button.

### F: Config Reload Strategy

- **D-12:** After every mutation (add/edit/delete/reorder), the workflow calls `IConfigurationRoot.Reload()`. This is implemented once in a private helper method within `BiliAccountPageWorkflow`.

### Agent's Discretion

- Exact MudBlazor dialog component naming (e.g., `AddAccountDialog.razor`, `EditAccountDialog.razor`, `DeleteAccountDialog.razor`)
- Cookie string validation before save (minimum: check `DedeUserID` is parseable)
- Success/error snackbar notifications after CRUD operations
- Exact MudBlazor icon choices for action buttons

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Cookie Storage Model
- `src/Ray.BiliBiliTool.Config/SQLite/SqliteConfigurationProvider.cs` — Set() and BatchSet() implementations; the write path
- `src/Ray.BiliBiliTool.Config/SQLite/SqliteConfigurationExtensions.cs` — AddSqlite extension method
- `src/Ray.BiliBiliTool.Infrastructure/Cookie/CookieStrFactory.cs` — reads BiliBiliCookies__N keys from IConfiguration

### Workflow Seam (Phase 17 output)
- `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/IBiliAccountPageWorkflow.cs` — interface to extend
- `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountPageWorkflow.cs` — implementation to extend
- `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountDto.cs` — DTO to evolve

### Page and UI
- `src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount.razor` — page to add CRUD UI and action buttons
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` — DI registrations

### Existing Patterns
- `src/Ray.BiliBiliTool.Web/Components/Pages/Configs/BaseConfigComponent.cs` — GetSqliteConfigurationProvider() pattern
- `.planning/research/SUMMARY.md` — Technical research with cookie storage model, reload strategy, and pitfalls

</canonical_refs>

<specifics>
## Specific Ideas

- Cookie key numbering is fragile: `CookieStrFactory` reads by `BiliBiliCookies__N` index. After delete, all higher indices must be shifted down to maintain contiguity.
- Concurrent access during task execution: if a scheduled task is running while cookies are modified, `CookieStrFactory` may have already loaded old config. Changes take effect on next task run — acceptable.
- The ROADMAP splits Phase 18 into two plans: 18-01 (add/edit/delete) and 18-02 (reorder + config reload). However, config reload is needed by ALL operations, so 18-01 must include it too.

</specifics>

<deferred>
## Deferred Ideas

- Drag-and-drop reordering (too complex; up/down buttons sufficient)
- QR code login (belongs in Phase 19)
- Nickname/avatar display from Bilibili API (deferred to future milestone)
- Primary/default account selection (deferred)

</deferred>

---

*Phase: 18-account-crud-operations*
*Context gathered: 2026-05-07*
