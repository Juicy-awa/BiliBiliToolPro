# Milestones

## v4.0.0.4 — Agent Interface Consolidation

**Shipped:** 2026-05-04
**Phases:** 1 (Phase 11) | **Plans:** 4 | **Commits:** 5

### Delivered

Merged all 8 `api.bilibili.com` Refit interfaces into a single `IApiApi` with `#region` organization by domain. Eliminated 8 old interface files, reduced DI from 8 registrations to 1. Updated 9 DomainServices and 5 test files. Fixed critical DI regression (7 accidentally-deleted service registrations) caught during milestone audit. Retained `INavApi` separately to avoid `IWbiService` circular dependency.

### Key Accomplishments

1. `IApiApi.cs` created — 8 `#region` sections (UpInfo, 每日任务, 关注, 充电, 视频, 专栏, 大会员积分, 商城) merging 8 old interfaces
2. IVipBigPointApi DI host registration bug fixed — was `BiliHosts.App`, correct is `BiliHosts.Api`; merged into IApiApi inline
3. 8 old interface files deleted; DI reduced from 8 `AddBiliBiliClientApi` calls to 1 with `MutatingPolicy()`
4. 9 DomainServices and 5 test files updated to inject `IApiApi` — 0 references to old interfaces remain
5. DI regression (`8d56f41`) fixed during audit — restored 7 accidentally-deleted registrations (IShowApi, IPassportApi, ILiveTraceApi, IHomeApi, IMangaApi, IAccountApi, ILiveApi)

### Stats

- Git range: `0fe810f` → `872d389`
- Files changed: 47 (+1,503/−751)
- Build: 0 errors | ArchitectureTests: 4/4 | IntegrationTests: 7/7

### Known Tech Debt

- Pre-existing test failure: `Daily_task_multi_account_wrapper_continues_after_account_failure` (open since Phase 5)
- ARCH-04: Notification adapter/port boundary deferred beyond this milestone
- IFACE-02: `#region` convention not enforced by architecture tests

### Archive

- [v4.0.0.4-ROADMAP.md](milestones/v4.0.0.4-ROADMAP.md)
- [v4.0.0.4-REQUIREMENTS.md](milestones/v4.0.0.4-REQUIREMENTS.md)
- [v4.0.0.4-MILESTONE-AUDIT.md](v4.0.0.4-MILESTONE-AUDIT.md)

---

## v4.0.0.3 — Refit Migration

**Shipped:** 2026-05-04
**Phases:** 3 (Phases 8–10) | **Plans:** 4 | **Commits:** 13

### Delivered

Replaced WebApiClientCore with Refit 8.0.0 across all 18 Agent-layer interfaces. Created `BiliBiliCommonHeadersDelegatingHandler` for common header injection. Migrated all DI registrations from `AddHttpApi<T>` to `AddRefitClient<T>`. Removed WebApiClientCore package and 4 legacy attribute files. Net −154 lines of code (460 deleted, 306 added).

### Key Accomplishments

1. `BiliBiliCommonHeadersDelegatingHandler` created — injects 6 Bilibili common headers with AddIfNotExist semantics, replacing `AppendHeaderAttribute`
2. All 17 Bilibili API interfaces converted (9 api.bilibili.com group + 8 other-host group) — `[HttpGet]`/`[FormContent]`/`[PathQuery]` → Refit equivalents
3. `IQingLongApi` converted with `[Get]`/`[Post]`/`[Put]` + `[Query]`/`[Body]`/`[Header]` attributes
4. All 18 DI registrations migrated to `AddRefitClient<T>()` with Polly policies and handler chains preserved
5. WebApiClientCore package fully removed; 4 legacy files deleted; net code reduction
6. `IVideoApi.UploadVideoHeartbeat` URL template bug surfaced and fixed (Refit requires explicit params; WebApiClientCore resolved from DTO body)

### Stats

- Git range: `682b0c9` → `3bc155d`
- src/ files changed: 27 (+306/−460)
- Build: 0 errors | ArchitectureTests: 4/4 | IntegrationTests: 7/7 | UAT: 5/5

---

## v4.0.0.2 — AppService Refactor Continuation

**Shipped:** 2026-05-04
**Phases:** 1 | **Plans:** 4 | **Commits:** 10

### Delivered

Eliminated duplicated `SetCookiesAsync` / `SaveCookieAsync` private methods from all 11 in-scope AppServices by centralizing them as `protected virtual` methods in `BaseMultiAccountsAppService`. Added `TaskFlowDiagnosticScope` telemetry to all 11 services.

### Key Accomplishments

1. `BaseMultiAccountsAppService` upgraded with `ILoginDomainService` + `IConfiguration` constructor deps and two `protected virtual` cookie methods — single source of truth for cookie handling.
2. 6 Group A services (DailyTask, Charge, MangaPrivilege, Manga, Silver2Coin, VipPrivilege) — private copies removed, base ctor wired, DiagnosticScope added.
3. 5 Group B services (LiveFansMedal, LiveLottery, UnfollowBatched, VipBigPoint, Test) — previously missing `SetCookiesAsync` call added, ctor deps injected, DiagnosticScope added.
4. `VipBigPointAppService` naming conflict resolved: `accountDomainService` vs `loginDomainService` correctly distinguished.
5. All 11 services now emit `TaskFlowDiagnosticScope.ExecuteAsync` telemetry with Chinese labels matching the established pattern.

### Stats

- Git range: 348233e (develop) → HEAD (feature/gsd-appservice)
- Phase: 07-appservice-cookie-handling-extraction (4 plans)
- Build: 0 errors | ArchitectureTests: 4/4 | IntegrationTests: 7/7
- UAT: 7/7 passed

### Known Gaps

- No VERIFICATION.md generated during execute-phase (covered by UAT 7/7 + test results).
- VALIDATION.md (Nyquist) not created — run `/gsd-validate-phase 7` optionally.
- `LoginTaskAppService` retains private cookie methods — explicitly out of scope per D-01 (it IS the login service itself).

---

## v4.0.0.1 — Brownfield Refactor

**Shipped:** 2026-05-03
**Phases:** 6 | **Plans:** 13 | **Execution time:** ~1.7 hours

### Delivered

Made the BiliBiliToolPro codebase safe to change through executable architecture guardrails, characterization tests, thin Quartz scheduling shells, and a typed exception/adapter boundary model.

### Key Accomplishments

1. ArchUnitNET dependency guardrails enforce layer direction across Agent, Application, DomainService, Infrastructure, and Web — violations fail the build.
2. Characterization tests freeze Login and DailyTask observable behavior so refactors can be safely validated.
3. Web and Console startup paths delegate business orchestration through named seams; host integration tests prove it.
4. All 12 Quartz job classes reduced to thin expression-body delegation shells over application use cases.
5. BiliException hierarchy (Business / Integration / Validation) enables distinguishable failure modes across DomainService and Agent layers.
6. IExecutionLogRepository and IUserRepository decouple Web and EF-facing code from direct `IDbContextFactory` injection.

### Stats

- Git range: cba2ea4 → 8def091
- Files changed: 128 files, 8707 insertions, 264 deletions
- Timeline: 2026-04-20 → 2026-05-03

### Known Gaps

- **ARCH-04 partial:** Notification-facing boundary not established — notifications still flow through `Ray.Serilog.Sinks.Batched` directly without an explicit adapter port. Deferred to next milestone.
- **Process artifacts:** No VERIFICATION.md generated for any of 6 phases; Phase 5/6 SUMMARY files lack YAML frontmatter; REQUIREMENTS.md traceability checkboxes not updated during execution.
- **Pre-existing test failure:** `Daily_task_multi_account_wrapper_continues_after_account_failure` — confirmed pre-Phase 5. Root cause not investigated this milestone.
- **Deferred (D-05):** `AuthService` generic `Exception` throw kept as-is.

### Archive

- [v4.0.0.1-ROADMAP.md](milestones/v4.0.0.1-ROADMAP.md)
- [v4.0.0.1-REQUIREMENTS.md](milestones/v4.0.0.1-REQUIREMENTS.md)
- [v4.0.0.1-MILESTONE-AUDIT.md](milestones/v4.0.0.1-MILESTONE-AUDIT.md)
