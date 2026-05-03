# Phase 05 Plan 01 Summary — BaseJob Enhancement + AddBiliJob Helper

## What Was Done

**Task 1 — BaseJob enhanced:**
- Added `protected ILogger<TJob> Logger { get; } = logger;` as the first member of `BaseJob<TJob>`
- Added `logger.LogInformation($"{typeof(TJob).Name} started.");` inside the `try` block immediately before `await DoExecuteAsync(context)`
- All other Execute logic (catch, finally, BatchSinkManager flush) preserved exactly

**Task 2 — AddBiliJob helper extracted:**
- Added `// Fires Jan 1 at midnight — disabled by default.` comment to `DefaultCron` constant
- Extracted `private static void AddBiliJob<TJob>(IServiceCollectionQuartzConfigurator quartz, JobKey key, string? configCronKey, IConfiguration configuration)` helper method
- Replaced all 12 AddJob+AddTrigger pairs in `AddBiliJobs` with single-line `AddBiliJob<T>(...)` calls
- Trigger identity format `$"{key}.Cron.Trigger"` and group `Constants.BiliJobGroup` preserved exactly

## Files Changed

- `src/Ray.BiliBiliTool.Web/Jobs/BaseJob.cs` — Logger property + started log
- `src/Ray.BiliBiliTool.Web/Extensions/ServiceCollectionQuartzConfiguratorExtensions.cs` — AddBiliJob helper + 12 single-line registrations

## Verification

- Build: 0 errors (19 pre-existing warnings, including CS9124 on BaseJob which is expected — primary constructor parameter captured in both property initializer and method body)
- Architecture tests: 4/4 passed
- Host integration tests: 7/7 passed

## Commit

`feat(05-01): enhance BaseJob with Logger property and started log; extract AddBiliJob registration helper`

## Must-Have Verification

- [x] `BaseJob.Execute` logs `"{typeof(TJob).Name} started."` before calling `DoExecuteAsync`
- [x] `BaseJob<TJob>` exposes `protected ILogger<TJob> Logger { get; }`
- [x] Each job registration in `AddBiliJobs` is a single `AddBiliJob` call
- [x] `DefaultCron` constant has comment explaining disabled-by-default schedule
- [x] Web host builds and integration tests pass unchanged
