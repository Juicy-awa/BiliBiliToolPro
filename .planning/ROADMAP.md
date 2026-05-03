# Roadmap: BiliBiliToolPro Brownfield Refactor

## Milestones

- ✅ **v4.0.0.1 Brownfield Refactor** — Phases 1–6 (shipped 2026-05-03) — [archive](milestones/v4.0.0.1-ROADMAP.md)
- 📋 **v4.0.0.2** — Phases 7+ (planned)

## Phases

<details>
<summary>✅ v4.0.0.1 Brownfield Refactor (Phases 1–6) — SHIPPED 2026-05-03</summary>

- [x] **Phase 1: Boundary Guardrails** — 2/2 plans complete — 2026-05-02
- [x] **Phase 2: Host Safety Nets** — 3/3 plans complete — 2026-05-03
- [x] **Phase 3: Login Refactor Slice** — 1/1 plan complete — 2026-05-03
- [x] **Phase 4: DailyTask Refactor Slice** — 1/1 plan complete — 2026-05-03
- [x] **Phase 5: Scheduler Shell Cleanup** — 2/2 plans complete — 2026-05-03
- [x] **Phase 6: Integration Boundary And Failure Model** — 4/4 plans complete — 2026-05-03

</details>

### 📋 v4.0.0.2 AppService Refactor Continuation (Phase 7)

**Goal:** Eliminate duplicated SetCookie/SaveCookie boilerplate across 6 AppServices.

| # | Phase | Goal | Requirements | Success Criteria |
|---|-------|------|--------------|-----------------|
| 7 | AppService Cookie Handling Extraction | Extract shared SetCookiesAsync + SaveCookieAsync into `BaseCookieAwareAppService`; migrate all 6 affected AppServices | FLOW-06, FLOW-07 | 4 |

#### Phase 7: AppService Cookie Handling Extraction

**Goal:** The duplicated `SetCookiesAsync` and `SaveCookieAsync` private methods that exist verbatim across AppServices are replaced by a single protected implementation in `BaseMultiAccountsAppService`, and all 11 applicable AppServices are migrated (per D-01: no new intermediate class).

**Requirements:** FLOW-06, FLOW-07

**Plans:** 4 plans

Plans:
- [ ] 07-01-PLAN.md — Upgrade BaseMultiAccountsAppService with ILoginDomainService + IConfiguration + two protected virtual cookie methods
- [ ] 07-02-PLAN.md — Migrate Group A (6 services with existing private copies): remove private methods, update base ctor call, add DiagnosticScope
- [ ] 07-03-PLAN.md — Migrate Group B (5 services missing SetCookiesAsync): add ctor deps, add SetCookiesAsync call, add DiagnosticScope
- [ ] 07-04-PLAN.md — Full solution build + all three test suites verification

**Status:** 🔲 Not started

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1. Boundary Guardrails | v4.0.0.1 | 2/2 | Complete | 2026-05-02 |
| 2. Host Safety Nets | v4.0.0.1 | 3/3 | Complete | 2026-05-03 |
| 3. Login Refactor Slice | v4.0.0.1 | 1/1 | Complete | 2026-05-03 |
| 4. DailyTask Refactor Slice | v4.0.0.1 | 1/1 | Complete | 2026-05-03 |
| 5. Scheduler Shell Cleanup | v4.0.0.1 | 2/2 | Complete | 2026-05-03 |
| 6. Integration Boundary And Failure Model | v4.0.0.1 | 4/4 | Complete | 2026-05-03 |