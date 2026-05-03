# Plan 06-03 Summary: Exception Sweep — Seven DomainService Files

**Phase:** 06-integration-boundary-and-failure-model
**Plan:** 03
**Completed:** 2026-05-03
**Commit:** feat(06-03): replace all generic Exception throws in seven DomainService files with typed BiliBusinessException/BiliIntegrationException

## What Was Built

Converted all 14 `throw new Exception(...)` in the DomainService layer to typed exceptions from the BiliException hierarchy. Exception messages preserved verbatim.

### Classification Applied (per D-03, D-05, D-06)
- API non-zero Code → `BiliBusinessException`
- QingLong/timeout/network failures → `BiliIntegrationException`

### Conversions by File

| File | Count | Types Used |
|------|-------|-----------|
| LoginDomainService.cs | 4 | 1x BiliBusinessException (QR code fail), 3x BiliIntegrationException (timeout, QingLong token, QingLong env) |
| AccountDomainService.cs | 1 | BiliBusinessException (cookie login fail) |
| LiveDomainService.cs | 1 | BiliBusinessException (live API fail; null-coalesced message) |
| VideoDomainService.cs | 2 | BiliBusinessException (API response code ≠ 0) |
| ArticleDomainService.cs | 2 | BiliBusinessException (API response code ≠ 0) |
| DonateCoinDomainService.cs | 1 | BiliBusinessException (coin donation error) |
| VipBigPointDomainService.cs | 3 | BiliBusinessException (VIP task failures) |

Also cleaned up trailing double semicolon `; ;` in AccountDomainService.cs.

## Verification Results
- `grep "throw new Exception" DomainService/` → 0 matches ✓
- `dotnet build Ray.BiliBiliTool.sln`: 0 errors, 106 pre-existing warnings
- `using Ray.BiliBiliTool.Domain.Exceptions;` added to all 7 files

## Files Modified
- `src/Ray.BiliBiliTool.DomainService/LoginDomainService.cs`
- `src/Ray.BiliBiliTool.DomainService/AccountDomainService.cs`
- `src/Ray.BiliBiliTool.DomainService/LiveDomainService.cs`
- `src/Ray.BiliBiliTool.DomainService/VideoDomainService.cs`
- `src/Ray.BiliBiliTool.DomainService/ArticleDomainService.cs`
- `src/Ray.BiliBiliTool.DomainService/DonateCoinDomainService.cs`
- `src/Ray.BiliBiliTool.DomainService/VipBigPointDomainService.cs`
