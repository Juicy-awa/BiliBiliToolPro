using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.Mall;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

/// <summary>
/// 大会员大积分
/// </summary>
[Headers("Host: api.bilibili.com")]
public interface IMallApi
{
    /// <summary>
    /// 签到任务
    /// </summary>
    /// <param name="requestPath"></param>
    /// <param name="request"></param>
    /// <param name="ck"></param>
    /// <returns></returns>
    [Headers("Referer: https://big.bilibili.com/mobile/index")]
    [Post("/pgc/activity/score/task/sign2")]
    Task<BiliApiResponse<Sign2Response>> Sign2Async(
        [Query] Sign2RequestPath requestPath,
        [Body] Sign2Request request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 获取任务 combine 信息
    /// </summary>
    /// <remarks>里面的登录信息是错误的，阿B特色</remarks>
    /// <returns></returns>
    [Headers("Referer: https://big.bilibili.com/mobile/bigPoint/task")]
    [Get("/x/vip_point/task/combine")]
    Task<BiliApiResponse<VipBigPointCombine>> GetCombineAsync(
        [Query] GetCombineRequest request,
        [Header("Cookie")] string ck
    );
}
