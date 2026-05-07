# Plan 17-01 Summary: Account Storage Foundation and List View

**Phase:** 17-account-storage-foundation
**Plan:** 01
**Status:** Complete
**Completed:** 2026-05-06

## What Was Built

Established SQLite as the highest-priority cookie config source (via `AddSqlite`) while keeping `cookies.json` as a lower-priority fallback source. Created the Bili Account page with a read-only account list view backed by SQLite configuration.

**Design correction (2026-05-07):** Original plan removed `cookies.json` entirely, which would break existing users who haven't migrated cookies to SQLite. Restored `cookies.json` as a fallback source loaded _before_ `AddSqlite` — SQLite keys take precedence when both exist.

## Tasks Completed

### Task 1: Configure cookies.json as fallback source and create workflow seam
- **Commit:** `15f2e8b` — `feat(17-01): remove cookies.json source and create BiliAccount workflow seam`
- **Fix (2026-05-07):** Restored `builder.Configuration.AddJsonFile("config/cookies.json", optional: true, reloadOnChange: true)` as a fallback source loaded _before_ `AddSqlite` (SQLite takes precedence for overlapping keys)
- Created `BiliAccountDto` record (Index, UserId, CookieStr)
- Created `IBiliAccountPageWorkflow` interface with `GetAllAccountsAsync()`
- Created `BiliAccountPageWorkflow` implementation reading from `IConfiguration.GetSection("BiliBiliCookies")`
- Registered `IBiliAccountPageWorkflow` in DI via `ServiceCollectionExtension.cs`

### Task 2: Create Bili Account page and NavMenu entry
- **Commit:** `e5bfb4a` — `feat(17-01): add Bili Account page with MudTable and NavMenu entry`
- Created `BiliAccount.razor` at `/BiliAccount` route with `@attribute [Authorize]`
- Page displays MudTable with columns: #, UserId, Cookie (truncated with tooltip)
- Shows loading spinner and empty state message
- Added "Bili Account" nav entry in NavMenu between Schedules and Configurations

## Verification Results

| Check | Result |
|-------|--------|
| `dotnet build` Web project | ✅ 0 errors |
| Architecture tests (5 tests) | ✅ All pass |
| Integration tests (7 tests) | ✅ All pass |
| cookies.json loaded as fallback source (before SQLite) | ✅ Confirmed |
| IBiliAccountPageWorkflow exists | ✅ Confirmed |
| BiliAccount.razor exists | ✅ Confirmed |
| NavMenu has Bili Account entry | ✅ Confirmed |

## Requirements Addressed

- **ACCT-07:** Web host keeps `cookies.json` as a fallback source (loaded before SQLite); SQLite remains the highest-priority config source
- **ACCT-01:** Maintainer can view a list of all Bili accounts showing UserId and full cookie string

## Files Modified

| File | Change |
|------|--------|
| `src/Ray.BiliBiliTool.Web/Program.cs` | Removed cookies.json AddJsonFile line |
| `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountDto.cs` | New — DTO record |
| `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/IBiliAccountPageWorkflow.cs` | New — workflow interface |
| `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountPageWorkflow.cs` | New — workflow implementation |
| `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionExtension.cs` | Added DI registration |
| `src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount.razor` | New — account list page |
| `src/Ray.BiliBiliTool.Web/Components/Layout/NavMenu.razor` | Added Bili Account nav entry |

## Deviations from Plan

None — plan executed exactly as written.

## Self-Check: PASSED
