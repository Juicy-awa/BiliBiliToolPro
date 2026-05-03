# Plan 06-02 Summary: IExecutionLogRepository + IUserRepository EF Adapter Boundaries

**Phase:** 06-integration-boundary-and-failure-model
**Plan:** 02
**Completed:** 2026-05-03
**Commit:** feat(06-02): introduce IExecutionLogRepository and IUserRepository; remove direct EF factory injection from LogsDialog and AuthService

## What Was Built

### Domain Interfaces
- `src/Ray.BiliBiliTool.Domain/IExecutionLogRepository.cs` — `GetLatestRunInstanceIdAsync` + `GetLogsForRunAsync`
- `src/Ray.BiliBiliTool.Domain/IUserRepository.cs` — `FindByUsernameAsync` + `GetAdminAsync` + `UpdateAsync`

### Infrastructure.EF Implementations
- `src/Ray.BiliBiliTool.Infrastructure.EF/ExecutionLogRepository.cs` — EF-backed implementation wrapping `BiliDbContext.ExecutionLogs` and `BiliDbContext.BiliLogs`
- `src/Ray.BiliBiliTool.Infrastructure.EF/UserRepository.cs` — EF-backed implementation wrapping `BiliDbContext.Users`

### DI Registration
Updated `Infrastructure.EF/Extensions/ServiceCollectionExtension.cs` to register both repositories as `Scoped`.

### Consumers Updated
- `LogsDialog.razor.cs`: Replaced `IDbContextFactory<BiliDbContext>` injection with `IExecutionLogRepository`. `OnInitializedAsync` and `OnRefreshLogs` now delegate to repository methods.
- `AuthService.cs`: Replaced `IDbContextFactory<BiliDbContext>` primary constructor parameter with `IUserRepository`. All three methods (`LoginAsync`, `ChangePasswordAsync`, `GetAdminUserNameAsync`) use repository. The existing `throw new Exception("Current password is incorrect.")` was intentionally left unchanged (excluded from Phase 6 sweep per D-05).

## Verification Results
- `dotnet build Ray.BiliBiliTool.sln`: 0 errors, 102 pre-existing warnings
- Grep for `IDbContextFactory` in `LogsDialog.razor.cs`: 0 matches ✓
- Grep for `IDbContextFactory` in `AuthService.cs`: 0 matches ✓

## Files Modified
- `src/Ray.BiliBiliTool.Domain/IExecutionLogRepository.cs` (created)
- `src/Ray.BiliBiliTool.Domain/IUserRepository.cs` (created)
- `src/Ray.BiliBiliTool.Infrastructure.EF/ExecutionLogRepository.cs` (created)
- `src/Ray.BiliBiliTool.Infrastructure.EF/UserRepository.cs` (created)
- `src/Ray.BiliBiliTool.Infrastructure.EF/Extensions/ServiceCollectionExtension.cs` (modified)
- `src/Ray.BiliBiliTool.Web/Components/Pages/Schedules/LogsDialog.razor.cs` (modified)
- `src/Ray.BiliBiliTool.Web/Services/AuthService.cs` (modified)
