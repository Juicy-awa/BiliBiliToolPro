# Roadmap: BiliBiliToolPro Brownfield Refactor

## Overview

This roadmap phases the existing BiliBiliToolPro refactor so the codebase becomes safer to change without interrupting current Console, Web, scheduling, and integration behavior. The sequence starts with executable guardrails and validation paths, then moves through thin vertical refactor slices for Login and DailyTask before tightening scheduler delegation and external integration boundaries.

## Phases

**Phase Numbering:**
- Integer phases (1, 2, 3): Planned milestone work
- Decimal phases (2.1, 2.2): Urgent insertions (marked with INSERTED)

Decimal phases appear between their surrounding integers in numeric order.

- [x] **Phase 1: Boundary Guardrails** - Establish dependency rules and module registration seams before moving core behavior.
- [x] **Phase 2: Host Safety Nets** - Freeze critical behavior with tests and diagnostics while thinning host orchestration.
- [ ] **Phase 3: Login Refactor Slice** - Move Login behind a clearer application boundary without changing outcomes.
- [ ] **Phase 4: DailyTask Refactor Slice** - Move DailyTask behind a testable application flow without changing outcomes.
- [x] **Phase 5: Scheduler Shell Cleanup** - Reduce Quartz jobs to thin delegation shells over application use cases.
- [ ] **Phase 6: Integration Boundary And Failure Model** - Normalize adapter boundaries and typed failure handling around critical integrations.

## Phase Details

### Phase 1: Boundary Guardrails
**Goal**: Maintainers can change modules with explicit dependency direction and module-owned registration seams in place.
**Depends on**: Nothing (first phase)
**Requirements**: ARCH-01, ARCH-03, TEST-03
**Success Criteria** (what must be TRUE):
  1. Maintainers can run architecture checks that fail when Web, Console, or scheduling code reaches across forbidden layer boundaries.
  2. Web and Console composition code resolves core modules through explicit registration entry points instead of ad hoc cross-layer wiring.
  3. The allowed dependency direction between Agent, Application, DomainService, Infrastructure, and Web is visible in code and enforced automatically.
**Plans**: 2 plans

Plans:
- [x] 01-01-PLAN.md — Add executable architecture guardrails for dependency direction and host reach-through.
- [x] 01-02-PLAN.md — Standardize Web and Console registration seams and named startup-task boundaries.

### Phase 2: Host Safety Nets
**Goal**: Hosts stay thin and critical runtime behavior is frozen by tests and diagnostics before slice refactors begin.
**Depends on**: Phase 1
**Requirements**: ARCH-02, TEST-01, TEST-02, QUAL-02
**Success Criteria** (what must be TRUE):
  1. Characterization tests freeze the current observable behavior of Login and DailyTask flows before their internals move.
  2. Host-level integration tests boot the relevant startup paths and validate configuration binding, EF integration, HTTP integration seams, and scheduling for critical flows.
  3. Web and Console startup paths delegate business orchestration decisions to module or application services instead of owning them directly.
  4. Maintainers can compare baseline and refactored Login or DailyTask executions through added logs or diagnostic markers instead of manual tracing.
**Plans**: 3 plans

Plans:
- [x] 02-01-PLAN.md — Create dedicated characterization and host integration test harnesses.
- [x] 02-02-PLAN.md — Freeze Login and DailyTask behavior with characterization tests and explicit diagnostics.
- [x] 02-03-PLAN.md — Add Web and Console startup integration tests plus thin-host delegation assertions.

### Phase 3: Login Refactor Slice
**Goal**: Login behavior runs through a clearer application boundary without changing observable behavior for existing callers.
**Depends on**: Phase 2
**Requirements**: FLOW-01
**Success Criteria** (what must be TRUE):
  1. Existing Login entry points continue to produce the same observable success and failure outcomes after the refactor.
  2. Login orchestration is invoked through a dedicated application-facing boundary that can be exercised without booting unrelated task flows.
  3. The Login slice remains covered by the characterization and integration checks established earlier in the roadmap.
**Plans**: 1 plan

Plans:
- [x] 03-01-PLAN.md — Clarify LoginTaskAppService internal structure and validate against characterization tests

### Phase 4: DailyTask Refactor Slice
**Goal**: DailyTask execution is isolated behind a testable application flow while preserving current automation behavior.
**Depends on**: Phase 3
**Requirements**: FLOW-02
**Success Criteria** (what must be TRUE):
  1. Existing DailyTask triggers continue to run the same observable task workflow and result reporting after the refactor.
  2. DailyTask orchestration is available through a dedicated application flow that can be exercised with focused tests.
  3. Host and scheduler callers delegate DailyTask work through the same application entry point instead of duplicating orchestration logic.
**Plans**: 1 plan

Plans:
- [x] 04-01-PLAN.md — Clarify DailyTaskAppService internal structure and validate against characterization tests

### Phase 5: Scheduler Shell Cleanup
**Goal**: Scheduled work is triggered by thin Quartz shells that schedule and delegate instead of owning business orchestration.
**Depends on**: Phase 4
**Requirements**: FLOW-03
**Success Criteria** (what must be TRUE):
  1. Quartz job classes are thin enough that their purpose is limited to schedule context and delegation.
  2. Existing scheduled Login and DailyTask behavior still runs through current job identities and schedules while delegating to application use cases.
  3. Maintainers can change scheduling concerns without editing core orchestration logic, and can change orchestration logic without rewriting Quartz job shells.
**Plans**: 2 plans

Plans:
- [x] 05-01-PLAN.md — Enhance BaseJob with Logger property and started log; extract AddBiliJob registration helper
- [x] 05-02-PLAN.md — Strip redundant _logger fields and started-log calls from all 12 Quartz job classes

### Phase 6: Integration Boundary And Failure Model
**Goal**: External integrations and failures sit behind consistent adapters with diagnosable outcomes for critical flows.
**Depends on**: Phase 5
**Requirements**: ARCH-04, FLOW-04, QUAL-01
**Success Criteria** (what must be TRUE):
  1. Critical Login and DailyTask paths access Bilibili HTTP behavior through consistent Agent boundaries instead of scattered direct integration patterns.
  2. Maintainers can distinguish expected business failures, integration failures, and unexpected crashes in critical paths from typed failures and logs.
  3. EF, HTTP Agent, and notification-facing dependencies are reachable through clearer adapter boundaries so implementations can change without host or flow rewiring.
  4. Outbound API policies and handlers are explicit enough that resilience behavior can be verified at the boundary.
**Plans**: 4 plans

Plans:
- [ ] 06-01-PLAN.md — Create BiliException hierarchy in Domain + extract BiliResiliencePolicies with split policies and 30s timeout
- [ ] 06-02-PLAN.md — Introduce IExecutionLogRepository and IUserRepository; remove direct EF factory injection from LogsDialog and AuthService
- [ ] 06-03-PLAN.md — Replace all generic Exception throws in seven DomainService files with typed BiliBusinessException/BiliIntegrationException
- [ ] 06-04-PLAN.md — Convert BiliCookie.cs and CookieInfo.cs to BiliValidationException; add unit tests for typed exceptions and resilience constants

## Progress

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Boundary Guardrails | 2/2 | Complete | 2026-05-02 |
| 2. Host Safety Nets | 3/3 | Complete | 2026-05-03 |
| 3. Login Refactor Slice | 1/1 | Complete | 2026-05-03 |
| 4. DailyTask Refactor Slice | 1/1 | Complete | 2026-05-03 |
| 5. Scheduler Shell Cleanup | 0/2 | Not started | - |
| 6. Integration Boundary And Failure Model | 0/TBD | Not started | - |