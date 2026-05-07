---
phase: 19-qr-code-login
plan: 01
subsystem: auth
tags: [qr-login, blazor, mudblazor, cookie, bilibili-api]

requires:
  - phase: 18-account-crud-operations
    provides: IBiliAccountPageWorkflow seam and MudDialog patterns
  - phase: 17-account-storage-foundation
    provides: SQLite-backed cookie storage via SqliteConfigurationProvider

provides:
  - QR code login via Bilibili passport API in Web browser
  - Service layer: GenerateQrCodeWebAsync and CheckQrLoginAsync on ILoginDomainService
  - Workflow seam: QrLoginGenerateAsync, QrLoginPollAsync, QrLoginCompleteAsync on IBiliAccountPageWorkflow
  - QrLoginDialog.razor MudDialog with state machine UI

affects: [20-future-milestone]
tech-stack:
  added: []
  patterns: [qr-login-web-domain-service, png-qrcode-generation, muddialog-state-machine]

key-files:
  created:
    - src/Ray.BiliBiliTool.DomainService/Dtos/QrLoginStatus.cs
    - src/Ray.BiliBiliTool.DomainService/Dtos/QrLoginGenerateResult.cs
    - src/Ray.BiliBiliTool.DomainService/Dtos/QrLoginCheckResult.cs
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/QrLoginDialog.razor
  modified:
    - src/Ray.BiliBiliTool.DomainService/Interfaces/ILoginDomainService.cs
    - src/Ray.BiliBiliTool.DomainService/LoginDomainService.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/IBiliAccountPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountPageWorkflow.cs
    - src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/BiliAccount.razor
    - test/Ray.BiliBiliTool.CharacterizationTests/LoginTaskCharacterizationTests.cs
    - test/Ray.BiliBiliTool.CharacterizationTests/DailyTaskCharacterizationTests.cs

key-decisions:
  - "QR image generated as base64 PNG via QRCoder PngByteQRCode in domain service (per D-01, D-05)"
  - "Cookie enrichment via SetCookieAsync before saving to SQLite (per D-02)"
  - "Poll loop: 5s interval, 10 max attempts, matching Console flow (per D-03)"
  - "Retry button generates new QR code without closing dialog (per D-04)"
  - "OnlineUrl fallback link for high-definition QR code viewing"

patterns-established:
  - "Web QR login domain service pattern: GenerateQrCodeWebAsync returns base64 PNG + qrcode key + online URL"
  - "QR check result discriminated union: Waiting/Success/Expired/Error status enum"
  - "MudDialog state machine: Generating -> Scanning -> Success/Failed/Expired with retry capability"

requirements-completed: [ACCT-02]

duration: ~15min
completed: 2026-05-07
---

# Plan 19-01: QR Code Login in Web Browser Summary

**QR login flow fully integrated into BiliAccount page — domain service, workflow seam, and MudDialog with polling state machine.**

## Performance

- **Duration:** ~15 min
- **Started:** 2026-05-07T00:00:00Z
- **Completed:** 2026-05-07T00:15:00Z
- **Tasks:** 2 completed
- **Files modified:** 11 (4 created, 7 modified)

## Accomplishments

- Implemented `GenerateQrCodeWebAsync` and `CheckQrLoginAsync` on `ILoginDomainService` reusing existing `IPassportApi` calls and `PngByteQRCode` for PNG generation
- Extended `IBiliAccountPageWorkflow` with three QR login methods: `QrLoginGenerateAsync`, `QrLoginPollAsync`, `QrLoginCompleteAsync`
- Created `QrLoginDialog.razor` with full state machine (Generating → Scanning → Success/Failed/Expired), progress indicator, retry support, and online URL fallback
- Added "Login with QR" button to BiliAccount page with automatic account list refresh on success
- Fixed test doubles in characterization tests to implement new interface methods
- Full solution builds with 0 errors; 3/4 characterization tests pass (1 pre-existing failure from Phase 5)

## Task Commits

Each task was committed atomically:

1. **Task 1: QR login service layer** - `1e7d5bc` (feat)
2. **Task 2: QR login dialog and page integration** - `25d97b5` (feat)
3. **Bonus: Test double fix** - `bff8ee2` (fix)

## Deviations from Plan

None — plan executed exactly as written.

## Self-Check

- [x] `ILoginDomainService` has `GenerateQrCodeWebAsync` and `CheckQrLoginAsync` methods
- [x] `LoginDomainService` implements both using `PngByteQRCode` and `IPassportApi`
- [x] `IBiliAccountPageWorkflow` has `QrLoginGenerateAsync`, `QrLoginPollAsync`, `QrLoginCompleteAsync`
- [x] `BiliAccountPageWorkflow` implements all three with cookie enrichment via `SetCookieAsync`
- [x] DTOs `QrLoginGenerateResult`, `QrLoginCheckResult`, `QrLoginStatus` enum exist in DomainService/Dtos
- [x] `QrLoginDialog.razor` exists with full state machine and polling
- [x] `BiliAccount.razor` has "Login with QR" button
- [x] DomainService project builds with 0 errors
- [x] Web project builds with 0 errors
- [x] Full solution builds with 0 errors
- [x] Existing tests still pass (pre-existing failure excluded)
- [x] `dotnet build Ray.BiliBiliTool.sln` — 0 errors

## Verification Results

| Check | Result |
|-------|--------|
| `dotnet build Ray.BiliBiliTool.sln` | 0 errors, warnings only (pre-existing) |
| `dotnet test CharacterizationTests` | 3/4 pass (1 pre-existing failure) |
| Key files exist on disk | All 11 files present |
| Commits in git log | 3 commits on feature branch |
