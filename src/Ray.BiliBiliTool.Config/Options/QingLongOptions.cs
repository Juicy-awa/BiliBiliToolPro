namespace Ray.BiliBiliTool.Config.Options;

public class QingLongOptions
{
    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    /// <summary>
    /// 青龙 OpenAPI 地址，如 http://127.0.0.1:5700。
    /// 不配置时自动探测（5700 -> 5600）；也可用环境变量 QL_URL 覆盖。
    /// </summary>
    public string? Url { get; set; }
}
