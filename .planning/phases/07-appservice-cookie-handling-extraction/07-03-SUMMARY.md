# Phase 07-03 Summary: Migrate Group B Services

## What Was Built

Migrated 5 AppService classes that were missing `ILoginDomainService` + `IConfiguration` and therefore had no cookie refresh at all.

### Services Migrated

| Service | DiagnosticScope Label | Special Notes |
|---------|----------------------|---------------|
| `LiveFansMedalAppService` | `"直播间互动"` | Added 2 new ctor params |
| `LiveLotteryTaskAppService` | `"天选时刻抽奖"` | Added 2 new ctor params |
| `UnfollowBatchedTaskAppService` | `"批量取关"` | Added 2 new ctor params |
| `VipBigPointAppService` | `"大会员大积分"` | Renamed `IAccountDomainService loginDomainService` → `accountDomainService`; updated usage; added 2 new params |
| `TestAppService` | `"测试Cookie"` | Added 2 new ctor params; `SetCookiesAsync` as first statement (no IsEnable guard) |

### Pattern Applied (per service)

1. Added `using Microsoft.Extensions.Configuration;`
2. Added `using Ray.BiliBiliTool.Application.Diagnostics;`
3. Added `ILoginDomainService loginDomainService` + `IConfiguration configuration` to constructor
4. Updated base ctor: `BaseMultiAccountsAppService(logger, cookieStrFactory, loginDomainService, configuration)`
5. Added `await SetCookiesAsync(ck, cancellationToken);` after IsEnable guard
6. Wrapped `DoTaskAccountAsync` body in `TaskFlowDiagnosticScope.ExecuteAsync`

### Bug Fix

`VipBigPointAppService` had a naming conflict: the `IAccountDomainService` parameter was incorrectly named `loginDomainService`. Renamed to `accountDomainService` and updated its usage in `LoginAndCheckVipStatusAsync`.

## Verification

- `dotnet build Ray.BiliBiliTool.sln --no-restore`: 0 errors, 93 warnings (all pre-existing)
- `Select-String` audit: no `private.*Task SetCookiesAsync|SaveCookieAsync` in Application project (except `LoginTaskAppService` which is out of scope)
- ArchitectureTests: 4/4 passed
- Host.IntegrationTests: 7/7 passed

## Artifacts

- `.planning/phases/07-appservice-cookie-handling-extraction/07-03-PLAN.md`
- 5 AppService files in `src/Ray.BiliBiliTool.Application/` (modified)
