# BiliBiliToolPro Refactor And Optimization

## What This Is

This project is a brownfield refactor of the existing BiliBiliToolPro codebase. The goal is not to replace the product, but to make the current system easier to change by clarifying layer boundaries, reducing coupling between modules, and adding enough test coverage to support safe incremental improvements.

The product being preserved is an automated Bilibili task execution system with Console, Web, scheduling, API-integration, and deployment surfaces. The work now is to restructure that existing system so maintainers can evolve it with lower regression risk.

## Core Value

Make the existing codebase safe to change: clear boundaries, lower coupling, and testable critical flows.

## Requirements

### Validated

- ✓ The system can execute Bilibili automation tasks through existing Console and Web hosts — existing
- ✓ The system already integrates with Bilibili APIs, scheduling, persistence, and multiple deployment targets — existing
- ✓ The system already supports operational workflows such as login, daily tasks, manga/live tasks, notifications, and multi-environment deployment — existing

### Active

- [ ] Clarify architectural boundaries between Agent, Application, DomainService, Infrastructure, and Web layers
- [ ] Reduce cross-module coupling so changes in one area do not cascade across unrelated areas
- [ ] Introduce test coverage around critical execution paths so refactors can be validated safely
- [ ] Make the refactor incremental and phase-based rather than a one-shot rewrite
- [ ] Establish cleaner error-handling and service-boundary patterns in core execution flows

### Out of Scope

- Rewriting the product from scratch — the goal is to improve the existing system, not replace it
- Large feature expansion unrelated to maintainability — this work is about structural improvement first
- UI redesign as a primary goal — web cleanup may happen where needed, but visual redesign is not the main objective

## Context

- The repository is a multi-project .NET 8 solution centered on `Ray.BiliBiliTool.sln`
- Current executable surfaces include `src\Ray.BiliBiliTool.Console`, `src\Ray.BiliBiliTool.Web`, and `src\Ray.BiliBiliTool.Web.Client`
- Integration code is concentrated in `src\Ray.BiliBiliTool.Agent`, while scheduling is composed through Quartz and BlazingQuartz in the web host
- The current codebase map shows several refactor pressure points: broad generic exception usage, dense startup composition, seeded default admin credentials, thin tests, and generated artifacts mixed into the workspace
- The current maintainer pain is clear: changes ripple too widely, layer boundaries are unclear, and the test safety net is too weak to support confident refactoring

## Constraints

- **Brownfield**: Existing behavior must be preserved while restructuring — this is a live codebase with validated capabilities
- **Incremental Delivery**: Refactor work must be phaseable and low-risk — a big-bang rewrite would increase regression risk
- **Compatibility**: Existing Console/Web/task workflows should keep working during transition — operational continuity matters
- **Testing**: New boundaries should be introduced alongside verifiable tests — otherwise refactors remain unsafe
- **Maintainability**: Refactor decisions should favor simpler dependency direction and clearer ownership — reducing future change cost is the primary purpose

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Treat this as a brownfield refactor project, not a new product | The user wants to improve the current system rather than replace it | — Pending |
| Prioritize architecture boundaries, code quality, and testability first | These are the main pain points blocking safe changes today | — Pending |
| Use gradual, phase-based refactoring instead of a rewrite | The user explicitly wants low-risk incremental change | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd-transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd-complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-04-27 after initialization*