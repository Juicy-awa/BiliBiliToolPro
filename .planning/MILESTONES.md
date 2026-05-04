# Milestones

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
