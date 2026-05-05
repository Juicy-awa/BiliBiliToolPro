---
phase: 13-web-boundary-foundation
plan: 01
subsystem: web-component-tests
tags: [bunit, xunit, web, test-harness, login, admin]
key-files:
  created:
    - test/Ray.BiliBiliTool.Web.ComponentTests/Ray.BiliBiliTool.Web.ComponentTests.csproj
    - test/Ray.BiliBiliTool.Web.ComponentTests/_Imports.razor
    - test/Ray.BiliBiliTool.Web.ComponentTests/LoginPageTests.cs
    - test/Ray.BiliBiliTool.Web.ComponentTests/AdminPageTests.cs
  modified:
    - Directory.Packages.props
    - Ray.BiliBiliTool.sln
metrics:
  files_created: 4
  files_modified: 2
  tests_added: 6
---

# Phase 13-01 Summary — Web Component-Test Harness

## What Was Built

1. **`Ray.BiliBiliTool.Web.ComponentTests`** — Dedicated bUnit/xUnit test project for the Web layer (`Microsoft.NET.Sdk.Razor`; references the Web project; versioned via central package management). Added to the solution.

2. **`bunit 1.34.0`** — Added to `Directory.Packages.props`; test project references it without hardcoded version.

3. **`LoginPageTests`** — 3 baseline component tests:
   - No-error state: no error alert rendered (pins `HasLoginError: false` → no MudAlert)
   - Error state: error alert rendered (pins `HasLoginError: true` → MudAlert with message)
   - Password toggle adornment renders (pins icon-button presence)
   - Uses `FakeLoginPageStateFactory` (returns pre-built `LoginPageState`; URI-independent)

4. **`AdminPageTests`** — 3 baseline component tests:
   - Username loaded from `IAuthService` on init (pins `OnInitializedAsync` behavior)
   - Empty-field submit shows "Password cannot be empty" validation error (pins the short-circuit branch)
   - Form has ≥4 input fields (pins structural layout)
   - Uses `FakeAuthService` inline double

## Commits

| Task | Commit | Description |
|------|--------|-------------|
| Task 1 + 2 | 78b802c | feat(13-01): add Web component-test harness with baseline Login and Admin tests |

## Deviations

- Executed 13-02 before 13-01 (within same Wave 1) to avoid a type-not-found compile error: `ILoginPageStateFactory` must exist before test files that reference it compile. The wave ordering was preserved; only the within-wave sequence was adjusted.

## Self-Check: PASSED

- `dotnet test test/Ray.BiliBiliTool.Web.ComponentTests/... -v minimal` → 6 passed, 0 failed
- `dotnet build Ray.BiliBiliTool.sln --no-restore -v minimal` → 0 errors (full solution)
- Project is in `Ray.BiliBiliTool.sln` ✓
- `bunit 1.34.0` in `Directory.Packages.props` ✓
- Login error-alert baseline locked ✓
- Admin username-loading and validation baseline locked ✓
