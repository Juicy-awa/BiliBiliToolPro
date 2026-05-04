# Summary: 11-02 — Update DI + Delete Old Interface Files

## What Was Built
Replaced 7 separate `AddBiliBiliClientApi` calls for `api.bilibili.com` with one:
```csharp
services.AddBiliBiliClientApi<IApiApi>(BiliHosts.Api, config, policy: BiliResiliencePolicies.MutatingPolicy());
```
Deleted 6 old interface files from the project.

## Key Files
- **Modified:** `src/Ray.BiliBiliTool.Agent/Extensions/ServiceCollectionExtension.cs` — 7 registrations → 1
- **Deleted:** IUpInfoApi.cs, IDailyTaskApi.cs, IRelationApi.cs, IChargeApi.cs, IVideoApi.cs, IArticleApi.cs

## Commit
`598b456` feat(11-02): replace 7 DI registrations with IApiApi; delete old interface files

## Self-Check: PASSED
- ServiceCollectionExtension.cs has exactly one AddBiliBiliClientApi&lt;IApiApi&gt;
- MutatingPolicy() applied
- IUserInfoApi registration unchanged (ignoreWrid=true)
- All 6 old files deleted from filesystem and git tracking
- Agent project: 0 errors, 0 warnings
