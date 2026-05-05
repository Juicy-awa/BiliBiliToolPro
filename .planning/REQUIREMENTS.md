# Requirements: BiliBiliToolPro — v4.0.0.7 Bili Account Management

**Defined:** 2026-05-06
**Core Value:** Make the existing codebase safe to change: clear boundaries, lower coupling, and testable critical flows.

## v4.0.0.7 Requirements

### Account CRUD

- [ ] **ACCT-01**: Maintainer can view a list of all Bili accounts showing UserId and full cookie string in the Web UI
- [ ] **ACCT-02**: Maintainer can add a new Bili account by scanning a QR code displayed in the Web browser
- [ ] **ACCT-03**: Maintainer can add a new Bili account by pasting a raw cookie string into a text field
- [ ] **ACCT-04**: Maintainer can edit an existing account's cookie string
- [ ] **ACCT-05**: Maintainer can delete an existing account
- [ ] **ACCT-06**: Maintainer can reorder accounts to change execution order

### Storage

- [ ] **ACCT-07**: Remove `config/cookies.json` as a configuration source from Web host; SQLite `bili_appsettings` table remains the highest-priority config source for Bili cookies via the existing `AddSqlite` provider; cookie reading logic stays on `IConfiguration` (no change to `CookieStrFactory` or task execution flows)

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
| ACCT-01 | Phase 17 | Pending |
| ACCT-02 | Phase 19 | Pending |
| ACCT-03 | Phase 18 | Pending |
| ACCT-04 | Phase 18 | Pending |
| ACCT-05 | Phase 18 | Pending |
| ACCT-06 | Phase 18 | Pending |
| ACCT-07 | Phase 17 | Pending |

**Coverage:**
- v4.0.0.7 requirements: 7 total
- Mapped to phases: 7
- Unmapped: 0 ✓

---
*Requirements defined: 2026-05-06*
*Last updated: 2026-05-06 after initial definition*
