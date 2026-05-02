# Phase 1: Boundary Guardrails - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-05-02
**Phase:** 1-Boundary Guardrails
**Areas discussed:** 目标分层模型, 依赖规则落地, 注册入口粒度, 宿主清理边界

---

## 目标分层模型

| Option | Description | Selected |
|--------|-------------|----------|
| Application 作为目标用例层，DomainService 逐步退场 | Long-term converge toward Application as the only use-case layer. | |
| 长期保留 DomainService 独立层 | Keep DomainService as a separate layer in the target model. | ✓ |
| Phase 1 先不定义目标关系 | Defer the target layer relationship. | |

**User's choice:** 长期保留 DomainService 独立层
**Notes:** `Application` should own orchestration and use-case entry points, while domain rules stay mainly in `Domain` and `DomainService`. The most important outcome is that hosts depend on `Application` entry points.

---

## 依赖规则落地

| Option | Description | Selected |
|--------|-------------|----------|
| 先以架构规则测试为主 | Start by enforcing boundaries with executable architecture rules/tests. | ✓ |
| 规则测试和项目引用同时收紧 | Tighten rules and project references together in Phase 1. | |
| 先靠约定和文档 | Start with conventions only. | |

**User's choice:** 先以架构规则测试为主
**Notes:** Minor project-reference cleanup is acceptable only when it is necessary to let the rules land. The first red-line category is host or job code reaching directly into lower layers. Success means new violations cannot enter mainline.

---

## 注册入口粒度

| Option | Description | Selected |
|--------|-------------|----------|
| 按业务模块/能力入口 | Prefer capability-oriented registration seams early. | |
| 按现有技术层入口继续整理 | Keep current technology-layer `Add*` seams and make them more orderly. | ✓ |
| 两层都保留 | Keep both technology-layer and capability-layer entry points. | |

**User's choice:** 按现有技术层入口继续整理
**Notes:** Hosts may still compose multiple technology-layer entry points directly. Web and Console should share the same ordering and grouping conventions. New entry points are allowed, but they must follow naming and placement rules.

---

## 宿主清理边界

| Option | Description | Selected |
|--------|-------------|----------|
| 过多业务编排和直连底层依赖 | Prioritize pulling business orchestration and lower-layer access out of hosts. | |
| 配置/日志/基础设施注册混杂 | Prioritize startup/wiring structure cleanup. | |
| 两者都先碰一点 | Make limited improvements to both concerns in Phase 1. | ✓ |

**User's choice:** 两者都先碰一点
**Notes:** The user does not want Phase 1 judged by visible host thinning. Current host shape can largely remain if host responsibilities become clearer and later enforceable by tests or rules. No hard list of logic-to-migrate is required in this phase.

---

## the agent's Discretion

- Research and planning may choose the concrete enforcement mechanism for dependency rules and registration-order validation.
- Research and planning may decide the exact dependency matrix between current layers, provided they preserve the user decisions above.

## Deferred Ideas

None.