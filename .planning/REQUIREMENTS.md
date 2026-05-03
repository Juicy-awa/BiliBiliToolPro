# Requirements: BiliBiliToolPro Refactor And Optimization

**Defined:** 2026-04-27
**Core Value:** Make the existing codebase safe to change: clear boundaries, lower coupling, and testable critical flows.

## v1 Requirements

### Architecture

- [ ] **ARCH-01**: Maintainer can enforce explicit dependency direction between `Agent`, `Application`, `DomainService`, `Infrastructure`, and `Web` layers.
- [ ] **ARCH-02**: Maintainer can keep `Web` and `Console` hosts thin so startup code no longer owns business orchestration decisions.
- [ ] **ARCH-03**: Maintainer can compose core modules through module-level service registration entry points instead of broad direct wiring in host `Program` files.
- [ ] **ARCH-04**: Maintainer can evolve EF, HTTP Agent, and notification integrations behind clearer adapter or port boundaries.

### Testing

- [ ] **TEST-01**: Maintainer can freeze the current observable behavior of critical `Login` and `DailyTask` flows with characterization tests before refactoring them.
- [ ] **TEST-02**: Maintainer can validate startup, configuration binding, EF, HTTP integration, and scheduling through host-level integration tests.
- [ ] **TEST-03**: Maintainer can detect new cross-layer dependency violations through automated architecture tests.

### Refactor Flows

- [ ] **FLOW-01**: Maintainer can refactor the `Login` flow behind a clearer application boundary without changing observable behavior.
- [ ] **FLOW-02**: Maintainer can refactor the `DailyTask` flow behind a clearer and testable application flow without changing observable behavior.
- [ ] **FLOW-03**: Maintainer can reduce Quartz job classes to thin scheduling shells that delegate to application use cases.
- [ ] **FLOW-04**: Maintainer can access Bilibili HTTP integrations through a more consistent Agent boundary with explicit policies and handlers.

### Quality

- [ ] **QUAL-01**: Maintainer can distinguish expected failures, integration failures, and unexpected crashes through a clearer exception model.
- [ ] **QUAL-02**: Maintainer can compare and diagnose old versus refactored critical paths using added logs and diagnostic signals.

## v2 Requirements

### Testing Expansion

- **TEST-04**: Maintainer can verify key Web or Blazor components with dedicated component tests.
- **TEST-05**: Maintainer can enforce focused coverage thresholds for critical modules in CI.

### Additional Refactor Scope

- **FLOW-05**: Maintainer can unify `Console` and `Web` configuration and startup composition paths where the current behavior meaningfully overlaps.

### Quality Expansion

- **QUAL-03**: Maintainer can remove default credential risks and similar obvious safety issues from bootstrap flows.
- **QUAL-04**: Maintainer can reduce repository noise from generated outputs so searches and reviews focus on source of truth files.

## Out of Scope

| Feature | Reason |
|---------|--------|
| Full product rewrite | Conflicts with the incremental, low-risk brownfield strategy |
| Framework swap as the primary effort | Does not directly solve coupling, boundaries, or testability |
| UI redesign as a primary milestone | This initiative is centered on architecture, code quality, and safety of change |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| ARCH-01 | Phase 1 - Boundary Guardrails | Pending |
| ARCH-02 | Phase 2 - Host Safety Nets | Pending |
| ARCH-03 | Phase 1 - Boundary Guardrails | Pending |
| ARCH-04 | Phase 6 - Integration Boundary And Failure Model | Pending |
| TEST-01 | Phase 2 - Host Safety Nets | Pending |
| TEST-02 | Phase 2 - Host Safety Nets | Pending |
| TEST-03 | Phase 1 - Boundary Guardrails | Pending |
| FLOW-01 | Phase 3 - Login Refactor Slice | Pending |
| FLOW-02 | Phase 4 - DailyTask Refactor Slice | Pending |
| FLOW-03 | Phase 5 - Scheduler Shell Cleanup | Pending |
| FLOW-04 | Phase 6 - Integration Boundary And Failure Model | Pending |
| QUAL-01 | Phase 6 - Integration Boundary And Failure Model | Pending |
| QUAL-02 | Phase 2 - Host Safety Nets | Pending |

**Coverage:**
- v1 requirements: 13 total
- Mapped to phases: 13
- Unmapped: 0

---
*Requirements defined: 2026-04-27*
*Last updated: 2026-04-28 after roadmap creation*