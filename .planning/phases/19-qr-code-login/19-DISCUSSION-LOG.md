# Phase 19: QR Code Login - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-05-07
**Phase:** 19-qr-code-login
**Areas discussed:** Service Layer Strategy, Cookie Enrichment, Timeout/Error UX, QR Display Format

---

## Service Layer Strategy

| Option | Description | Selected |
|--------|-------------|----------|
| A: 在 `ILoginDomainService` 新增方法 | 逻辑集中在 domain 层，复用已有 `IPassportApi`，但让 domain service 更胖 | ✓ |
| B: Web 层新建独立服务 | `IQrLoginService` 直接调用 Refit API，边界清晰，但与 domain 层逻辑有少量重复 | |
| C: 你来决定 | 由我根据代码模式判断 | |

**User's choice:** A
**Notes:** 在 `ILoginDomainService` 上新增 `LoginByQrCodeWebAsync()` 方法，复用 `IPassportApi`，不做 console 渲染。逻辑集中在 domain 层。

---

## Cookie Enrichment

| Option | Description | Selected |
|--------|-------------|----------|
| A: 包含 SetCookie 步骤 | 和控制台流程一致，cookie 更完整。`LoginDomainService.SetCookieAsync()` 已有实现可复用 | ✓ |
| B: 直接保存原始 cookie | 更简单，跳过 SetCookie 步骤。但 cookie 可能缺少某些字段 | |
| C: 你来决定 | 由我判断 | |

**User's choice:** A
**Notes:** 与控制台流程保持一致，确保 cookie 完整性。复用 `LoginDomainService.SetCookieAsync()`。

---

## Timeout and Error UX

| Option | Description | Selected |
|--------|-------------|----------|
| A: 简单超时 + 重试按钮 | 轮询期间显示进度提示，超时后显示"重试"按钮让用户手动重新生成 QR 码。MudDialog 内实现 | ✓ |
| B: 自动刷新 QR 码 | 超时或失效后自动生成新的 QR 码并刷新显示，无需用户操作。连续失败 N 次后才停止 | |
| C: 你来决定 | 由我判断 | |

**User's choice:** A
**Notes:** MudDialog 内轮询 + 进度提示（如"等待扫描... 3/10"），超时或 QR 失效后显示"重试"按钮。简单直接。

---

## QR Display Format

| Option | Description | Selected |
|--------|-------------|----------|
| A: 服务端用 QRCoder 生成 PNG | 用 QRCoder 的 `PngByteQRCode` 生成 PNG 字节数组，转 base64 传给前端 `<img src="data:image/png;base64,..."/>` | ✓ |
| B: 前端用 JS 库渲染 | 只传 QR URL 给前端，用 JS QR 库在浏览器端渲染。不需要 QRCoder 的 PNG 功能，但需引入前端依赖 | |
| C: 你来决定 | 由我判断 | |

**User's choice:** A
**Notes:** 复用已有 QRCoder 依赖，无新依赖引入。服务端生成 base64 PNG 字符串。

---

## Agent's Discretion

以下由 agent 自行决定：
- `LoginByQrCodeWebAsync` 放在 `ILoginDomainService` vs. `BiliAccountPageWorkflow` 内部调用已有 domain service
- Dialog 状态机结构（initial → scanning → success/failed/retry）
- 是否显示 QR URL 作为 fallback 链接
- 超时 vs. 过期 vs. API 失败的具体错误消息
- 是否需要在 Web.csproj 中添加 QRCoder 包

## Deferred Ideas

None — discussion stayed within phase scope.
