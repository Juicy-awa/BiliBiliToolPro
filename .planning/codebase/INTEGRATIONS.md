# Integrations

Mapped: 2026-04-27

## Primary External Domain

- The core integration target is Bilibili. The README describes the tool as an automation assistant for Bilibili tasks, and the typed clients are registered in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.
- The client layer targets multiple Bilibili host groups through `BiliHosts.*` constants referenced in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.

## Bilibili API Surfaces

- General API clients: `IUserInfoApi`, `IUpInfoApi`, `IDailyTaskApi`, `IRelationApi`, `IChargeApi`, `IVideoApi`, `IVideoWithoutCookieApi`, and `IArticleApi`.
- Other Bilibili domains: `IVipMallApi`, `IPassportApi`, `ILiveTraceApi`, `IHomeApi`, `IMangaApi`, `IAccountApi`, `ILiveApi`, `IVipBigPointApi`, and `IMallApi`.
- These clients are configured through `AddHttpApi<TInterface>` plus delegating handlers and Polly retry in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.

## Authentication And Session Inputs

- Cookie-based auth is central. `CookieStrFactory<BiliCookie>` is registered in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.
- The console host loads cookies from `cookies.json`, user secrets, environment variables, and command-line options in `src\Ray.BiliBiliTool.Console\Program.cs`.
- The web host also loads `config/cookies.json` and can overlay configuration from SQLite in `src\Ray.BiliBiliTool.Web\Program.cs`.

## QingLong Integration

- A QingLong HTTP client is registered via `IQingLongApi` in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.
- The host defaults to `http://localhost:5600` unless `QL_URL` is configured.
- QingLong-specific configuration types exist in `src\Ray.BiliBiliTool.Config\Options\QingLongOptions.cs`.
- Operational artifacts for QingLong live in `qinglong\` and `qinglong\DefaultTasks\`.

## Persistence And Internal Service Integrations

- EF Core is added through `AddDbContextFactory<BiliDbContext>()` in `src\Ray.BiliBiliTool.Infrastructure.EF\Extensions\ServiceCollectionExtension.cs`.
- The web host can read config values from SQLite by calling `builder.Configuration.AddSqlite(...)` in `src\Ray.BiliBiliTool.Web\Program.cs`.
- Quartz uses a persistent Microsoft SQLite store in `src\Ray.BiliBiliTool.Web\Program.cs`.

## Logging And Notification Channels

- Serilog sinks include Console, Debug, File, and SQLite.
- Custom notification sinks listed in `Directory.Packages.props` and referenced by the host projects include Telegram, DingTalk, PushPlus, ServerChan, CoolPush, WorkWeiXin, WorkWeiXinApp, Microsoft Teams, Gotify, and a generic OtherApi sink.
- The README explicitly documents remote push notifications and optional webhook-style delivery.

## UI And Documentation Integrations

- Swagger/OpenAPI is configured in `src\Ray.BiliBiliTool.Web\Program.cs` and exposed under `/swagger`.
- MudBlazor powers the Blazor UI in `src\Ray.BiliBiliTool.Web\Program.cs` and `src\Ray.BiliBiliTool.Web.Client\Program.cs`.
- QR code generation support is implied by the `QRCoder` dependency from `Directory.Packages.props` and the login flow documented in the README.

## Deployment And Platform Integrations

- Container packaging: `Dockerfile`, `docker\`, and `podman\`.
- Chart packaging: `helm\`.
- Tencent SCF deployment assets: `tencentScf\`.
- GitHub workflow automation: `.github\workflows\publish-image.yml`, `.github\workflows\publish-release.yml`, and `.github\workflows\codeql-analysis.yml` visible from the solution file.

## Reliability Layers Around Integrations

- Each typed client gets retry handling through `GetRetryPolicy()` in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.
- Request shaping uses delegating handlers discovered by Scrutor-style scanning from the agent assembly.
- Optional proxy support is applied globally by `SetGlobalProxy(...)` in `src\Ray.BiliBiliTool.Agent\Extensions\ServiceCollectionExtension.cs`.