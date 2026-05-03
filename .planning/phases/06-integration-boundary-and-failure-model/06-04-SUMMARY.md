# Plan 06-04 Summary: BiliCookie/CookieInfo Exception Sweep + Unit Tests

**Phase:** 06-integration-boundary-and-failure-model
**Plan:** 04
**Completed:** 2026-05-03
**Commit:** feat(06-04): convert BiliCookie and CookieInfo throws to BiliValidationException; add BiliCookieExceptionTests; fix pre-existing format string bugs

## What Was Built

### Task 1: Exception conversion in CookieInfo and BiliCookie

- `Infrastructure.csproj` — added `<ProjectReference>` to Domain project (needed for BiliValidationException)
- `CookieInfo.cs` — 1 throw converted: `throw new Exception("Cookie字符串为空")` → `BiliValidationException`
- `BiliCookie.cs` — 6 throws in `Check()` converted to `BiliValidationException`

#### Pre-existing Bugs Discovered and Fixed (per D-04 intent)

During conversion, two format string bugs were found that caused `FormatException` before the intended validation exception was ever raised:

| Bug | Original | Fixed |
|-----|----------|-------|
| `msg` format placeholder | `"Cookie字符串异常，无[{1}]项"` | `"Cookie字符串异常，无[{0}]项"` |
| NonNumericUserId format | `"[{uidKey}]={uid} ..."` (named, invalid) | `"[{0}]={1} ..."` (positional, valid) |

Both were clearly unintentional bugs (the intended output was obvious from context). Fixing them makes the `BiliValidationException` sweep meaningful — without the fix, the code would throw `FormatException` instead of any `BiliValidationException`.

### Task 2: Unit tests

Created `test/BiliAgentTest/BiliCookieExceptionTests.cs` with 8 tests:

| Test | Validates |
|------|-----------|
| `Check_EmptyDictionary_ThrowsBiliValidationException` | CookieInfo base validation path |
| `Check_MissingUserId_ThrowsBiliValidationException` | UserId check |
| `Check_NonNumericUserId_ThrowsBiliValidationException` | UserId parse check |
| `Check_MissingSessData_ThrowsBiliValidationException` | SessData check |
| `Check_MissingBiliJct_ThrowsBiliValidationException` | BiliJct check |
| `BiliResiliencePolicies_ReadOnlyRetryCount_IsOne` | Policy constant |
| `BiliResiliencePolicies_HttpTimeout_Is30Seconds` | Policy constant |
| `BiliResiliencePolicies_ReadOnlyRetryBackoff_Is2Seconds` | Policy constant |

Also added explicit Domain project reference to `BiliAgentTest.csproj`.

## Verification Results
- 8/8 new tests pass ✓
- 4/4 architecture tests pass ✓
- `dotnet build Ray.BiliBiliTool.sln`: 0 errors, 106 pre-existing warnings

## Files Modified
- `src/Ray.BiliBiliTool.Infrastructure/Ray.BiliBiliTool.Infrastructure.csproj`
- `src/Ray.BiliBiliTool.Infrastructure/Cookie/CookieInfo.cs`
- `src/Ray.BiliBiliTool.Agent/BiliCookie.cs`
- `test/BiliAgentTest/BiliCookieExceptionTests.cs` (new)
- `test/BiliAgentTest/BiliAgentTest.csproj`
