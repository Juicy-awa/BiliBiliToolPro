# Retrospectives

## v4.0.0.3 — Refit Migration (2026-05-04)

### What Was Built

Replaced WebApiClientCore with Refit 8.0.0 across all 18 Agent-layer HTTP interfaces. Created `BiliBiliCommonHeadersDelegatingHandler` for common header injection. Migrated all DI registrations from `AddHttpApi<T>` to `AddRefitClient<T>`. Removed WebApiClientCore package and 4 legacy attribute files. Net code reduction: +306/−460 lines.

### What Worked

- **Parallel plan structure paid off immediately.** Phase 9's two-plan split (api.bilibili.com group vs other-host group) let both batches be planned at the same time with zero file conflicts. No sequential dependency — clean parallelism.
- **Systematic attribute mapping table was the key artifact.** Having an explicit mapping (`[FormContent]` → `[Body(BodySerializationMethod.UrlEncoded)]`, `[HttpGet]` → `[Get]`, etc.) in the research meant execution was mechanical and consistent across all 17 files.
- **Test harnesses caught the URL template bug before it reached prod.** `IVideoApi.UploadVideoHeartbeat` had template params `{aid}` and `{playedTime}` with no matching method params — WebApiClientCore resolved from DTO body implicitly, Refit does not. The integration tests caught this within minutes of execution.
- **Version pinning decision made early avoided blocked execution.** Refit 10.x requires `Microsoft.Extensions.Http >= 9.0.3`, incompatible with .NET 8. Pinning to Refit 8.0.0 in Phase 8 planning (before any execution) meant Phases 9 and 10 had zero package resolution issues.

### What Was Inefficient

- **IVideoApi URL template bug required retroactive call site changes.** Three call sites in `VideoDomainService` and `VipBigPointDomainService` had to be updated after the bug was found. If the PLAN had included an explicit note "Refit requires explicit params for every URL template placeholder," this would have been caught at plan time, not test time.
- **Phase 10 initially missed the IVideoApi bug scope.** The bug fix was discovered during integration test execution and required modifying 2 domain service files that weren't in the original plan scope. Phase plans should include a "Refit URL template validation" task to catch this class of issue upfront.

### Patterns Established

- **Refit URL template rule:** Every `{param}` in a URL template MUST match an explicit method parameter by name. WebApiClientCore resolved from body DTOs implicitly — Refit does not. This is a migration gotcha to document in project conventions.
- **Handler chain order is now documented:** `LogDelegatingHandler` → `BiliBiliCommonHeadersDelegatingHandler` → `IntervalDelegatingHandler` → Polly → `WridEncryptionDelegatingHandler`. Log outermost for complete visibility.
- **Refit 8.0.0 is the correct version for .NET 8** — Refit 10.x pulls in .NET 9 HttpClient deps. Pinned in `Directory.Packages.props`.

### Key Lessons

- **Add a "URL template param audit" task to any future Refit migration plan.** Run `grep -n "{[a-zA-Z]" on all interface files and verify every template placeholder has a matching explicit method param. This would have caught the IVideoApi bug at plan time.
- **ILiveApi `[RawFormContent]` has no direct Refit equivalent** — use `[Body] string` + method-level `[Headers("Content-Type: application/x-www-form-urlencoded")]`. Document this pattern for future reference.
- **Scrutor assembly anchor must be updated after deleting the anchor type.** `FromAssemblyOf<IBiliBiliApi>()` broke when `IBiliBiliApi.cs` was deleted. Switch to a stable, non-deletable type like `BiliBiliCommonHeadersDelegatingHandler`.

### Cost Observations

- Duration: 1 day (single session, 2026-05-04)
- Sessions: 1 (plus context handoff from previous session)
- Commits: 13 across 3 phases
- Net code: −154 lines (more deleted than added — a good sign for a cleanup migration)

---

## v4.0.0.2 — AppService Refactor Continuation (2026-05-04)

### What Was Built

Centralized `SetCookiesAsync` and `SaveCookieAsync` into `BaseMultiAccountsAppService` as `protected virtual` methods. Migrated all 11 in-scope AppServices (6 Group A with existing private copies, 5 Group B missing the call entirely). Added `TaskFlowDiagnosticScope` to all 11.

### What Worked

- **Group A / Group B split was the right decomposition.** Separating services that had wrong implementations (Group A) from services that had no implementation at all (Group B) made the migration ordered and safe. No service was left in an ambiguous state.
- **Existing test harnesses caught nothing new.** ArchitectureTests and IntegrationTests stayed green throughout — which means the structural refactor was genuinely behavior-preserving. The test investment from v4.0.0.1 paid off immediately.
- **UAT was fast because the change was mechanical.** 7 checks, all structural — no UI, no API behavior changes. Verification took minutes.

### What Was Inefficient

- **VERIFICATION.md still not written.** Despite a lesson from v4.0.0.1, no VERIFICATION.md was generated during execute-phase. The pattern hasn't been embedded in the workflow yet.
- **Milestone audit required manual grep work.** Without VERIFICATION.md, the audit step had to perform fresh grep verification rather than reading already-captured evidence. Extra work that compounds over phases.

### Patterns Established

- **`protected virtual` on base class for shared behavior.** The pattern of putting shared cookie logic on the base class as overridable methods is now established. Future AppService additions should follow this without debate.
- **Group-based migration.** When many services need the same change, classifying them by their current state (has/missing/wrong) and handling each group in its own plan is faster and clearer than treating all services uniformly.

### Key Lessons

- **Write VERIFICATION.md in the plan itself.** Add it as an explicit task in the final plan of every phase — not as an afterthought. The lesson from v4.0.0.1 was not followed; make it structural.
- **Scope decisions (out-of-scope items) should be in PLAN.md decisions.** `LoginTaskAppService` being out of scope was correct but required re-explanation in the audit. If D-01 had explicitly said "LoginTaskAppService excluded," the audit would have been instantaneous.

---

## v4.0.0.1 — Brownfield Refactor (2026-04-20 → 2026-05-03)

### What Went Well

- **Phase sequencing worked.** Starting with executable guardrails (ArchUnitNET) before touching any behavior gave confidence that later refactors wouldn't silently break layer boundaries. The "guardrails first" principle held throughout.
- **Characterization tests as safety net.** Freezing Login and DailyTask behavior in Phase 2 before editing internals in Phases 3–4 meant refactors could be verified rather than manually inspected. No observable behavior regressions after either slice.
- **Thin-shell Quartz pattern is clean.** Reducing 12 job classes to primary-constructor + static JobKey + expression-body `DoExecuteAsync` produced genuinely easier-to-read code with no orchestration logic buried in the scheduler.
- **BiliException hierarchy improved diagnostic clarity immediately.** The Business/Integration/Validation split made throw sites self-documenting. 14 conversion sites identified and fixed in one phase sweep.
- **Phase sizing was appropriate.** Each phase delivered a standalone, verifiable improvement. No phase felt too large to review or too small to justify.

### What Could Be Better

- **VERIFICATION.md was never written for any phase.** The GSD workflow expects a VERIFICATION.md after execution, but none was generated. This caused process-level audit failures even though all implementation goals were met. In future milestones, explicitly include writing VERIFICATION.md in phase execution plans.
- **REQUIREMENTS.md traceability was not updated live.** The traceability table stayed as `[ ] Pending` throughout all 6 phases. Requirements were only archived retroactively. Should checkpoint requirement status at each phase transition.
- **ARCH-04 notification boundary was deferred ambiguously.** The deferred scope (notification boundary) was not clearly flagged as deferred at the time — it was carried in the phase description as a third adapter boundary alongside EF and HTTP, which were both delivered. Deferred items should be explicit in PLAN.md decisions rather than inferred at audit time.
- **One pre-existing test failure left undiagnosed.** `Daily_task_multi_account_wrapper_continues_after_account_failure` was confirmed pre-existing before Phase 5 but not investigated. Root cause should be captured in a follow-up task to avoid confusion in v4.0.0.2.

### Surprises

- The ArchUnitNET rules required two iterations — initial rules were too broad and produced false positives for cross-assembly reflection patterns. The split into primary rules + allowlist refinement was more work than expected.
- `BiliResiliencePolicies` split (ReadOnlyPolicy / MutatingPolicy) surfaced 4 side-effecting HTTP clients that were silently using read-optimized retry behavior. Identifying and reclassifying them was unexpected but valuable.
- Phase 5 (Quartz shell cleanup) was estimated at 2 plans but only needed 1 wave of execution — the pattern was uniform enough that the second plan (stripping redundant logger fields) was faster than anticipated.

### Patterns to Carry Forward

- **Guardrails before behavior changes.** Always establish architectural enforcement before touching execution paths.
- **Freeze before refactor.** Characterization tests written before slice refactors are worth the upfront cost.
- **Deferred items must be named.** Any scope item not delivered should have an explicit deferred decision record in PLAN.md, not just be absent from SUMMARY.md.
- **Write VERIFICATION.md per phase.** Even a brief checklist confirming each success criterion is TRUE should be created immediately after plan execution.
- **Track requirements live.** Update traceability at each phase transition — don't rely on retroactive archaeology.

---

## v4.0.0.7 — Bili Account Management (2026-05-06 → 2026-05-07)

### What Was Built

Web-based "Bili Account" page with full CRUD (add via cookie paste or QR scan, edit, delete, reorder) backed by SQLite as primary config source. `cookies.json` retained as lower-priority fallback for Console-host compatibility. QR login uses QRCoder `PngByteQRCode` to render QR as base64 PNG displayed in MudDialog with state machine (Generating → Scanning → Success/Failed/Expired). 3 phases (17–19), 4 plans, 36 files changed (+2,965/−28).

### What Worked

- **v4.0.0.6 workflow seam pattern was the direct enabler.** `IBiliAccountPageWorkflow` followed the exact same pattern as the 5 seams from v4.0.0.6 — DI as Scoped, page workflow method routing, MudDialog invocations. No architecture decisions needed; just pattern replication.
- **Keeping cookies.json as fallback was the right call.** Early planning considered removing it entirely. Keeping it with lower priority (loaded before `AddSqlite` so SQLite wins for overlapping keys) means zero disruption for Console-host users.
- **QR login polling with state machine in MudDialog was clean.** The Generating→Scanning→Success/Failed/Expired state machine mapped naturally to Blazor's re-render model. StateTimer-based polling at 5s intervals with 10-attempt limit handled all edge cases.
- **Reordering with atomic batch swap prevented partial failures.** `BatchSet` with full key swap in a single `SqliteConfigurationProvider` call means no intermediate inconsistent state.
- **Architecture and component tests kept pace.** 5/5 ArchUnit, 28/28 bUnit, 7/7 integration tests all passing at milestone completion. Only the pre-existing Phase 5 characterization failure remains.

### What Was Inefficient

- **PLAN.md RESEARCH section was missing for Phase 19.** QR login research was done inline during execution rather than upfront. If RESEARCH.md had covered QRCoder PNG generation and the Bilibili passport QR API, Phase 19 planning would have been faster.
- **Account reorder UX is functional but primitive.** Up/down buttons work but drag-and-drop would be better. This is acceptable for v1 but should be noted.
- **No UAT.md or VERIFICATION.md for any phase.** Same gap as all prior milestones — inline verification via build + test suite, but no formal capture. The gsd-sdk unavailability makes this harder to enforce.

### Patterns Established

- **Workflow seam pattern is now canonical.** After v4.0.0.6 (5 seams) and v4.0.0.7 (1 seam), the pattern is: `I{Page}PageWorkflow` interface → `ServiceCollectionExtension.AddScoped` registration → Blazor page `@inject` → MudDialog invocation from workflow method. No debate needed for future pages.
- **QR-based auth flows go in DomainService layer.** `ILoginDomainService` already handles terminal QR login; web QR login extends it with `QrLoginGenerateResult` and `QrLoginCheckResult` DTOs. Shared API calls, different rendering targets.
- **SqliteConfigurationProvider as single source of truth for settings.** Web page mutations write to SQLite bili_appsettings, then reload `IConfigurationRoot`. All consumers (including AppService cookie reading) see updates immediately.

### Key Lessons

- **Write VERIFICATION.md in execute-phase plans.** This is now the fourth milestone where VERIFICATION.md was skipped. Make it a mandatory plan task.
- **Research QR/auth APIs before planning, not during execution.** QR login involved Refit API calls (`QrLogin`, `QrLoginConfirm`) that were already in the codebase but only discovered during implementation.
- **The `Login_flow_preserves_current_step_order_and_emits_diagnostics` failure has persisted since Phase 5.** It's now 14 phases old. Should be investigated or removed as a characterization test if it's testing behavior that was intentionally changed.

### Cost Observations

- Duration: 2 days (2026-05-06 → 2026-05-07)
- Sessions: 2
- Commits: 18 across 3 phases + audit
- Net code: +2,965/−28 (new feature, net growth expected)
- All requirement checkboxes verified as ✓ in REQUIREMENTS.md before archival
