# Research Summary: Bili Account Management (v4.0.0.7)

## Stack

**Existing (no additions needed):**
- .NET 8, Blazor Server (`@rendermode InteractiveServer`), MudBlazor 8.6.0 UI
- EF Core 8.0.18 + SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)
- `SqliteConfigurationProvider` — custom key-value config store in `bili_appsettings` table
- `QRCoder 1.6.0` — already in `Directory.Packages.props`, used by `LoginDomainService`
- Refit 8.0.0 — `IPassportApi` with `GenerateQrCode()` and `CheckQrCodeHasScaned()`
- `CookieStrFactory<BiliCookie>` — reads `BiliBiliCookies__0`, `BiliBiliCookies__1` etc. from IConfiguration

**No new packages required.** All needed libraries are already referenced.

## Features

### Cookie Storage Model
- Cookies are config keys: `BiliBiliCookies__0`, `BiliBiliCookies__1`, etc.
- `SqliteConfigurationProvider.Set(key, value)` writes to `bili_appsettings` table
- `SqliteConfigurationProvider.BatchSet(dict)` writes multiple keys atomically
- `BaseMultiAccountsAppService` iterates `cookieStrFactory.Count` accounts
- `CookieStrFactory<BiliCookie>` reads from `IConfiguration` at runtime

### QR Code Login Flow
- `IPassportApi.GenerateQrCode()` → returns `QrCodeDto` with `Url` and `Qrcode_key`
- `IPassportApi.CheckQrCodeHasScaned(qrcode_key)` → returns HttpResponseMessage with Set-Cookie headers
- `LoginDomainService.LoginByQrCodeAsync()` orchestrates: generate → display QR → poll 10 times (5s intervals) → extract cookie
- QRCoder library generates QR bitmap from URL — currently writes to console; needs adaptation for browser display (base64 image)

### Web Page Patterns
- Pages: `@page "/path"`, `@attribute [Authorize]`, `@rendermode InteractiveServer`
- Workflow seams: `ILoginPageStateFactory`, `IAdminPageWorkflow`, `ISchedulerPageWorkflow` (established in v4.0.0.6)
- NavMenu: collapsible submenu with `_showConfigSubMenu` toggle pattern
- Config pages: `BaseConfigComponent<T>` loads from `IOptionsMonitor<T>`, saves via `SqliteConfigurationProvider`

## Architecture

### Integration Points
1. **New workflow seam**: `IBiliAccountPageWorkflow` — follows v4.0.0.6 pattern (page injects seam, seam orchestrates)
2. **Cookie storage**: Write `BiliBiliCookies__N` keys to SQLite via `SqliteConfigurationProvider.Set()`
3. **QR login**: Wrap `IPassportApi` + `LoginDomainService.LoginByQrCodeAsync()` behind the workflow seam
4. **Config reload**: After writing to SQLite, call `SqliteConfigurationProvider.Load()` to refresh in-memory config so `CookieStrFactory` picks up changes
5. **NavMenu**: Add "Bili Account" entry (top-level, not under Configurations submenu)

### Data Flow
```
Web Page (Blazor) → IBiliAccountPageWorkflow → SqliteConfigurationProvider.Set("BiliBiliCookies__N", cookieStr)
                                              → IConfiguration reload
                                              → CookieStrFactory reads updated config
```

### Key Constraint
- `CookieStrFactory<BiliCookie>` is registered as singleton and reads cookies at construction time
- After modifying SQLite config, the configuration root must be reloaded for changes to take effect in task execution
- `SqliteConfigurationProvider.Load()` refreshes the provider's `Data` dictionary, but `IConfigurationRoot.Reload()` is needed to propagate

## Pitfalls

1. **Config reload timing**: Writing to SQLite doesn't automatically update `IConfiguration` in-memory. Must call `IConfigurationRoot.Reload()` after writes, or task execution will use stale cookies.
2. **QR code in browser**: `LoginDomainService.LoginByQrCodeAsync()` uses `QRCoder` to generate a bitmap and logs it to console. For Web, need to generate a base64 PNG data URI and display in an `<img>` tag. The polling loop must run in Blazor's `InteractiveServer` render mode (SignalR connection stays alive).
3. **Cookie key numbering**: When deleting an account at index 2 of 4, must re-key remaining accounts (3→2) to maintain contiguous `BiliBiliCookies__0..N` numbering, since `CookieStrFactory` iterates by index.
4. **Reorder complexity**: Drag-drop reorder in MudBlazor requires `MudDropContainer`/`MudDropZone`. Simpler alternative: up/down arrow buttons that swap keys.
5. **Concurrent access**: If a scheduled task is running while cookies are being modified, the `CookieStrFactory` may have already loaded the old config. This is acceptable for now — changes take effect on next task run.
6. **Existing `cookies.json` removal for Web**: The Web host loads `config/cookies.json` in `Program.cs`. Must remove that `AddJsonFile` line. The existing `AddSqlite` provider remains and becomes the sole config source for cookies in Web. `IConfiguration` reading logic (`CookieStrFactory`, task execution) stays unchanged — only the config source pipeline changes.

## Table Stakes For This Refactor

- Define milestone scorecards around change safety: lead time, regression rate, hotspot reduction, and time to validate critical flows.
- Map the current runtime paths for login, scheduled task execution, outbound API calls, and persistence before moving code.
- Add characterization tests around the most failure-prone orchestration flows before extraction.
- Enforce dependency rules early so hosts depend on module contracts instead of infrastructure internals.
- Treat adapters and temporary facades as planned assets with rollback and removal criteria.
- Add observability on refactor paths before switching callers or schedules to new code.
- Keep routes, config keys, scheduler identities, and external DTO contracts stable unless a change is intentional and covered.

## High-Leverage Early Wins

- Introduce one module registration entry point per slice so Program startup stops wiring business details directly.
- Create an application facade for DailyTask or Login first; both exercise the most cross-cutting behavior.
- Move Agent and EF usage behind module-owned ports without replacing the underlying implementations yet.
- Add architecture tests to block Web, Quartz jobs, and hosts from reaching into infrastructure internals.
- Add a thin integration suite with real host bootstrapping and realistic configuration binding for critical flows.
- Add HTTP resilience selectively to the outbound clients that have known retry, timeout, or rate-limit risk.
- Use hotspot analysis to rank the next slices instead of following current folder or team ownership.

## Major Risks To Control

- Superficial project or namespace reorganization before behavior seams exist will create noise without reducing coupling.
- Interface-heavy wrappers around existing orchestration will preserve the same complexity with more indirection.
- Module boundaries will remain fiction unless cross-module calls, shared DTO leakage, and direct data access are blocked.
- A unit-test-first strategy will miss the real regression risk in jobs, startup flow, EF mappings, configuration, and HTTP integration.
- Transitional code can become permanent unless each adapter, toggle, or dual path has an exit condition.
- Host startup and DI registration can become the new dumping ground if business decisions are not moved into application use cases.
- Boundary cleanup will stall if data ownership stays global and modules still write across each other's persistence concerns.

## Suggested Planning Principles

- Plan in thin vertical slices that preserve behavior and can ship independently.
- Make hosts thinner before making the domain purer.
- Add seams only where they improve testing, observation, or traffic redirection.
- Prefer logical modularization inside the current solution before splitting more assemblies.
- Pair every structural change with a focused validation path: characterization, integration, or architecture test.
- Sequence work as map critical flow -> freeze behavior -> add seam -> redirect caller -> enforce boundary -> retire legacy path.
- Favor branch-by-abstraction, side-by-side verification, and rollback-ready cutovers over one-step replacements.
- Use temporary architecture deliberately, but record owners and teardown triggers at the time it is introduced.
- Keep refactor work attached to user-visible or operationally meaningful slices so the program remains fundable.

## Planning Implication

The first roadmap phases should establish scorecards, flow maps, characterization coverage, module registration seams, and dependency enforcement, then refactor one high-churn vertical slice end to end. DailyTask and Login are the strongest starting candidates because they provide the best signal on architecture boundaries, code quality, and testability with relatively low-risk incremental delivery.
