# Retrospectives

## v4.0.0.1 — Brownfield Refactor (2026-04-20 → 2026-05-03)

### What Went Well

- **Phase sequencing worked.** Starting with executable guardrails (ArchUnitNET) before touching any behavior gave confidence that later refactors wouldn't silently break layer boundaries. The "guardrails first" principle held throughout.
- **Characterization tests as safety net.** Freezing Login and DailyTask behavior in Phase 2 before editing internals in Phases 3–4 meant refactors could be verified rather than manually inspected. No observable behavior regressions after either slice.
- **Thin-shell Quartz pattern is clean.** Reducing 12 job classes to primary-constructor + static JobKey + expression-body `DoExecuteAsync` produced genuinely easier-to-read code with no orchestration logic buried in the scheduler.
- **BiliException hierarchy improved diagnostic clarity immediately.** The Business/Integration/Validation split made throw sites self-documenting. 14 conversion sites identified and fixed in one phase sweep.
- **Phase sizing was appropriate.** Each phase delivered a standalone, verifiable improvement. No phase felt too large to review or too small to justify.

### What Could Be Better

- **VERIFICATION.md was never written for any phase.** The GSD workflow expects a VERIFICATION.md after execution, but none was generated. This caused process-level audit failures even though all implementation goals were met. In future milestones, explicitly include writing VERIFICATION.md in phase execution plans.
- **REQUIREMENTS.md traceability was not updated live.** The traceability table stayed as `[ ] Pending` throughout all 6 phases. Requirements were only archived retroactively. Should checkpoint requirement status at each phase transition.
- **ARCH-04 notification boundary was deferred ambiguously.** The deferred scope (notification boundary) was not clearly flagged as deferred at the time — it was carried in the phase description as a third adapter boundary alongside EF and HTTP, which were both delivered. Deferred items should be explicit in PLAN.md decisions rather than inferred at audit time.
- **One pre-existing test failure left undiagnosed.** `Daily_task_multi_account_wrapper_continues_after_account_failure` was confirmed pre-existing before Phase 5 but not investigated. Root cause should be captured in a follow-up task to avoid confusion in v4.0.0.2.

### Surprises

- The ArchUnitNET rules required two iterations — initial rules were too broad and produced false positives for cross-assembly reflection patterns. The split into primary rules + allowlist refinement was more work than expected.
- `BiliResiliencePolicies` split (ReadOnlyPolicy / MutatingPolicy) surfaced 4 side-effecting HTTP clients that were silently using read-optimized retry behavior. Identifying and reclassifying them was unexpected but valuable.
- Phase 5 (Quartz shell cleanup) was estimated at 2 plans but only needed 1 wave of execution — the pattern was uniform enough that the second plan (stripping redundant logger fields) was faster than anticipated.

### Patterns to Carry Forward

- **Guardrails before behavior changes.** Always establish architectural enforcement before touching execution paths.
- **Freeze before refactor.** Characterization tests written before slice refactors are worth the upfront cost.
- **Deferred items must be named.** Any scope item not delivered should have an explicit deferred decision record in PLAN.md, not just be absent from SUMMARY.md.
- **Write VERIFICATION.md per phase.** Even a brief checklist confirming each success criterion is TRUE should be created immediately after plan execution.
- **Track requirements live.** Update traceability at each phase transition — don't rely on retroactive archaeology.
