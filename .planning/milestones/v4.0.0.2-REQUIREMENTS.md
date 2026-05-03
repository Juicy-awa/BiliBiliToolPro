# Requirements: BiliBiliToolPro Brownfield Refactor

**Defined:** 2026-05-04
**Core Value:** Make the existing codebase safe to change: clear boundaries, lower coupling, and testable critical flows.

## v4.0.0.2 Requirements

AppService refactor continuation — eliminate duplicated cookie-handling boilerplate shared across 6 AppServices.

### Application Layer

- [x] **FLOW-06**: Maintainer can see shared SetCookie/SaveCookie behavior defined exactly once in a protected base class (`BaseCookieAwareAppService`), not copied across 6 AppServices
- [x] **FLOW-07**: Maintainer can verify the refactored AppService hierarchy produces the same observable behavior as before (all existing characterization and integration tests continue to pass)

## Deferred Requirements

Captured from v4.0.0.1 — not in scope for v4.0.0.2. Tracked for future milestones.

### Testing

- **TEST-04**: Maintainer can verify key Web or Blazor components with dedicated component tests
- **TEST-05**: Maintainer can enforce focused coverage thresholds for critical modules in CI

### Flows

- **FLOW-05**: Maintainer can unify Console and Web configuration and startup composition paths where behavior meaningfully overlaps

### Quality

- **QUAL-03**: Maintainer can remove default credential risks and similar obvious safety issues from bootstrap flows
- **QUAL-04**: Maintainer can reduce repository noise from generated outputs so searches and reviews focus on source of truth files

### Architecture

- **ARCH-04**: Notification adapter/port boundary (Serilog sink deferred from v4.0.0.1)

## Out of Scope

| Feature | Reason |
|---------|--------|
| Changing AppService task behavior | Pure structural refactor — observable behavior must be preserved |
| New Bilibili task features | This milestone is about internal structure only |
| Blazor component tests / CI coverage | Deferred to future milestone |
| Console/Web config unification | Deferred to future milestone |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| FLOW-06 | Phase 7 | ✅ Satisfied |
| FLOW-07 | Phase 7 | ✅ Satisfied |

**Coverage:**
- v4.0.0.2 requirements: 2 total
- Mapped to phases: 2
- Unmapped: 0 ✓

---
*Requirements defined: 2026-05-04*
*Last updated: 2026-05-04 — milestone v4.0.0.2 started*
