# Roadmap: BiliBiliToolPro Brownfield Refactor

## Milestones

- ✅ **v4.0.0.6 Web Layer Boundary Cleanup** — Phases 13–16 (shipped 2026-05-05) — [archive](milestones/v4.0.0.6-ROADMAP.md)
- ✅ **v4.0.0.1 Brownfield Refactor** — Phases 1–6 (shipped 2026-05-03) — [archive](milestones/v4.0.0.1-ROADMAP.md)
- ✅ **v4.0.0.2 AppService Refactor Continuation** — Phase 7 (shipped 2026-05-04) — [archive](milestones/v4.0.0.2-ROADMAP.md)
- ✅ **v4.0.0.3 Refit Migration** — Phases 8–10 (shipped 2026-05-04) — [archive](milestones/v4.0.0.3-ROADMAP.md)
- ✅ **v4.0.0.4 Agent Interface Consolidation** — Phase 11 (shipped 2026-05-04) — [archive](milestones/v4.0.0.4-ROADMAP.md)
- ✅ **v4.0.0.5 Agent DTO Reorganization** — Phase 12 (shipped 2026-05-04) — [archive](milestones/v4.0.0.5-ROADMAP.md)
- 🔲 **v4.0.0.7 Bili Account Management** — Phases 17–19 (active)

## Phases

### Phase 17: Account Storage Foundation

**Goal:** Establish the Bili Account page with account list view backed by SQLite, keeping `cookies.json` as a lower-priority fallback source so existing users are not broken.

**Requirements:** ACCT-07, ACCT-01

**Success criteria:**
1. Web host loads `config/cookies.json` as a fallback source (before SQLite) — existing cookies.json entries still work, but SQLite takes precedence for overlapping keys
2. SQLite `bili_appsettings` remains the highest-priority config source
2. "Bili Account" menu item appears in NavMenu (top-level, not under Configurations)
3. Account list page shows all configured accounts with UserId and full cookie string
4. `IBiliAccountPageWorkflow` seam follows v4.0.0.6 pattern
5. Build 0 errors | existing tests still pass

Plans:
- [x] 17-01-PLAN.md — Account storage foundation and list view

### Phase 18: Account CRUD Operations

**Goal:** Enable adding, editing, deleting, and reordering Bili accounts through the Web UI.

**Requirements:** ACCT-03, ACCT-04, ACCT-05, ACCT-06

**Success criteria:**
1. Maintainer can add a new account by pasting a cookie string
2. Maintainer can edit an existing account's cookie string
3. Maintainer can delete an account (with confirmation)
4. Maintainer can reorder accounts (up/down buttons swap `BiliBiliCookies__N` keys)
5. All mutations write to SQLite and reload `IConfigurationRoot`
6. Build 0 errors | existing tests still pass

**Plans:** 2 plans

Plans:
- [x] 18-01-PLAN.md — Account add/edit/delete with config reload (ACCT-03, ACCT-04, ACCT-05)
- [x] 18-02-PLAN.md — Account reorder with atomic key swap (ACCT-06)

### Phase 19: QR Code Login

**Goal:** Enable QR code login directly in the Web browser so maintainers can add accounts without manually copying cookie strings.

**Requirements:** ACCT-02

**Success criteria:**
1. Maintainer can click "Login with QR" to generate and display a Bilibili QR code in the browser
2. QR code is rendered as a base64 PNG image (not terminal output)
3. Page polls for scan result; on success, cookie is extracted and saved to SQLite
4. Login timeout and error states are handled gracefully
5. Build 0 errors | existing tests still pass

**Plans:** 1 plan

Plans:
- [x] 19-01-PLAN.md — QR code login in Web browser

<details>
<summary>✅ v4.0.0.6 Web Layer Boundary Cleanup (Phases 13–16) — SHIPPED 2026-05-05</summary>

- [x] **Phase 13: Web Boundary Foundation** — 2/2 plans complete — 2026-05-05
- [x] **Phase 14: Auth And Admin UI Boundary Cleanup** — 2/2 plans complete — 2026-05-05
- [x] **Phase 15: Scheduler UI Boundary Cleanup** — 3/3 plans complete — 2026-05-05
- [x] **Phase 16: Web Composition And Regression Verification** — 2/2 plans complete — 2026-05-05

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

**v4.0.0.7 Bili Account Management** — 3 phases, 4 plans, 7 requirements

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 17. Account Storage Foundation | v4.0.0.7 | 1/1 | Complete | 2026-05-06 |
| 18. Account CRUD Operations | v4.0.0.7 | 2/2 | Complete | 2026-05-07 |
| 19. QR Code Login | v4.0.0.7 | 0/1 | Pending | — |
| 13. Web Boundary Foundation | v4.0.0.6 | 2/2 | Complete | 2026-05-05 |
| 14. Auth And Admin UI Boundary Cleanup | v4.0.0.6 | 2/2 | Complete | 2026-05-05 |
| 15. Scheduler UI Boundary Cleanup | v4.0.0.6 | 3/3 | Complete | 2026-05-05 |
| 16. Web Composition And Regression Verification | v4.0.0.6 | 2/2 | Complete | 2026-05-05 |
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
