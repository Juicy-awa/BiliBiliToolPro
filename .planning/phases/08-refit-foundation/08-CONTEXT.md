# Phase 8: Refit Foundation - Context

**Gathered:** 2026-05-04
**Status:** Ready for planning

<domain>
## Phase Boundary

Add Refit and Refit.HttpClientFactory packages (alongside WebApiClientCore, which stays for now), create `BiliBiliCommonHeadersDelegatingHandler` to replace the `AppendHeaderAttribute` behavior on `IBiliBiliApi`, wire `LogDelegatingHandler` into the client pipeline, and strip `IBiliBiliApi` of all WebApiClientCore-specific attributes so it is an empty marker interface ready for Phase 9 deletion.

**In scope:**
- Add `Refit` and `Refit.HttpClientFactory` to `Directory.Packages.props` and `Ray.BiliBiliTool.Agent.csproj`
- Create `BiliBiliCommonHeadersDelegatingHandler` — injects the 6 common headers with AddIfNotExist semantics
- Wire `LogDelegatingHandler` into `AddBiliBiliClientApi` private helper
- Strip `[AppendHeader]` and `[LogFilter]` from `IBiliBiliApi`, leaving it as `public interface IBiliBiliApi;`

**Out of scope:**
- Converting any interface attributes (Phase 9)
- Removing `: IBiliBiliApi` from inheriting interfaces (Phase 9)
- DI registration switch from `AddHttpApi<T>` to `AddRefitClient<T>` (Phase 10)
- Removing WebApiClientCore package (Phase 10)
- Deleting `AppendHeaderAttribute.cs`, `AppendHeaderType.cs`, `LogFilterAttribute.cs` (Phase 10)

</domain>

<decisions>
## Implementation Decisions

### Logging
- **D-01:** Wire `LogDelegatingHandler` in Phase 8 — add it to the `AddBiliBiliClientApi` private helper chain alongside `IntervalDelegatingHandler`. All Bilibili clients get consistent request/response debug logging from Phase 8 onwards.

### IBiliBiliApi Disposition
- **D-02:** `IBiliBiliApi` is emptied in Phase 8 (strip `[AppendHeader]` and `[LogFilter]` attributes, remove `using Ray.BiliBiliTool.Agent.Attributes;` and `using WebApiClientCore.Attributes;`). The file is NOT deleted in Phase 8 because 14 interface files still inherit from it. Phase 9 removes `: IBiliBiliApi` from all inheriting interfaces and deletes the file.

### BiliBiliCommonHeadersDelegatingHandler — Header Set
- **D-03:** The handler must inject the following 6 headers with AddIfNotExist semantics (i.e., only add if the header is not already present on the outgoing request):
  - `Accept: application/json, text/plain, */*`
  - `Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6`
  - `Sec-Fetch-Dest: empty`
  - `Sec-Fetch-Mode: cors`
  - `Sec-Fetch-Site: same-site`
  - `Connection: keep-alive`
- **D-04:** This handler is added to the `AddBiliBiliClientApi` private helper — applies to all 17 Bilibili clients. It does NOT apply to QingLong (registered separately). `IVipMallApi`, `IMallApi`, and `IVipBigPointApi` do not inherit `IBiliBiliApi` but are registered through `AddBiliBiliClientApi`, so they will receive the headers too — acceptable since these headers are harmless on any Bilibili host.

### Package Versions
- **D-05:** Add `Refit` and `Refit.HttpClientFactory`. Target the latest stable Refit version compatible with .NET 8 (verify at write time). Use central package version management — add to `Directory.Packages.props`, reference without version in `Agent.csproj`.

### the agent's Discretion
- Handler registration order in `AddBiliBiliClientApi`: `LogDelegatingHandler` → `BiliBiliCommonHeadersDelegatingHandler` → `IntervalDelegatingHandler` → Polly → (`WridEncryptionDelegatingHandler` if applicable). Log first so the raw request (before interval delay) is captured; common headers before interval so they appear in logs.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Agent Layer — Current Structure
- `src/Ray.BiliBiliTool.Agent/Extensions/ServiceCollectionExtension.cs` — current `AddBiliBiliClientApi` private helper and all client registrations; Phase 8 adds handlers here
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/IBiliBiliApi.cs` — the interface being emptied; contains the AppendHeader and LogFilter attributes to remove
- `src/Ray.BiliBiliTool.Agent/HttpClientDelegatingHandlers/LogDelegatingHandler.cs` — existing handler to wire in Phase 8
- `src/Ray.BiliBiliTool.Agent/HttpClientDelegatingHandlers/IntervalDelegatingHandler.cs` — existing handler; BiliBiliCommonHeadersDelegatingHandler inserts before or after it
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/WridEncryptionDelegatingHandler.cs` — conditionally applied; stays as-is

### Packages
- `Directory.Packages.props` — central version management; add Refit entries here
- `src/Ray.BiliBiliTool.Agent/Ray.BiliBiliTool.Agent.csproj` — add `<PackageReference Include="Refit" />` and `<PackageReference Include="Refit.HttpClientFactory" />`

### Requirements
- `.planning/REQUIREMENTS.md` — REFIT-04 and REFIT-05 are the requirements this phase addresses

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `LogDelegatingHandler` — logs method + URI, request content, response content at Debug level. Already DI-registered via Scrutor scan (`FromAssemblyOf<IBiliBiliApi>()` → classes assignable to `DelegatingHandler`). Just needs to be added to the pipeline in `AddBiliBiliClientApi`.
- `IntervalDelegatingHandler` — rate-limiting handler, already wired. `BiliBiliCommonHeadersDelegatingHandler` should insert before it so headers are set on the raw request.
- `Scrutor` assembly scan in `ServiceCollectionExtension` registers all `DelegatingHandler` subclasses as Transient automatically — new handler will be picked up without extra registration line.

### Established Patterns
- All delegating handlers are registered as Transient via Scrutor scan, then added to the pipeline with `.AddHttpMessageHandler<THandler>()`.
- `AddBiliBiliClientApi` private helper is the single place to modify for all Bilibili clients — changes here apply to all 17 + QingLong (QingLong is registered separately so it won't get the new handlers unless explicitly added).
- The `ignorWrid` parameter controls whether `WridEncryptionDelegatingHandler` is added — keep this param, add new handlers unconditionally.

### Integration Points
- `BiliBiliCommonHeadersDelegatingHandler` slots into the `AddBiliBiliClientApi` chain. No other files change in Phase 8.
- `IBiliBiliApi.cs` is the only interface file modified in Phase 8; the 14 inheriting interfaces are untouched until Phase 9.

</code_context>

<specifics>
## Specific Ideas

- AddIfNotExist semantics for header injection: check `request.Headers.Contains(headerName)` before calling `request.Headers.TryAddWithoutValidation(headerName, value)`.
- The handler class goes in `src/Ray.BiliBiliTool.Agent/HttpClientDelegatingHandlers/` alongside the existing handlers.

</specifics>

<deferred>
## Deferred Ideas

- Removing `AppendHeaderAttribute.cs`, `AppendHeaderType.cs`, `LogFilterAttribute.cs` — Phase 10 (after all usages are gone)
- Removing WebApiClientCore package — Phase 10
- Deleting `IBiliBiliApi.cs` and removing `: IBiliBiliApi` from 14 files — Phase 9

</deferred>
