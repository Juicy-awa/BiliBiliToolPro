# Phase 4: DailyTask Refactor Slice - Context

**Gathered:** 2026-05-03
**Status:** Ready for planning

<domain>
## Phase Boundary

Move the **DailyTask automation flow** (triggered by DailyJob) behind a clearer application boundary that isolates orchestration from domain operations. The refactor preserves all observable behavior frozen by Phase 2 characterization tests while improving the testability and clarity of the DailyTask slice.

**In scope:**
- DailyJob → IDailyTaskAppService → DailyTaskAppService orchestration chain
- Multi-account wrapper behavior (BaseMultiAccountsAppService inheritance)
- Six-step task sequence: Cookie setup → Login → Get status → Watch/share → Add coins → Receive VIP privileges
- Integration with multiple domain services (Account, Video, Article, DonateCoin, VipPrivilege, Login)
- TaskInterceptor attributes and diagnostic scope markers

**Out of scope:**
- Other task app services (VipPrivilegeTask, MangaTask, Silver2CoinTask, etc.) — handled in later phases
- Changes to domain service implementations (domain layer remains unchanged)
- BaseMultiAccountsAppService base class refactoring — only DailyTaskAppService internals
- Quartz job scheduling logic cleanup (deferred to Phase 5)
- Web-based manual task triggering flows

</domain>

<decisions>
## Implementation Decisions

### Scope Boundary
- **D-01:** DailyTask refactor focuses exclusively on the **automation task daily workflow** triggered by `DailyJob` (scheduled execution). Manual web-triggered flows are out of scope for Phase 4.

### the agent's Discretion

The following areas are delegated to planning and implementation agents to decide based on codebase evidence:

- **D-02 (Contract stability):** Whether to keep the existing `IDailyTaskAppService` contract unchanged (following Phase 3's stable-boundary pattern) or to evolve it during this refactor. Decision should consider caller impact (DailyJob, potential Console commands) and consistency with the Login refactor approach.

- **D-03 (Documentation level):** The appropriate level of XML documentation to add to `DailyTaskAppService`. Phase 3 added comprehensive XML docs to `LoginTaskAppService` to establish a documentation pattern. Agent should determine whether to follow the same comprehensive approach or use a lighter style based on code complexity.

- **D-04 (Error handling preservation):** The level of behavioral preservation for multi-account error handling during refactor. The characterization tests from Phase 2 validate that execution continues after per-account failures. Agent should be guided by test coverage — if tests freeze specific error messages and logging patterns, preserve them exactly; if tests only validate outcome (continues-after-failure), functional equivalence is acceptable.

- **D-05 (Workflow organization):** How to organize the six-step workflow internally (SetCookies → Login → GetStatus → Watch/Share → Coins → VIP). Current structure uses private methods called from `DoTaskAccountAsync`. Agent should determine whether to keep private methods, extract to explicit testable steps, flatten to a single method, or use another approach based on clarity and testability needs.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Architecture & Boundaries
- `.planning/codebase/ARCHITECTURE.md` — Module dependency rules, layer boundaries, and host composition patterns from Phase 1
- `.planning/codebase/CONVENTIONS.md` — Coding patterns, naming conventions, service registration style, and established idioms
- `.planning/codebase/TESTING.md` — Testing approaches, xUnit + FluentAssertions patterns, characterization test guidelines

### Prior Phase Context
- `.planning/phases/01-boundary-guardrails/01-CONTEXT.md` — Locked decisions from Phase 1: dependency direction (D-01), thin hosts (D-02), module registration (D-03), TaskInterceptor preservation (D-11)
- `.planning/phases/03-login-refactor-slice/03-CONTEXT.md` — Locked decisions from Phase 3: stable contract boundary (D-02), internal granularity (D-03), compatibility preservation (D-04). DailyTask can follow similar patterns.

### Test Artifacts
- `test/Ray.BiliBiliTool.CharacterizationTests/DailyTaskCharacterizationTests.cs` — Frozen DailyTask behavior baseline established in Phase 2 Plan 02
- `test/Ray.BiliBiliTool.Host.IntegrationTests/WebStartupIntegrationTests.cs` — Host startup validation for DailyTask flow
- `test/Ray.BiliBiliTool.ArchitectureTests/` — Boundary guardrails that must continue passing

</canonical_refs>

<code_context>
## Existing Code Insights

### Current DailyTask Flow Chain

**Trigger:** `src/Ray.BiliBiliTool.Web/Jobs/DailyJob.cs`
- Quartz job that calls `IDailyTaskAppService.DoTaskAsync()`
- Thin delegation shell (Phase 1 established pattern)
- Static JobKey for scheduler identification

**Application Boundary:** `src/Ray.BiliBiliTool.Application.Contracts/IDailyTaskAppService.cs`
- Contract: `interface IDailyTaskAppService : IAppService`
- Current implementation inherits base `DoTaskAsync()` pattern from `BaseMultiAccountsAppService`

**Orchestration:** `src/Ray.BiliBiliTool.Application/DailyTaskAppService.cs`
- Inherits `BaseMultiAccountsAppService` for multi-account iteration and error resilience
- Six-step workflow in `DoTaskAccountAsync()`:
  1. `SetCookiesAsync()` — Validates and sets cookie context if incomplete (delegates to LoginDomainService)
  2. `Login()` — Authenticates and retrieves user info
  3. `GetDailyTaskStatus()` — Fetches current task completion status
  4. `WatchAndShareVideo()` — Completes watch/share requirements
  5. `AddCoins()` — Donates coins to videos or articles based on configuration
  6. `ReceiveVipPrivilege()` — Claims monthly VIP benefits
- Decorated with `[TaskInterceptor]` attributes for logging at each step
- Wrapped in `TaskFlowDiagnosticScope` (Phase 2 addition)
- Configuration-driven behavior (DailyTaskOptions: IsEnable, IsWatchVideo, IsShareVideo, IsDonateCoinForArticle, SaveCoinsWhenLv6)

**Domain Dependencies:** Six domain services
- `IAccountDomainService` — Login, user info, daily task status
- `IVideoDomainService` — Watch and share operations
- `IArticleDomainService` — Article coin donation
- `IDonateCoinDomainService` — Video coin donation
- `IVipPrivilegeDomainService` — VIP privilege claiming
- `ILoginDomainService` — Cookie setup and persistence

### Established Patterns

**Multi-account base class** (existing pattern)
- `BaseMultiAccountsAppService` provides cookie iteration via `CookieStrFactory`
- Continues execution even when one account fails (validated by characterization tests)
- Per-account try/catch with logging before moving to next account

**TaskInterceptor pattern** (Phase 1 D-11)
- Logging and telemetry via `[TaskInterceptor("label", TaskLevel)]` attributes
- Must be preserved during refactor
- Each step method has its own interceptor

**Diagnostic scopes** (Phase 2 D-03)
- `TaskFlowDiagnosticScope.ExecuteAsync()` wraps the main flow
- Enables before/after comparison of DailyTask behavior
- Emits FlowStart, FlowCompleted, and FlowFailed markers

**Configuration-driven branching** (existing pattern)
- `DailyTaskOptions.IsEnable` — Skip entire task if disabled
- `DailyTaskOptions.IsWatchVideo/IsShareVideo` — Conditional step execution
- `DailyTaskOptions.SaveCoinsWhenLv6` — Skip coin donation for max-level users
- `DailyTaskOptions.IsDonateCoinForArticle` — Prefer articles over videos for coins
- `PlatformType` — QingLong vs JSON file cookie persistence

**Cookie persistence** (shared with Login)
- Platform-aware: QingLong → environment variables, others → JSON file
- Reuses `LoginDomainService` methods

### Integration Points

- **Host → Application:** DailyJob delegates to IDailyTaskAppService
- **Application → Domain:** DailyTaskAppService orchestrates 6 domain services
- **Configuration:** Reads `DailyTaskOptions` and `PlatformType` from IConfiguration
- **Logging:** ILogger<DailyTaskAppService> with structured logging
- **Multi-account:** CookieStrFactory provides cookie enumeration

</code_context>

<specifics>
## Specific Expectations

- **Characterization tests must continue passing:** The DailyTask refactor cannot change observable behavior frozen by `DailyTaskCharacterizationTests.cs` in Phase 2. This includes:
  - Six-step sequence validation
  - Multi-account continue-after-failure behavior
  - Diagnostic marker emissions (FlowStart, FlowCompleted, FlowFailed)
  
- **Diagnostic markers remain in place:** TaskFlowDiagnosticScope integration must survive the refactor so maintainers can still trace DailyTask flow execution.

- **TaskInterceptor attributes preserved:** These are relied upon for operational telemetry (Phase 1 D-11 lock). Each step's interceptor provides observability.

- **Multi-account wrapper behavior intact:** BaseMultiAccountsAppService inheritance and its error-resilience guarantee (validated by characterization tests) must be preserved.

- **No domain service changes:** All six domain services (Account, Video, Article, DonateCoin, VipPrivilege, Login) and their implementations remain untouched; Phase 4 only refactors the application orchestration layer.

- **Configuration-driven behavior preserved:** All DailyTaskOptions flags must continue controlling workflow branching exactly as before.

</specifics>

<deferred>
## Deferred Ideas

- **Other task app services:** VipPrivilegeTaskAppService, MangaTaskAppService, Silver2CoinTaskAppService, LiveLotteryTaskAppService, LiveFansMedalTaskAppService, UnfollowBatchedTaskAppService — these also use similar orchestration patterns but are out of scope for Phase 4 (addressed in later phases if needed).

- **BaseMultiAccountsAppService refactoring:** The multi-account base class pattern itself is not being redesigned in Phase 4; only DailyTaskAppService's implementation of its abstract methods.

- **Quartz job shell cleanup:** Thinning DailyJob further or standardizing job delegation patterns is deferred to Phase 5.

- **Domain service boundaries:** Changes to domain service interfaces or implementations are deferred to Phase 6 (Integration Boundary And Failure Model).

- **Web manual task triggering:** Any web UI or API endpoints that manually trigger daily tasks are out of scope for Phase 4.

</deferred>

---

*Phase: 4-DailyTask Refactor Slice*
*Context gathered: 2026-05-03*
