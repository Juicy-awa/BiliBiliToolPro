using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

/// <summary>
/// BiliBili每日任务相关接口
/// </summary>
[Headers("Host: api.bilibili.com")]
public interface IDailyTaskApi
{
    /// <summary>
    /// 获取每日任务的完成情况
    /// </summary>
    /// <returns></returns>
    [Headers(
        "Referer: https://account.bilibili.com/account/home",
        "Origin: https://account.bilibili.com"
    )]
    [Get("/x/member/web/exp/reward")]
    Task<BiliApiResponse<DailyTaskInfo>> GetDailyTaskRewardInfoAsync([Header("Cookie")] string ck);

    /// <summary>
    /// 获取通过投币已获取的经验值
    /// </summary>
    /// <returns></returns>
    [Headers("Referer: https://www.bilibili.com/", "Origin: https://www.bilibili.com")]
    [Get("/x/web-interface/coin/today/exp")]
    Task<BiliApiResponse<int>> GetDonateCoinExpAsync([Header("Cookie")] string ck);

    /// <summary>
    /// 获取VIP特权
    /// </summary>
    /// <param name="type"></param>
    /// <param name="csrf"></param>
    /// <returns></returns>
    [Post("/x/vip/privilege/receive?type={type}&csrf={csrf}")]
    Task<BiliApiResponse> ReceiveVipPrivilegeAsync(
        int type,
        string csrf,
        [Header("Cookie")] string ck
    );
}
