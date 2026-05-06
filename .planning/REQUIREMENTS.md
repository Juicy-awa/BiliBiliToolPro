# Requirements: BiliBiliToolPro — v4.0.0.7 Bili Account Management

**Defined:** 2026-05-06
**Core Value:** Make the existing codebase safe to change: clear boundaries, lower coupling, and testable critical flows.

## v4.0.0.7 Requirements

### Account CRUD

- [x] **ACCT-01**: Maintainer can view a list of all Bili accounts showing UserId and full cookie string in the Web UI
- [x] **ACCT-02**: Maintainer can add a new Bili account by scanning a QR code displayed in the Web browser
- [x] **ACCT-03**: Maintainer can add a new Bili account by pasting a raw cookie string into a text field
- [x] **ACCT-04**: Maintainer can edit an existing account's cookie string
- [x] **ACCT-05**: Maintainer can delete an existing account
- [x] **ACCT-06**: Maintainer can reorder accounts to change execution order

### Storage

- [x] **ACCT-07**: Keep `config/cookies.json` as a fallback configuration source in Web host (loaded before `AddSqlite`); SQLite `bili_appsettings` table remains the highest-priority config source for Bili cookies via the existing `AddSqlite` provider — when both sources define the same `BiliBiliCookies__N` key, SQLite wins; cookie reading logic stays on `IConfiguration` (no change to `CookieStrFactory` or task execution flows); existing users who only have cookies.json entries continue to work until they migrate to SQLite

## Deferred (future milestones)

- Setting a "primary" / default account
- QR code login in Console host
- Nickname/avatar display from Bilibili API
- Account-level enable/disable toggle

## Out of Scope

| Feature | Reason |
|---------|--------|
| Console project changes | Console keeps `cookies.json` — only Web moves to SQLite |
| Primary/default account selection | User explicitly deferred |
| Bilibili API profile enrichment | Nice-to-have, not core to account management |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| ACCT-01 | Phase 17 | Done |
| ACCT-02 | Phase 19 | Done |
| ACCT-03 | Phase 18 | Done |
| ACCT-04 | Phase 18 | Done |
| ACCT-05 | Phase 18 | Done |
| ACCT-06 | Phase 18 | Done |
| ACCT-07 | Phase 17 | Done |

**Coverage:**
- v4.0.0.7 requirements: 7 total
- Mapped to phases: 7
- Unmapped: 0 ✓

---
*Requirements defined: 2026-05-06*
*Last updated: 2026-05-07 after milestone audit (7/7 satisfied)*
