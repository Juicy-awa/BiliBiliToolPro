# Roadmap: BiliBiliToolPro Brownfield Refactor

## Milestones

- ✅ **v4.0.0.1 Brownfield Refactor** — Phases 1–6 (shipped 2026-05-03) — [archive](milestones/v4.0.0.1-ROADMAP.md)
- ✅ **v4.0.0.2 AppService Refactor Continuation** — Phase 7 (shipped 2026-05-04) — [archive](milestones/v4.0.0.2-ROADMAP.md)
- ✅ **v4.0.0.3 Refit Migration** — Phases 8–10 (shipped 2026-05-04) — [archive](milestones/v4.0.0.3-ROADMAP.md)
- ✅ **v4.0.0.4 Agent Interface Consolidation** — Phase 11 (shipped 2026-05-04) — [archive](milestones/v4.0.0.4-ROADMAP.md)
- 🔄 **v4.0.0.5 Agent DTO Reorganization** — Phase 12 (in progress)

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

## 🔄 v4.0.0.5 Agent DTO Reorganization

### Phase 12: Agent DTO Reorganization

**Goal:** Rename and move DTO files so each folder maps 1:1 to its interface grouping — `Space/` → `UpInfo/`, `Coin/CoinBalance` → `AccountApi/`, root `UserInfo` → `Nav/`. Update all namespaces and consumer using directives. Build 0 errors, arch 4/4, integration 7/7.

**Requirements:** DTO-01, DTO-02, DTO-03, DTO-04, DTO-05, DTO-06

**Plans:**
- [ ] 12-01-PLAN.md — Move DTO files: Space→UpInfo, Coin/CoinBalance→AccountApi/, UserInfo→Nav/; update namespace declarations inside moved files
- [ ] 12-02-PLAN.md — Update all consumers: fix using directives in interfaces, DomainServices, AppServices, tests; verify build 0 errors + arch 4/4 + integration 7/7

**Success criteria:**
1. `ls Dtos/UpInfo/` contains GetSpaceInfoFullDto, GetSpaceInfoResponse, UpInfo — `Space/` folder deleted
2. `ls Dtos/AccountApi/` contains CoinBalance.cs — `Coin/CoinBalance.cs` deleted
3. `ls Dtos/Nav/` contains UserInfo.cs — root-level `UserInfo.cs` deleted
4. `dotnet build` → 0 errors, 0 warnings on namespace changes
5. Architecture tests 4/4 pass
6. Integration tests 7/7 pass

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
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
| 12. Agent DTO Reorganization | v4.0.0.5 | 0/2 | In Progress | — |