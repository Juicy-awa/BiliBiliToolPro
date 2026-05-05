# Roadmap: BiliBiliToolPro Brownfield Refactor

## Milestones

- 🚧 **v4.0.0.6 Web Layer Boundary Cleanup** — Phases 13–16 (defined 2026-05-05) — active milestone
- ✅ **v4.0.0.1 Brownfield Refactor** — Phases 1–6 (shipped 2026-05-03) — [archive](milestones/v4.0.0.1-ROADMAP.md)
- ✅ **v4.0.0.2 AppService Refactor Continuation** — Phase 7 (shipped 2026-05-04) — [archive](milestones/v4.0.0.2-ROADMAP.md)
- ✅ **v4.0.0.3 Refit Migration** — Phases 8–10 (shipped 2026-05-04) — [archive](milestones/v4.0.0.3-ROADMAP.md)
- ✅ **v4.0.0.4 Agent Interface Consolidation** — Phase 11 (shipped 2026-05-04) — [archive](milestones/v4.0.0.4-ROADMAP.md)
- ✅ **v4.0.0.5 Agent DTO Reorganization** — Phase 12 (shipped 2026-05-04) — [archive](milestones/v4.0.0.5-ROADMAP.md)

## Phases

<details open>
<summary>🚧 v4.0.0.6 Web Layer Boundary Cleanup (Phases 13–16) — DEFINED 2026-05-05</summary>

- [x] **Phase 13: Web Boundary Foundation** — 2/2 plans complete — 2026-05-05
- [x] **Phase 14: Auth And Admin UI Boundary Cleanup** — 2/2 plans complete — 2026-05-05
- [x] **Phase 15: Scheduler UI Boundary Cleanup** — 3/3 plans complete — 2026-05-05
- [ ] **Phase 16: Web Composition And Regression Verification** — pending planning

</details>

<details>
<summary>✅ v4.0.0.1 Brownfield Refactor (Phases 1–6) — SHIPPED 2026-05-03</summary>

- [x] **Phase 1: Boundary Guardrails** — 2/2 plans complete — 2026-05-02
- [x] **Phase 2: Host Safety Nets** — 3/3 plans complete — 2026-05-03
- [x] **Phase 3: Login Refactor Slice** — 1/1 plan complete — 2026-05-03
- [x] **Phase 4: DailyTask Refactor Slice** — 1/1 plan complete — 2026-05-03
- [x] **Phase 5: Scheduler Shell Cleanup** — 2/2 plans complete — 2026-05-03
- [x] **Phase 6: Integration Boundary And Failure Model** — 4/4 plans complete — 2026-05-03

</details>

<details>
<summary>✅ v4.0.0.2 AppService Refactor Continuation (Phase 7) — SHIPPED 2026-05-04</summary>

- [x] **Phase 7: AppService Cookie Handling Extraction** — 4/4 plans complete — 2026-05-04

</details>

<details>
<summary>✅ v4.0.0.3 Refit Migration (Phases 8–10) — SHIPPED 2026-05-04</summary>

- [x] **Phase 8: Refit Foundation** — 1/1 plan complete — 2026-05-04
- [x] **Phase 9: Bilibili Interface Migration** — 2/2 plans complete — 2026-05-04
- [x] **Phase 10: DI Migration & Cleanup** — 1/1 plan complete — 2026-05-04

</details>

<details>
<summary>✅ v4.0.0.4 Agent Interface Consolidation (Phase 11) — SHIPPED 2026-05-04</summary>

- [x] **Phase 11: Agent Interface Consolidation** — 4/4 plans complete — 2026-05-04

</details>

<details>
<summary>✅ v4.0.0.5 Agent DTO Reorganization (Phase 12) — SHIPPED 2026-05-04</summary>

- [x] **Phase 12: Agent DTO Reorganization** — 2/2 plans complete — 2026-05-04

</details>

## Active Milestone

### v4.0.0.6 Web Layer Boundary Cleanup

**Goal:** Refactor the Web layer so UI rendering stays thin, component code-behind stops owning workflow orchestration for the targeted slices, and the resulting seams are safer to test and evolve.

**Requirements:** WEB-01, WEB-02, WEB-03, WEB-04, WEB-05, WEB-06
**Research:** `.planning/research/v4.0.0.6-web-layer-boundary-cleanup-RESEARCH.md`

### Phase 13: Web Boundary Foundation

**Goal:** Define and introduce the first Web-facing coordination seams for the targeted Web slices, add the minimal Web test harness, and establish the pattern used by later phases.

**Requirements:** WEB-01, WEB-05
**Plans:** 2 plans

Plans:
- [ ] 13-01-PLAN.md — Add the dedicated Web component-test harness and baseline Login/Admin component tests
- [ ] 13-02-PLAN.md — Extract the first Login page-state seam and define auth/admin page-workflow contracts for the next phase

**Success criteria:**
1. Targeted Web slices have explicit coordination seams owned by the Web layer.
2. A dedicated Web component or thin-slice test harness exists in source control.
3. The milestone has a concrete pattern for moving orchestration out of component code-behind.

### Phase 14: Auth And Admin UI Boundary Cleanup

**Goal:** Refactor the login and admin Web pages so validation, success-flow handling, and navigation orchestration are isolated behind the selected Web-facing seam while preserving current auth behavior.

**Requirements:** WEB-03
**Plans:** 2 plans

Plans:
- [x] 14-01-PLAN.md — Implement AdminPageWorkflow + DI registration + refactor Admin.razor.cs + Logout button markup
- [x] 14-02-PLAN.md — AdminPageWorkflow unit tests + updated AdminPageTests with FakeAdminPageWorkflow

**Success criteria:**
1. Login and admin pages use the selected Web-layer seam for their targeted workflow logic.
2. Existing auth behavior remains intact from the user perspective.
3. The refactored auth/admin slice has focused verification coverage.

### Phase 15: Scheduler UI Boundary Cleanup

**Goal:** Refactor the schedule pages and dialogs so repository-backed log polling, execution-history loading, and scheduler-event coordination are moved out of the component classes.

**Requirements:** WEB-02
**Plans:** 3 plans

Plans:
- [ ] 15-01-PLAN.md — Define ISchedulerPageWorkflow + implement SchedulerPageWorkflow + DI registration + Schedules.razor.cs refactor
- [ ] 15-02-PLAN.md — Define ILogsDialogWorkflow + IHistoryDialogWorkflow + implement + DI + LogsDialog + HistoryDialog refactors
- [ ] 15-03-PLAN.md — Workflow unit tests (3 classes) + bUnit component tests (3 classes) in Schedules/ sub-directory

**Success criteria:**
1. Scheduler dialogs and pages no longer own the targeted coordination logic directly.
2. Log and history behavior remains stable for the selected scheduler screens.
3. The refactored scheduler slice has focused verification coverage.

### Phase 16: Web Composition And Regression Verification

**Goal:** Finish the Web-layer cleanup by keeping startup and registration logic wiring-focused, then verify the milestone through build, architecture, host integration, and new Web tests.

**Requirements:** WEB-04, WEB-06
**Plans:** 2 plans

Plans:
- [ ] 16-01-PLAN.md — Add ArchUnit Web.Components guardrail + extend WebStartupIntegrationTests with 6 workflow seam assertions
- [ ] 16-02-PLAN.md — Full WEB-06 regression: build + arch tests + integration tests + component tests

**Success criteria:**
1. Web host wiring remains composition-focused for the refactored slices.
2. Milestone regression checks pass across build, architecture, host integration, and Web-focused tests.
3. The milestone can transition cleanly into phase planning starting with Phase 13.

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 13. Web Boundary Foundation | v4.0.0.6 | 0/2 | Planned | |
| 14. Auth And Admin UI Boundary Cleanup | v4.0.0.6 | 0/? | Planned | |
| 15. Scheduler UI Boundary Cleanup | v4.0.0.6 | 0/? | Planned | |
| 16. Web Composition And Regression Verification | v4.0.0.6 | 0/? | Planned | |
| 1. Boundary Guardrails | v4.0.0.1 | 2/2 | Complete | 2026-05-02 |
| 2. Host Safety Nets | v4.0.0.1 | 3/3 | Complete | 2026-05-03 |
| 3. Login Refactor Slice | v4.0.0.1 | 1/1 | Complete | 2026-05-03 |
| 4. DailyTask Refactor Slice | v4.0.0.1 | 1/1 | Complete | 2026-05-03 |
| 5. Scheduler Shell Cleanup | v4.0.0.1 | 2/2 | Complete | 2026-05-03 |
| 6. Integration Boundary And Failure Model | v4.0.0.1 | 4/4 | Complete | 2026-05-03 |
| 7. AppService Cookie Handling Extraction | v4.0.0.2 | 4/4 | Complete | 2026-05-04 |
| 8. Refit Foundation | v4.0.0.3 | 1/1 | Complete | 2026-05-04 |
| 9. Bilibili Interface Migration | v4.0.0.3 | 2/2 | Complete | 2026-05-04 |
| 10. DI Migration & Cleanup | v4.0.0.3 | 1/1 | Complete | 2026-05-04 |
| 11. Agent Interface Consolidation | v4.0.0.4 | 4/4 | Complete | 2026-05-04 |
| 12. Agent DTO Reorganization | v4.0.0.5 | 2/2 | Complete | 2026-05-04 |