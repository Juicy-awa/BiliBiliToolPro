# Phase 07-01 Summary: Upgrade BaseMultiAccountsAppService

## What Was Built

Upgraded `BaseMultiAccountsAppService` to accept and manage cookie handling centrally.

### Key Changes

**`src/Ray.BiliBiliTool.Application/BaseMultiAccountsAppService.cs`**
- Expanded constructor from 2 → 4 params: added `ILoginDomainService loginDomainService`, `IConfiguration configuration`
- Added `protected virtual async Task SetCookiesAsync(BiliCookie biliCookie, CancellationToken)` — checks Buvid, calls `loginDomainService.SetCookieAsync`, then `SaveCookieAsync`
- Added `protected virtual async Task SaveCookieAsync(BiliCookie ckInfo, CancellationToken)` — reads `PlatformType`, routes to `SaveCookieToQinLongAsync` or `SaveCookieToJsonFileAsync`
- Added usings: `Microsoft.Extensions.Configuration`, `Ray.BiliBiliTool.DomainService.Interfaces`, `Ray.BiliBiliTool.Infrastructure.Enums`

## Verification

- `dotnet build` Application project: 0 errors
- All derived services still compile (pre-migration errors were expected; fixed in Plans 02+03)

## Artifacts

- `.planning/phases/07-appservice-cookie-handling-extraction/07-01-PLAN.md`
- `src/Ray.BiliBiliTool.Application/BaseMultiAccountsAppService.cs` (modified)
