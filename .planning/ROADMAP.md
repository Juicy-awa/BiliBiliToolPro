# Roadmap: BiliBiliToolPro Brownfield Refactor

## Milestones

- ✅ **v4.0.0.1 Brownfield Refactor** — Phases 1–6 (shipped 2026-05-03) — [archive](milestones/v4.0.0.1-ROADMAP.md)
- ✅ **v4.0.0.2 AppService Refactor Continuation** — Phase 7 (shipped 2026-05-04) — [archive](milestones/v4.0.0.2-ROADMAP.md)
- 📋 **v4.0.0.3 Refit Migration** — Phases 8–10 (in planning)

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

### 📋 v4.0.0.3 — Refit Migration (Phases 8–10)

**Goal:** Replace WebApiClientCore with Refit as the HTTP client abstraction in the Agent layer — 18 interfaces, DI registration, and associated infrastructure.

#### Phase 8: Refit Foundation

**Goal:** Add Refit packages, create `BiliBiliCommonHeadersDelegatingHandler` to replace `AppendHeaderAttribute` behavior, update `IBiliBiliApi` to use Refit-compatible declarations.

**Requirements:** REFIT-04
**Plans:** 1 plan

**Success criteria:**
1. `Refit` and `Refit.HttpClientFactory` packages appear in `Directory.Packages.props` and `Agent.csproj` (alongside WebApiClientCore for now)
2. `BiliBiliCommonHeadersDelegatingHandler` exists and injects Accept, Accept-Language, Sec-Fetch-Dest, Sec-Fetch-Mode, Sec-Fetch-Site, Connection headers using AddIfNotExist semantics
3. `IBiliBiliApi` no longer references `AppendHeaderAttribute` or `[LogFilter]`
4. Build passes with 0 errors

#### Phase 9: Bilibili Interface Migration

**Goal:** Convert all 17 Bilibili HTTP client interface files from WebApiClientCore attributes to Refit attributes. Two parallel plans by host group.

**Requirements:** REFIT-01
**Plans:** 2 plans (parallel)

Plans:
- [ ] 09-01-PLAN.md — api.bilibili.com group: IUserInfoApi, IUpInfoApi, IDailyTaskApi, IRelationApi, IChargeApi, IVideoApi (+ IVideoWithoutCookieApi), IArticleApi, IAccountApi; delete IBiliBiliApi.cs
- [ ] 09-02-PLAN.md — other-host group: IVipMallApi, IPassportApi, ILiveTraceApi, IHomeApi, IMangaApi, ILiveApi, IVipBigPointApi, IMallApi

**Success criteria:**
1. All 17 interface files use `[Get]`, `[Post]`, `[Body]`, `[Query]`, `[Headers]` from `Refit` namespace
2. No `using WebApiClientCore.Attributes;` remains in any interface file
3. No `[LogFilter]` or `[AppendHeader]` remains in any interface file
4. `[FormContent]` → `[Body(BodySerializationMethod.UrlEncoded)]`, `[JsonContent]` → `[Body]`, `[PathQuery]` → `[Query]` fully replaced
5. Build passes with 0 errors

#### Phase 10: DI Migration & Cleanup

**Goal:** Convert DI registration to `AddRefitClient<T>`, migrate `IQingLongApi`, remove WebApiClientCore package, delete legacy attribute files.

**Requirements:** REFIT-02, REFIT-03, REFIT-05, REFIT-06
**Plans:** 1 plan

**Success criteria:**
1. `ServiceCollectionExtension.cs` uses `AddRefitClient<T>().ConfigureHttpClient(...)` for all 18 interfaces
2. Read-only vs mutating Polly policy distinction preserved across all clients
3. `IQingLongApi` attributes fully converted to Refit
4. `WebApiClientCore` removed from `Directory.Packages.props` and `Agent.csproj`
5. `AppendHeaderAttribute.cs`, `AppendHeaderType.cs`, `LogFilterAttribute.cs` deleted
6. Build: 0 errors | Architecture tests: 4/4 | Integration tests: 7/7

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
| 8. Refit Foundation | v4.0.0.3 | 0/1 | Planned | — |
| 9. Bilibili Interface Migration | v4.0.0.3 | 0/2 | Planned | — |
| 10. DI Migration & Cleanup | v4.0.0.3 | 0/1 | Planned | — |