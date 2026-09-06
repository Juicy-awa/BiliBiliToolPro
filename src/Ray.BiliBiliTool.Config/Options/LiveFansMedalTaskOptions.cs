namespace Ray.BiliBiliTool.Config.Options;

/// <summary>
/// 粉丝牌等级任务相关配置
/// </summary>
public class LiveFansMedalTaskOptions : BaseConfigOptions
{
    public override string SectionName => "LiveFansMedalTaskConfig";

    /// <summary>
    /// 自定义发送弹幕内容，如 "打卡" 等来触发直播间内机器人关键词
    /// </summary>
    public string DanmakuContent { get; set; } = "OvO";

    /// <summary>
    /// 心跳包发送的个数 / 单个直播间挂机的时间，单位为分钟。
    /// 按 B 站新规(粉丝牌每日观看时长亲密度)默认 25 分钟/房
    /// </summary>
    public int HeartBeatNumber { get; set; } = 25;

    /// <summary>
    /// 当心跳包发送连续失败多少次时放弃
    /// </summary>
    public int HeartBeatSendGiveUpThreshold { get; set; } = 5;

    /// <summary>
    /// 对直播时长任务是否跳过粉丝牌等级大于等于 20 的
    /// </summary>
    public bool IsSkipLevel20Medal { get; set; } = true;

    /// <summary>
    /// 单个直播间两次心跳包的发送间隔（秒），最小 30
    /// </summary>
    public int HeartBeatIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// 本次"直播观看时长"任务整体挂机的时间上限（分钟），0 表示不限制。
    /// 粉丝牌较多时避免单次任务过长，默认限制 2 小时
    /// </summary>
    public int HeartBeatGlobalMaxMinutes { get; set; } = 120;

    /// <summary>
    /// 点赞次数，默认值为30（用于点亮粉丝勋章）
    /// </summary>
    public int LikeNumber { get; set; } = 30;

    /// <summary>
    /// 每个直播间发送弹幕次数。按 B 站新规(每日10条弹幕)默认 10；
    /// 若不需要维持亲密度可调小以减少请求
    /// </summary>
    public int SendDanmakuNumber { get; set; } = 10;

    /// <summary>
    /// 弹幕发送失败多少次时放弃
    /// </summary>
    public int SendDanmakugiveUpThreshold { get; set; } = 3;

    /// <summary>
    /// 连续发送弹幕的随机间隔下限（秒），默认 3
    /// </summary>
    public int SendDanmakuIntervalMinSeconds { get; set; } = 3;

    /// <summary>
    /// 连续发送弹幕的随机间隔上限（秒），默认 8；
    /// 单日多条弹幕时随机间隔可降低风控概率，可按需调大
    /// </summary>
    public int SendDanmakuIntervalMaxSeconds { get; set; } = 8;

    public override Dictionary<string, string> ToConfigDictionary()
    {
        return MergeConfigDictionary(
            new Dictionary<string, string>
            {
                { $"{SectionName}:{nameof(DanmakuContent)}", DanmakuContent },
                { $"{SectionName}:{nameof(HeartBeatNumber)}", HeartBeatNumber.ToString() },
                {
                    $"{SectionName}:{nameof(HeartBeatSendGiveUpThreshold)}",
                    HeartBeatSendGiveUpThreshold.ToString()
                },
                {
                    $"{SectionName}:{nameof(IsSkipLevel20Medal)}",
                    IsSkipLevel20Medal.ToString().ToLower()
                },
                {
                    $"{SectionName}:{nameof(HeartBeatIntervalSeconds)}",
                    HeartBeatIntervalSeconds.ToString()
                },
                {
                    $"{SectionName}:{nameof(HeartBeatGlobalMaxMinutes)}",
                    HeartBeatGlobalMaxMinutes.ToString()
                },
                { $"{SectionName}:{nameof(LikeNumber)}", LikeNumber.ToString() },
                { $"{SectionName}:{nameof(SendDanmakuNumber)}", SendDanmakuNumber.ToString() },
                {
                    $"{SectionName}:{nameof(SendDanmakugiveUpThreshold)}",
                    SendDanmakugiveUpThreshold.ToString()
                },
                {
                    $"{SectionName}:{nameof(SendDanmakuIntervalMinSeconds)}",
                    SendDanmakuIntervalMinSeconds.ToString()
                },
                {
                    $"{SectionName}:{nameof(SendDanmakuIntervalMaxSeconds)}",
                    SendDanmakuIntervalMaxSeconds.ToString()
                },
            }
        );
    }
}
