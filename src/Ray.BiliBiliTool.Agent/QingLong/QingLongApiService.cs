using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ray.BiliBiliTool.Agent.QingLong.Dtos;

namespace Ray.BiliBiliTool.Agent.QingLong;

/// <summary>
/// 青龙 OpenAPI 客户端（手工 HttpClient 实现）。
/// 说明：青龙 2.20.2+ 将 OpenAPI 默认端口由 5600 调整为 5700（与 Web 同端口），
/// 旧版本（&lt;=2.19）仍使用 5600。本实现支持端口自动探测：
/// 显式配置(QL_URL 或 QingLongConfig:Url) -> http://localhost:5700 -> http://localhost:5600，
/// 避免"升级青龙后 cookie 持久化失败"（对应上游 issue #1067/#1068）。
/// </summary>
public class QingLongApiService(
    IHttpClientFactory httpClientFactory,
    ILogger<QingLongApiService> logger,
    string? configuredUrl
) : IQingLongApi
{
    private const string ClientName = "QingLong";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly object _lock = new();
    private string? _workingHost;

    private string[] GetHostCandidates()
    {
        if (!string.IsNullOrWhiteSpace(configuredUrl))
        {
            return [configuredUrl!.TrimEnd('/')];
        }

        return ["http://localhost:5700", "http://localhost:5600"];
    }

    private string? GetWorkingHost()
    {
        lock (_lock)
        {
            return _workingHost;
        }
    }

    private void SetWorkingHost(string host)
    {
        lock (_lock)
        {
            if (_workingHost == null)
            {
                _workingHost = host;
            }
        }
    }

    private void ResetWorkingHost()
    {
        lock (_lock)
        {
            _workingHost = null;
        }
    }

    private HttpClient CreateClient()
    {
        return httpClientFactory.CreateClient(ClientName);
    }

    private static async Task<T?> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }

    private static StringContent JsonContent(object body)
    {
        return new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// 发送请求；非 2xx 一律视为失败（例如访问到错误的端口/未开启 OpenAPI 时返回 404/502），
    /// 避免把 HTML 错误页"假解析"为成功响应。
    /// </summary>
    private async Task<HttpResponseMessage> SendWithCheckAsync(
        HttpMethod method,
        string host,
        string path,
        string? token,
        HttpContent? content
    )
    {
        using var request = new HttpRequestMessage(method, host + path);
        if (content != null)
        {
            request.Content = content;
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.TryAddWithoutValidation("Authorization", token);
        }

        var response = await CreateClient().SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        // 尝试读取 ql 风格错误体 {code,message}，便于日志定位
        string detail = $"HTTP {(int)response.StatusCode}";
        try
        {
            var err = await ReadJsonAsync<QingLongGenericResponse<object>>(response);
            if (err?.Code != 0 || err.Message != null)
            {
                detail += $" code={err?.Code} message={err?.Message}";
            }
        }
        catch
        {
            // 忽略解析错误，保留 HTTP 状态码信息
        }

        throw new HttpRequestException($"请求青龙 OpenAPI 失败：{detail}（url={host}{path}）");
    }

    public async Task<QingLongGenericResponse<TokenResponse>> GetTokenAsync(
        string client_id,
        string client_secret
    )
    {
        var candidates = GetHostCandidates();
        HttpRequestException? lastException = null;

        foreach (var host in candidates)
        {
            try
            {
                var url =
                    $"{host}/open/auth/token?client_id={Uri.EscapeDataString(client_id)}"
                    + $"&client_secret={Uri.EscapeDataString(client_secret)}";
                using var response = await SendWithCheckAsync(
                    HttpMethod.Get,
                    host,
                    url,
                    null,
                    null
                );
                var result = await ReadJsonAsync<QingLongGenericResponse<TokenResponse>>(response);
                if (result == null)
                {
                    throw new HttpRequestException($"无法解析青龙 OpenAPI 响应（host={host}）");
                }

                if (result.Code == 200 && result.Data != null)
                {
                    SetWorkingHost(host);
                    return result;
                }

                // 能连通但鉴权失败（如 ClientId/Secret 错误），无需再探测其他端口
                return result;
            }
            catch (Exception ex)
                when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                lastException =
                    ex as HttpRequestException
                    ?? new HttpRequestException($"请求青龙 OpenAPI 失败：{ex.Message}", ex);
                logger.LogWarning("青龙 OpenAPI 端口探测失败 {host}：{message}", host, ex.Message);
            }
        }

        throw new HttpRequestException(
            $"青龙 OpenAPI 不可达，已尝试端口：[{string.Join(", ", candidates)}]。"
                + "请检查青龙是否已开启 OpenAPI（系统设置-应用设置）并为 BiliBiliToolPro 创建应用，"
                + "或通过环境变量 QL_URL（如 http://127.0.0.1:5700）显式指定地址",
            lastException
        );
    }

    public async Task<QingLongGenericResponse<List<QingLongEnv>>> GetEnvsAsync(
        string searchValue,
        string token
    )
    {
        var host = GetWorkingHost() ?? GetHostCandidates()[0];
        try
        {
            var url = $"/open/envs?searchValue={Uri.EscapeDataString(searchValue)}";
            using var response = await SendWithCheckAsync(HttpMethod.Get, host, url, token, null);
            var result =
                await ReadJsonAsync<QingLongGenericResponse<List<QingLongEnv>>>(response)
                ?? new QingLongGenericResponse<List<QingLongEnv>> { Code = 200, Data = [] };
            if (result.Code == 200 && result.Data == null)
            {
                result.Data = [];
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            // 端口漂移时（host 已不可用）重置，让下一次 token 流程重新探测
            ResetWorkingHost();
            logger.LogWarning("请求青龙环境变量失败({host})：{message}", host, ex.Message);
            throw;
        }
    }

    public async Task<QingLongGenericResponse<List<QingLongEnv>>> AddEnvsAsync(
        List<AddQingLongEnv> envs,
        string token
    )
    {
        var host = GetWorkingHost() ?? GetHostCandidates()[0];
        using var response = await SendWithCheckAsync(
            HttpMethod.Post,
            host,
            "/open/envs",
            token,
            JsonContent(envs)
        );
        var result = await ReadJsonAsync<QingLongGenericResponse<List<QingLongEnv>>>(response);
        return result ?? new QingLongGenericResponse<List<QingLongEnv>> { Code = 200, Data = [] };
    }

    public async Task<QingLongGenericResponse<QingLongEnv>> UpdateEnvsAsync(
        UpdateQingLongEnv env,
        string token
    )
    {
        var host = GetWorkingHost() ?? GetHostCandidates()[0];
        using var response = await SendWithCheckAsync(
            HttpMethod.Put,
            host,
            "/open/envs",
            token,
            JsonContent(env)
        );
        var result = await ReadJsonAsync<QingLongGenericResponse<QingLongEnv>>(response);
        return result ?? new QingLongGenericResponse<QingLongEnv> { Code = 200, Data = null };
    }
}
