# Phase 7: AppService Cookie Handling Extraction — Context

**Gathered:** 2026-05-04
**Status:** Ready for planning

<domain>
## Phase Boundary

Apply the same refactoring treatment as Phase 3 (LoginTask) and Phase 4 (DailyTask) to all remaining AppServices. This includes:

1. Moving the duplicated `SetCookiesAsync` + `SaveCookieAsync` into `BaseMultiAccountsAppService` directly (no new intermediate class — these operations belong in the common base that all services share)
2. Adding `TaskFlowDiagnosticScope` wrapping in `DoTaskAccountAsync` / `DoTaskAsync` for all applicable services
3. Filling in the gap for services that were missing `SetCookiesAsync` calls entirely

**In scope — 11 AppServices (all except LoginTaskAppService, which is the login service itself):**
- `DailyTaskAppService` — remove private copies, inherit from base (already has DiagnosticScope)
- `ChargeTaskAppService` — remove private copies, add DiagnosticScope
- `MangaPrivilegeTaskAppService` — remove private copies, add DiagnosticScope
- `MangaTaskAppService` — remove private copies, add DiagnosticScope
- `Silver2CoinTaskAppService` — remove private copies, add DiagnosticScope
- `VipPrivilegeTaskAppService` — remove private copies, add DiagnosticScope
- `LiveFansMedalAppService` — add SetCookiesAsync call (missing), add DiagnosticScope
- `LiveLotteryTaskAppService` — add SetCookiesAsync call (missing), add DiagnosticScope
- `UnfollowBatchedTaskAppService` — add SetCookiesAsync call (missing), add DiagnosticScope
- `VipBigPointAppService` — add SetCookiesAsync call (missing), add DiagnosticScope
- `TestAppService` — add SetCookiesAsync call (missing), add DiagnosticScope

**Out of scope:**
- LoginTaskAppService — it IS the login/cookie service itself
- Domain service implementations — application layer refactor only
- BaseMultiAccountsAppService behavioral changes beyond adding the two protected virtual methods
- XML documentation — explicitly deferred by user

</domain>

<decisions>
## Implementation Decisions

### D-01: No new intermediate base class
The user explicitly chose **not** to introduce a new `BaseCookieAwareAppService`. Instead, `SetCookiesAsync` and `SaveCookieAsync` are added directly to `BaseMultiAccountsAppService`. Rationale: every multi-account AppService should perform this operation — the absence in some services was an oversight, not an intentional design difference.

### D-02: BaseMultiAccountsAppService receives two new dependencies
To host `SetCookiesAsync` and `SaveCookieAsync`, `BaseMultiAccountsAppService` must add `ILoginDomainService loginDomainService` and `IConfiguration configuration` to its constructor. All 11 inheriting services must pass these up to the base via their own constructors. Services that don't currently inject them (Live*, Unfollow, VipBigPoint, Test) must add them.

### D-03: Methods are protected virtual
`SetCookiesAsync` and `SaveCookieAsync` in `BaseMultiAccountsAppService` are `protected virtual` so subclasses can override them if they need service-specific cookie handling. The base implementation is identical to the existing private implementations across the 6 services that had it.

### D-04: All 11 services call SetCookiesAsync
Every service that inherits `BaseMultiAccountsAppService` (all except `LoginTaskAppService`) should call `SetCookiesAsync(ck, cancellationToken)` at the start of its `DoTaskAccountAsync`. Services that were missing this call (Live*, Unfollow, VipBigPoint, Test) were considered incomplete — fill the gap.

### D-05: TaskFlowDiagnosticScope for all applicable services
All 11 services should wrap their main task body in `TaskFlowDiagnosticScope.ExecuteAsync(logger, "[ServiceName]", async () => { ... })` — matching the pattern established in `LoginTaskAppService` and `DailyTaskAppService`. The label string should be the task name in Chinese to match existing conventions (e.g., "充电任务", "漫画权益任务", etc.).

### D-06: DailyTaskAppService included in this phase
Although DailyTaskAppService was refactored in Phase 4, it still has its own private `SetCookiesAsync` and `SaveCookieAsync` copies. It must be updated to inherit these from the base, removing its private copies. Its existing `TaskFlowDiagnosticScope` is already correct and should be preserved.

### D-07: XML documentation explicitly skipped
Do NOT add XML documentation comments to any of the 11 services in this phase. The user explicitly deferred this for now.

### D-08: Behavior preservation is mandatory
All existing characterization and integration tests must continue to pass. The `SetCookiesAsync` / `SaveCookieAsync` logic must be byte-for-byte identical to the current private implementations — no behavioral changes allowed. This is a structural refactor only.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Architecture & Boundaries
- `.planning/codebase/ARCHITECTURE.md` — Layer boundaries and composition patterns
- `.planning/codebase/CONVENTIONS.md` — Naming conventions, service registration style

### Prior Phase Context
- `.planning/phases/03-login-refactor-slice/03-CONTEXT.md` — Pattern to follow: LoginTaskAppService refactor (DiagnosticScope, step methods)
- `.planning/phases/04-dailytask-refactor-slice/04-CONTEXT.md` — Pattern to follow: DailyTaskAppService refactor

### Test Artifacts (must remain green)
- `test/Ray.BiliBiliTool.CharacterizationTests/` — Frozen behavior baseline — must not regress
- `test/Ray.BiliBiliTool.Host.IntegrationTests/` — Host startup and integration — must not regress
- `test/Ray.BiliBiliTool.ArchitectureTests/` — ArchUnitNET boundary guardrails — must not regress

### Source Files to Modify
- `src/Ray.BiliBiliTool.Application/BaseMultiAccountsAppService.cs` — Add the two protected virtual methods + new constructor deps
- `src/Ray.BiliBiliTool.Application/DailyTaskAppService.cs` — Remove private copies, pass deps to base
- `src/Ray.BiliBiliTool.Application/ChargeTaskAppService.cs` — Remove private copies, add DiagnosticScope, pass deps to base
- `src/Ray.BiliBiliTool.Application/MangaPrivilegeTaskAppService.cs` — Remove private copies, add DiagnosticScope, pass deps to base
- `src/Ray.BiliBiliTool.Application/MangaTaskAppService.cs` — Remove private copies, add DiagnosticScope, pass deps to base
- `src/Ray.BiliBiliTool.Application/Silver2CoinTaskAppService.cs` — Remove private copies, add DiagnosticScope, pass deps to base
- `src/Ray.BiliBiliTool.Application/VipPrivilegeTaskAppService.cs` — Remove private copies, add DiagnosticScope, pass deps to base
- `src/Ray.BiliBiliTool.Application/LiveFansMedalAppService.cs` — Add SetCookiesAsync call, add DiagnosticScope, add new deps to constructor
- `src/Ray.BiliBiliTool.Application/LiveLotteryTaskAppService.cs` — Add SetCookiesAsync call, add DiagnosticScope, add new deps to constructor
- `src/Ray.BiliBiliTool.Application/UnfollowBatchedTaskAppService.cs` — Add SetCookiesAsync call, add DiagnosticScope, add new deps to constructor
- `src/Ray.BiliBiliTool.Application/VipBigPointAppService.cs` — Add SetCookiesAsync call, add DiagnosticScope, add new deps to constructor
- `src/Ray.BiliBiliTool.Application/TestAppService.cs` — Add SetCookiesAsync call, add DiagnosticScope, add new deps to constructor

</canonical_refs>

<code_context>
## Existing Code Insights

### BaseMultiAccountsAppService (current)
```csharp
public abstract class BaseMultiAccountsAppService(
    ILogger logger,
    CookieStrFactory<BiliCookie> cookieStrFactory
) : AppService
{
    public override async Task DoTaskAsync(CancellationToken cancellationToken = default) { ... }
    protected abstract Task DoTaskAccountAsync(BiliCookie ck, CancellationToken cancellationToken = default);
}
```
Will gain: `ILoginDomainService loginDomainService`, `IConfiguration configuration` as constructor params, plus two `protected virtual` methods.

### The two extracted methods (current private impl — identical across 6 services)
```csharp
// To become: protected virtual async Task SetCookiesAsync(BiliCookie biliCookie, CancellationToken cancellationToken)
private async Task SetCookiesAsync(BiliCookie biliCookie, CancellationToken cancellationToken)
{
    if (!string.IsNullOrWhiteSpace(biliCookie.Buvid))
    {
        logger.LogInformation("Cookie完整，不需要Set Cookie");
        return;
    }
    logger.LogInformation("开始Set Cookie");
    var ck = await loginDomainService.SetCookieAsync(biliCookie, cancellationToken);
    logger.LogInformation("持久化Cookie");
    await SaveCookieAsync(ck, cancellationToken);
}

// To become: protected virtual async Task SaveCookieAsync(BiliCookie ckInfo, CancellationToken cancellationToken)
private async Task SaveCookieAsync(BiliCookie ckInfo, CancellationToken cancellationToken)
{
    var platformType = configuration.GetSection("PlatformType").Get<PlatformType>();
    logger.LogInformation("当前运行平台：{platform}", platformType);
    if (platformType == PlatformType.QingLong)
    {
        await loginDomainService.SaveCookieToQinLongAsync(ckInfo, cancellationToken);
        return;
    }
    await loginDomainService.SaveCookieToJsonFileAsync(ckInfo, cancellationToken);
}
```

### DiagnosticScope pattern (from DailyTaskAppService)
```csharp
await TaskFlowDiagnosticScope.ExecuteAsync(
    logger,
    "DailyTask",           // ← label (use Chinese task name for consistency)
    async () =>
    {
        // ... task body ...
    }
);
```

### TaskFlowDiagnosticScope location
`src/Ray.BiliBiliTool.Application/Diagnostics/TaskFlowDiagnosticScope.cs`

### Services NOT requiring SetCookiesAsync migration
`LoginTaskAppService` — this IS the login/cookie service; it handles the full flow natively.
</code_context>

<deferred_ideas>
## Deferred Ideas (noted but not in this phase)

- XML documentation for the 9 remaining AppServices — user deferred for now
- ARCH-04: Notification adapter/port boundary — still deferred beyond this milestone
</deferred_ideas>
