# Requirements: v4.0.0.5 — Agent DTO Reorganization

## Milestone Goal

Reorganize Agent-layer DTO folders so the directory structure mirrors the interface grouping established in v4.0.0.4. After consolidating 8 api.bilibili.com interfaces into `IApiApi`, the DTO sub-folder names are misaligned with the interface structure.

## v4.0.0.5 Requirements

### DTO Restructuring (DTO)

- [ ] **DTO-01**: Maintainer can navigate Agent-layer DTOs through folders that mirror interface boundaries — `IApiApi` sub-folders match its `#region` names, per-host DTOs live in folders named after their interface
- [ ] **DTO-02**: `Dtos/Space/` renamed to `Dtos/UpInfo/` to match `IApiApi #region UpInfo`; namespace updated from `Dtos.Space` → `Dtos.UpInfo` in all 3 DTO files
- [ ] **DTO-03**: `Dtos/Coin/CoinBalance.cs` moved to `Dtos/AccountApi/CoinBalance.cs` to match `IAccountApi`; namespace updated from `Dtos.Coin` → `Dtos.AccountApi`
- [ ] **DTO-04**: Root-level `Dtos/UserInfo.cs` moved to `Dtos/Nav/UserInfo.cs` to match `INavApi`; namespace updated from `Dtos` → `Dtos.Nav`
- [ ] **DTO-05**: All consumers of renamed/moved DTOs (Agent interfaces, DomainServices, AppServices, tests) updated with corrected `using` directives
- [ ] **DTO-06**: Build 0 errors | Architecture tests 4/4 | Integration tests 7/7

## Future Requirements

*(none at this time)*

## Out of Scope

- Renaming DTO classes (file moves and namespace updates only — no class renames)
- Adding or removing DTO properties
- Reorganizing `Live/`, `Manga/`, `Passport/`, `Show/` folders (already correctly aligned with their interfaces)
- Creating a parent `Dtos/ApiApi/` container folder (excessive disruption for marginal gain — IApiApi sub-folders are self-evident)

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| DTO-01 | Phase 12 | planned |
| DTO-02 | Phase 12 | planned |
| DTO-03 | Phase 12 | planned |
| DTO-04 | Phase 12 | planned |
| DTO-05 | Phase 12 | planned |
| DTO-06 | Phase 12 | planned |
