# Phase 12: Agent DTO Reorganization - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-05-04
**Phase:** 12-agent-dto-reorganization
**Areas discussed:** Move strategy, interface-to-directory mapping, Nav DTO grouping, consumer repair rule

---

## Move Strategy

| Option | Description | Selected |
|--------|-------------|----------|
| Preserve move history | Treat the change as a real file move/rename and preserve history with a move strategy equivalent to `git mv` | ✓ |
| Final state only | Only care about destination files existing; do not require move history | |

**User's choice:** Preserve move history.
**Notes:** This phase is a structural reorganization, not a rewrite, so history should remain traceable.

---

## Interface-To-Directory Mapping

| Option | Description | Selected |
|--------|-------------|----------|
| Region-first | Use a top-level folder derived directly from `IApiApi` internal regions, such as bare `Dtos/UpInfo/` | |
| Interface-first with fully locked subtree | Use interface-level roots (`Api`, `AccountApi`, `Nav`) and lock the full `IApiApi` subtree immediately | |
| Interface-first, subtree refined later | Use interface-level roots first, and let later planning refine `IApiApi` subfolders such as `ApiApi/UpInfo` | ✓ |

**User's choice:** Interface-first, with `IApiApi` subfolder structure refined later.
**Notes:** The final delivered naming rule is interface name minus leading `I`: `AccountApi -> IAccountApi`, `NavApi -> INavApi`, `ApiApi -> IApiApi`, with nested grouping allowed under `ApiApi`.

---

## Nav DTO Grouping

| Option | Description | Selected |
|--------|-------------|----------|
| File-only move | Only lock `UserInfo.cs` location; companion types move only because they are in the same file today | |
| Nav DTO group | Treat `UserInfo`, `WbiImg`, `LevelInfo`, and `Wallet` as one Nav DTO group that should remain under `Dtos/NavApi/` even if split later | ✓ |

**User's choice:** Nav DTO group.
**Notes:** The grouping should remain stable even if these types are split into separate files in a later change.

---

## Consumer Repair Rule

| Option | Description | Selected |
|--------|-------------|----------|
| Listed files only | Only change the consumers already enumerated in the plan files | |
| Compiler-and-test driven | Allow additional real consumer fixes when build or tests expose references outside the original plan list | ✓ |

**User's choice:** Compiler-and-test driven.
**Notes:** The plan file list is a starting point, not a cap, for namespace migration work.

---

## the agent's Discretion

- If a namespace/type collision remains under the interface-first structure, resolve it without renaming DTO classes unless a later phase explicitly expands scope.

## Deferred Ideas

None.