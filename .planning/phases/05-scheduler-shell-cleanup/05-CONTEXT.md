# Phase 5: Scheduler Shell Cleanup - Context

**Gathered:** 2026-05-03
**Status:** Ready for planning

<domain>
## Phase Boundary

Standardize all **Quartz job classes** in the Web host so each is a minimal, consistent delegation shell. The jobs already follow the pattern established by LoginJob and DailyJob, but have small redundancies (per-job logger field, per-job "started" log, repeated registration boilerplate) that this phase tidies up. No behavioral changes — existing job identities, schedules, and observable behavior are preserved.

**In scope:**
- `BaseJob<TJob>` — pull "job started" log up from subclasses; expose `protected Logger` property
- All 12 Quartz job classes — remove redundant `_logger` field; rely on base class logger
- `ServiceCollectionQuartzConfiguratorExtensions.AddBiliJobs` — extract a helper method to reduce 12× repetition
- `DefaultCron` constant — add a comment explaining the "disabled by default" intent
- Job registration behavior and scheduling — preserved exactly, no trigger or schedule changes

**Out of scope:**
- Application service implementations (`IDailyTaskAppService`, `ILoginTaskAppService`, etc.) — not touched in Phase 5
- Quartz persistence, SQLite store configuration, or ADO job store changes
- Adding new jobs or removing existing jobs
- Cron schedule changes — default crons remain as-is
- BlazingQuartz scheduling UI concerns

</domain>

<decisions>
## Implementation Decisions

### BaseJob Responsibilities
- **D-01:** Move the `"{nameof(Job)} started."` log call into `BaseJob.Execute` before calling `DoExecuteAsync`. Use `typeof(TJob).Name` to produce the same log message. This removes the only reason subclasses needed their own logger.

### Logger Exposure
- **D-02:** Add a `protected ILogger<TJob> Logger` property to `BaseJob<TJob>` (backed by the existing primary constructor `logger` parameter). Subclasses remove their private `_logger` field entirely and can reference `Logger` if they need it in future. All 12 job subclasses remove `private readonly ILogger<TJob> _logger = logger;`.

### Job Registration Pattern
- **D-03:** Extract a private helper method in `ServiceCollectionQuartzConfiguratorExtensions` — e.g., `AddBiliJob<T>(this IServiceCollectionQuartzConfigurator quartz, JobKey key, string? configCronKey, IConfiguration config)` — that contains the `AddJob<T>` + `AddTrigger` pair. All 12 job registrations become single-line calls. The public `AddBiliJobs` method signature is unchanged.

### DefaultCron Documentation
- **D-04:** Add a code comment on the `DefaultCron` constant explaining it is a "disabled by default" placeholder (fires Jan 1 at midnight). No config keys added for LoginJob or TestBiliJob — their default-disabled behavior is intentional and stays hardcoded.

### Job Documentation
- **D-05:** No XML documentation added to job classes or BaseJob. The delegation pattern is self-explanatory. Documentation effort follows application service complexity (Phases 3–4 pattern), not scheduling shells.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Architecture & Boundaries
- `.planning/codebase/ARCHITECTURE.md` — Scheduling architecture, layer responsibilities, and composition root patterns
- `.planning/codebase/CONVENTIONS.md` — Coding patterns, naming conventions, and extension method style

### Prior Phase Context
- `.planning/phases/01-boundary-guardrails/01-CONTEXT.md` — D-11: TaskInterceptor preservation (not relevant to jobs but confirms thin-host intent)
- `.planning/phases/03-login-refactor-slice/03-CONTEXT.md` — Stable contract boundary pattern (jobs stay as thin callers of IAppService)
- `.planning/phases/04-dailytask-refactor-slice/04-CONTEXT.md` — DailyJob delegation model that Phase 5 standardizes across all jobs

### Key Source Files
- `src/Ray.BiliBiliTool.Web/Jobs/BaseJob.cs` — Base class to modify (add protected Logger, pull up started log)
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionQuartzConfiguratorExtensions.cs` — Registration to refactor with helper method
- `src/Ray.BiliBiliTool.Web/Jobs/` — All 12 job classes to clean up

### Test Artifacts
- `test/Ray.BiliBiliTool.ArchitectureTests/` — Boundary guardrails must continue passing after refactor
- `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs` — Host startup validation covering job registration

</canonical_refs>

<code_context>
## Existing Code Insights

### Current Job Pattern (all 12 jobs follow this exactly)
```csharp
public class DailyJob(ILogger<DailyJob> logger, IDailyTaskAppService appService)
    : BaseJob<DailyJob>(logger)
{
    private readonly ILogger<DailyJob> _logger = logger;              // REMOVE: redundant after D-01/D-02
    public static readonly JobKey Key = new(nameof(DailyJob), Constants.BiliJobGroup);

    protected override async Task DoExecuteAsync(IJobExecutionContext context)
    {
        _logger.LogInformation($"{nameof(DailyJob)} started.");        // MOVE UP to BaseJob
        await appService.DoTaskAsync();
    }
}
```

**Target pattern after Phase 5:**
```csharp
public class DailyJob(ILogger<DailyJob> logger, IDailyTaskAppService appService)
    : BaseJob<DailyJob>(logger)
{
    public static readonly JobKey Key = new(nameof(DailyJob), Constants.BiliJobGroup);

    protected override async Task DoExecuteAsync(IJobExecutionContext context)
        => await appService.DoTaskAsync();
}
```

### BaseJob (current)
```csharp
public abstract class BaseJob<TJob>(ILogger<TJob> logger) : IJob
    where TJob : BaseJob<TJob>
{
    public async Task Execute(IJobExecutionContext context) { ... }    // handles context, errors, version log
    protected abstract Task DoExecuteAsync(IJobExecutionContext context);
}
```

**Target: add `protected ILogger<TJob> Logger => logger;` and log `"{typeof(TJob).Name} started."` in Execute before calling DoExecuteAsync.**

### Registration Pattern (current — 12× repetition)
```csharp
quartz.AddJob<DailyJob>(opts => opts.WithIdentity(DailyJob.Key));
quartz.AddTrigger(opts =>
    opts.ForJob(DailyJob.Key)
        .WithIdentity($"{DailyJob.Key}.Cron.Trigger", Constants.BiliJobGroup)
        .WithCronSchedule(configuration["DailyTaskConfig:Cron"] ?? DefaultCron)
);
```

**Target: single helper call per job — `quartz.AddBiliJob<DailyJob>(DailyJob.Key, "DailyTaskConfig:Cron", configuration);`**

### DefaultCron Placement
- Defined in `ServiceCollectionQuartzConfiguratorExtensions` as `private const string DefaultCron = "0 0 0 1 1 ?";`
- Used by LoginJob and TestBiliJob — no config override, intentionally disabled by default

### Integration Points
- BaseJob is the only inheritance point — all 12 jobs extend it
- JobKey static fields are used by BlazingQuartz UI for job identification — must remain unchanged
- `Constants.BiliJobGroup` used across all registrations — unchanged

</code_context>

<specifics>
## Specific Expectations

- **All 12 job classes must be modified** — each removes `_logger` field and simplifies `DoExecuteAsync`
- **Log output preserved** — `"DailyJob started."` still appears in logs (produced by BaseJob instead of subclass)
- **JobKey identity preserved** — `new JobKey(nameof(XJob), Constants.BiliJobGroup)` pattern unchanged across all jobs
- **Registration behavior identical** — same Quartz job store entries, same trigger identities, same cron sources
- **Architecture tests continue passing** — no new layer violations introduced

</specifics>

<deferred>
## Deferred Ideas

- **LoginJob and TestBiliJob config keys:** Adding `LoginTaskConfig:Cron` and `TestBiliTaskConfig:Cron` to allow operator overrides was considered but deferred — their default-disabled behavior is intentional and adding config keys may confuse operators.
- **BlazingQuartz UI integration:** Any changes to how jobs appear in the scheduling UI belong in a separate UI-focused phase.
- **Job-level retry or resilience policies:** BaseJob currently swallows all exceptions; typed resilience (retry, dead-letter) is deferred to Phase 6 scope discussions.
- **Console host scheduling:** Console host does not use Quartz; any unification of scheduling between Console and Web is out of scope here.

</deferred>

---

*Phase: 5-Scheduler Shell Cleanup*
*Context gathered: 2026-05-03*
