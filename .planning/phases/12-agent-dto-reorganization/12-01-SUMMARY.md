# Summary: 12-01 — Move UpInfo DTOs Under Api Root

## What Was Built
Moved the current top-level UpInfo DTO slice under the interface-first `Dtos/ApiApi/UpInfo/` root and updated the namespace declarations inside each moved file.

## Files Modified
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/ApiApi/UpInfo/GetSpaceInfoFullDto.cs` — moved from top-level `Dtos/UpInfo/`; namespace updated to `Dtos.ApiApi.UpInfo`
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/ApiApi/UpInfo/GetSpaceInfoResponse.cs` — moved from top-level `Dtos/UpInfo/`; namespace updated to `Dtos.ApiApi.UpInfo`
- `src/Ray.BiliBiliTool.Agent/BiliBiliAgent/Dtos/ApiApi/UpInfo/UpInfo.cs` — moved from top-level `Dtos/UpInfo/`; namespace updated to `Dtos.ApiApi.UpInfo`

## Validation
- Destination files exist in `Dtos/ApiApi/UpInfo/`
- Old top-level `Dtos/UpInfo/` folder deleted
- Namespace checks confirmed `Dtos.ApiApi.UpInfo`
- Existing `Dtos/AccountApi/` and `Dtos/NavApi/` roots left unchanged

## Self-Check: PASSED
- File relocation verified
- Namespace relocation verified
