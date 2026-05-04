# Phase 10 Plan 01 — SUMMARY

**Plan:** 10-01 (Wave 1)
**Status:** COMPLETE
**Commit:** 9adbd40

## Objective

Complete the Refit migration by converting `IQingLongApi`, rewriting all DI registrations from `AddHttpApi<T>` to `AddRefitClient<T>`, deleting the four legacy WebApiClientCore attribute files, and removing the `WebApiClientCore` package from the solution entirely.

## Files Changed

| File | Action |
|------|--------|
| `src/Ray.BiliBiliTool.Agent/QingLong/IQingLongApi.cs` | Rewritten with Refit attributes (`[Get]`, `[Post]`, `[Put]`, `[Query]`, `[Body]`, `[Header]`) |
| `src/Ray.BiliBiliTool.Agent/Extensions/ServiceCollectionExtension.cs` | Added `using Refit;`; replaced `AddHttpApi<TInterface>(o => { o.HttpHost = ... })` → `AddRefitClient<TInterface>().ConfigureHttpClient((_, c) => c.BaseAddress = ...)` for the private helper; replaced QingLong `AddHttpApi<IQingLongApi>` block with `AddRefitClient<IQingLongApi>()` + merged `ConfigureHttpClient` |
| `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/IVideoApi.cs` | Fixed: added explicit `long aid, int playedTime` params to `UploadVideoHeartbeat` to satisfy Refit URL template `{aid}` and `{playedTime}` |
| `src/Ray.BiliBiliTool.DomainService/VideoDomainService.cs` | Updated 2 `UploadVideoHeartbeat` call sites to pass `request.Aid, request.Played_time` as first two args |
| `src/Ray.BiliBiliTool.DomainService/VipBigPointDomainService.cs` | Updated 1 `UploadVideoHeartbeat` call site likewise |
| `src/Ray.BiliBiliTool.Agent/Attributes/AppendHeaderAttribute.cs` | **Deleted** |
| `src/Ray.BiliBiliTool.Agent/Attributes/AppendHeaderType.cs` | **Deleted** |
| `src/Ray.BiliBiliTool.Agent/Attributes/LogFilterAttribute.cs` | **Deleted** |
| `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Attributes/WbiParameterAttribute.cs` | **Deleted** (implements `IApiParameterAttribute` from WebApiClientCore; zero usages post-Phase 9) |
| `src/Ray.BiliBiliTool.Agent/Ray.BiliBiliTool.Agent.csproj` | Removed `<PackageReference Include="WebApiClientCore" />` |
| `Directory.Packages.props` | Removed `<PackageVersion Include="WebApiClientCore" Version="2.1.5" />` |

## DI Migration Pattern Applied

| Old (WebApiClientCore) | New (Refit) |
|---|---|
| `services.AddHttpApi<T>(o => { o.HttpHost = uri; o.UseDefaultUserAgent = false; })` | `services.AddRefitClient<T>().ConfigureHttpClient((_, c) => c.BaseAddress = new Uri(host))` |
| QingLong: `AddHttpApi<IQingLongApi>` + two chained `ConfigureHttpClient` | `AddRefitClient<IQingLongApi>().ConfigureHttpClient((sp, c) => { c.BaseAddress = ...; c.DefaultRequestHeaders.Add(...); c.Timeout = ...; })` |

## Bug Fixed (discovered during verification)

`IVideoApi.UploadVideoHeartbeat` had URL template params `{aid}` and `{playedTime}` with no matching method parameters — WebApiClientCore resolved these from the body DTO implicitly, but Refit requires explicit method params. Fixed by adding `long aid` and `int playedTime` as the first two method parameters and updating all 3 call sites.

## Build Status

- 0 errors, 108 warnings (all pre-existing: Rougamo analyzer version, RF001 on IVipBigPointApi stubs, CS9042 obsolete member)
- Architecture tests: 4/4 passed
- Integration tests: 7/7 passed

## Remaining WebApiClientCore References

Zero functional references. One comment in `BiliBiliCommonHeadersDelegatingHandler.cs` line 5 mentioning WebApiClientCore for historical context — intentionally retained.
