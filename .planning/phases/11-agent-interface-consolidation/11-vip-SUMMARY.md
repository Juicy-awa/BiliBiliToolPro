# Summary: 11-vip — Merge IVipBigPointApi into IApiApi

## What Was Built
Merged `IVipBigPointApi` (an `api.bilibili.com` interface incorrectly registered with `BiliHosts.App`) into `IApiApi`, fixing both the DI host registration bug and completing the interface consolidation.

## Root Cause
`IVipBigPointApi` had `[Headers("Host: api.bilibili.com")]` at the interface level but was registered in DI using `BiliHosts.App` + `configApp`, causing requests to be sent to the wrong host. User investigation confirmed the correct host is `api.bilibili.com`.

## Files Modified

### Agent
- `IApiApi.cs` — added `#region 大会员积分` with all 12 methods from IVipBigPointApi (6 renamed to avoid collision: `GetVipBigPointCombineAsync`, `VipBigPointSignAsync`, `VipBigPointReceive`, `VipBigPointReceiveV2`, `VipBigPointCompleteAsync`, `VipBigPointCompleteV2`, `VipBigPointViewComplete`); added 3 new usings for Mall/VipTask DTOs
- `ServiceCollectionExtension.cs` — removed `IVipBigPointApi` registration

### DomainService
- `VipBigPointDomainService.cs` — removed `IVipBigPointApi vipApi` from constructor; replaced 8 `vipApi.X` call sites with `apiApi.X` (with renamed method names)

### Tests
- `VipBigPointApiTest.cs` — switched `IVipBigPointApi` → `IApiApi`; updated 3 method calls (`GetCombineAsync`→`GetVipBigPointCombineAsync`, `SignAsync`→`VipBigPointSignAsync`, `CompleteAsync`→`VipBigPointCompleteAsync`)
- `VipServiceTest.cs` — switched `IVipBigPointApi` → `IApiApi`; updated 2 method calls (`CompleteV2`→`VipBigPointCompleteV2`, `ReceiveV2`→`VipBigPointReceiveV2`)

## Files Deleted
- `IVipBigPointApi.cs` — replaced entirely by `#region 大会员积分` in `IApiApi`

## Commit
`872d389` feat(11-vip): merge IVipBigPointApi into IApiApi; fix DI host registration

## Self-Check: PASSED
- Full solution build: 0 errors
- Architecture tests: 4/4
- Integration tests: 7/7
