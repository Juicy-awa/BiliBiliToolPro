# Phase 19: QR Code Login - Context

**Gathered:** 2026-05-07
**Status:** Ready for planning

<domain>
## Phase Boundary

Enable QR code login directly in the Web browser so maintainers can add Bili accounts without manually copying cookie strings. The maintainer clicks "Login with QR", a Bilibili QR code is displayed as a base64 PNG image, the page polls for scan result, and on success the resulting cookie is enriched (via `SetCookieAsync`) and saved to SQLite.

Phase 19 builds on Phase 17's SQLite storage, Phase 18's `IBiliAccountPageWorkflow` CRUD seam and MudDialog patterns, and the existing `LoginDomainService.LoginByQrCodeAsync()` QR login flow.

</domain>

<decisions>
## Implementation Decisions

### A: Service Layer Strategy (D-01)

- **D-01:** Add a new method to `ILoginDomainService` for Web-specific QR login — `LoginByQrCodeWebAsync()` that returns both the `BiliCookie` and the QR image data (base64 PNG). This method reuses `IPassportApi.GenerateQrCode()` and `IPassportApi.CheckQrCodeHasScaned()` but does NOT render to console/logger. The existing `LoginByQrCodeAsync()` remains untouched for Console flows.

### B: Cookie Enrichment (D-02)

- **D-02:** The Web QR login flow MUST include the `SetCookieAsync` step after successful QR scan. After the QR scan returns a raw `BiliCookie`, the flow calls `LoginDomainService.SetCookieAsync(cookie)` to enrich it (visits Bilibili home page, extracts additional cookie fields like `bili_jct`, `sid`). The enriched cookie is then saved via `IBiliAccountPageWorkflow.AddAsync(cookieStr)`. This ensures parity with the Console flow.

### C: Timeout and Error UX (D-03, D-04)

- **D-03:** The QR login runs inside a MudDialog. During polling, the dialog shows a progress indicator with current poll count (e.g., "等待扫描... 3/10"). Poll interval: 5 seconds, max 10 attempts (50 seconds total), matching the existing Console flow.
- **D-04:** On timeout or QR code expiration (`Code == 86038`), the dialog shows an error message and a "重试" (Retry) button. Clicking retry generates a new QR code and restarts the poll loop. The dialog stays open — user does not need to re-open it.

### D: QR Display Format (D-05)

- **D-05:** The QR code is rendered as a base64 PNG image on the server side using QRCoder's `PngByteQRCode` class. The resulting base64 string is passed to the Blazor component, which renders it as `<img src="data:image/png;base64,..." />`. The `QRCoder` package is already referenced in `DomainService.csproj` — it must also be added to the Web project's `.csproj` if the QR generation happens in the Web layer, or kept in domain service and returned as part of the response.

### E: UI Integration (D-06)

- **D-06:** A "Login with QR" button is added to the BiliAccount page next to the existing "Add Account" button. Clicking it opens a `QrLoginDialog.razor` MudDialog. On successful login + save, the dialog closes and the account list refreshes automatically (same pattern as `AddAccountDialog` in Phase 18).

### Agent's Discretion

- Whether `LoginByQrCodeWebAsync` lives on `ILoginDomainService` vs. a separate method on `BiliAccountPageWorkflow` that calls the existing domain service internally
- How to structure the dialog's state machine (initial → scanning → success/failed/retry)
- Whether to show the QR URL as a fallback link (like the Console flow's "高清二维码" link)
- Exact error messages for timeout vs. expiration vs. API failure
- Whether to add QRCoder package to Web.csproj or keep QR generation in domain service

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Existing QR Login Flow (Console)
- `src/Ray.BiliBiliTool.DomainService/LoginDomainService.cs` — `LoginByQrCodeAsync()` (line 34-102): full QR generate → poll → extract cookie flow; `SetCookieAsync()` (line 104+): cookie enrichment; `GenerateQrCode()` (line 313+): QRCoder ASCII rendering
- `src/Ray.BiliBiliTool.DomainService/Interfaces/ILoginDomainService.cs` — interface to extend with Web method

### Bilibili Passport API (Refit)
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Interfaces/IPassportApi.cs` — `GenerateQrCode()` returns `QrCodeDto`; `CheckQrCodeHasScaned(qrcode_key)` returns `HttpResponseMessage` with `Set-Cookie` headers
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/PassportApi/QrCodeDto.cs` — `Url` (QR content), `Qrcode_key` (poll key)
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/PassportApi/TokenDto.cs` — `Code`: 0=success, 86038=expired; `Message`

### Web Layer — Account Page (Phases 17-18 output)
- `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/IBiliAccountPageWorkflow.cs` — `AddAsync(cookieStr)` for saving to SQLite
- `src/Ray.BiliBiliTool.Web/Services/Pages/BiliAccount/BiliAccountPageWorkflow.cs` — implementation with `SqliteConfigurationProvider` + `ReloadConfiguration()`
- `src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/BiliAccount.razor` — page to add "Login with QR" button
- `src/Ray.BiliBiliTool.Web/Components/Pages/BiliAccount/AddAccountDialog.razor` — reference MudDialog pattern

### Cookie Utilities
- `src/Ray.BiliBiliTool.Infrastructure/Cookie/CookieStrFactory.cs` — `CreateNew(cookieStr)` to parse cookie string into `BiliCookie`
- `src/Ray.BiliBiliTool.Agent/BiliCookie.cs` — cookie model with `Check()` validation
- `src/Ray.BiliBiliTool.Infrastructure/Cookie/CookieInfo.cs` — `ConvertSetCkHeadersToCkStr()` to extract cookies from `Set-Cookie` headers

### Libraries
- QRCoder — already in `DomainService.csproj`; `PngByteQRCode` for PNG generation
- MudBlazor — already in `Web.csproj`; `MudDialog`, `MudProgressLinear`, `MudButton`

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `LoginDomainService.LoginByQrCodeAsync()`: Complete QR flow logic — `IPassportApi.GenerateQrCode()` → poll loop with `CheckQrCodeHasScaned()` → extract `Set-Cookie` headers → `CookieStrFactory.CreateNew()`. Can be adapted for Web without console rendering.
- `LoginDomainService.SetCookieAsync()`: Cookie enrichment — visits Bilibili home page to get additional cookie fields. Ready to reuse as-is.
- `BiliAccountPageWorkflow`: Already has `AddAsync(cookieStr)` that writes to SQLite via `SqliteConfigurationProvider` and reloads config. QR login result flows through this.
- `CookieInfo.ConvertSetCkHeadersToCkStr()`: Extracts cookie string from HTTP `Set-Cookie` response headers. Used in the existing QR flow.

### Established Patterns
- MudDialog pattern: Phase 18 created `AddAccountDialog`, `EditAccountDialog`, `DeleteAccountDialog` — all use `IMudDialogInstance`, `DialogParameters`, return data via `MudDialog.Close(DialogResult.Ok(data))`
- Workflow seam: `IBiliAccountPageWorkflow` is the single entry point for account mutations from Blazor pages. QR login should add a method here (e.g., `QrLoginAsync()`) rather than calling domain service directly from the page.
- Blazor Server `@rendermode InteractiveServer`: Server-side rendering with SignalR. Polling loops with `StateHasChanged()` work naturally — no need for JS interop for polling.

### Integration Points
- `BiliAccount.razor` — add "Login with QR" button alongside existing "Add Account" button
- `IBiliAccountPageWorkflow` — add `QrLoginAsync()` method that orchestrates: domain service QR login → cookie enrichment → save to SQLite
- `ILoginDomainService` — add `LoginByQrCodeWebAsync()` returning QR image data + polling capability
- DI registration in `ServiceCollectionExtension.cs` — may need to register new services

</code_context>

<specifics>
## Specific Ideas

- The Console flow's `GenerateQrCode()` also prints a "高清二维码" link for browser viewing. Consider including a similar fallback URL in the dialog for users who can't scan the rendered image.
- The existing `IPassportApi.CheckQrCodeHasScaned()` returns `HttpResponseMessage` (not a typed response) because it needs to read `Set-Cookie` headers. The Web flow must handle this the same way.
- `CookieStrFactory<BiliCookie>.CreateNew(cookieStr)` and `BiliCookie.Check()` should be called on the enriched cookie before saving to validate completeness.

</specifics>

<deferred>
## Deferred Ideas

None — discussion stayed within phase scope.

</deferred>

---

*Phase: 19-qr-code-login*
*Context gathered: 2026-05-07*
