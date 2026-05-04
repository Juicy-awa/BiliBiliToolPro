using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.Mall;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.VipTask;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.VipTask.ThreeDaysSign;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

/// <summary>
/// 大会员大积分
/// </summary>
[Headers("Host: api.bilibili.com", "Referer: https://big.bilibili.com/mobile/bigPoint/task")]
public interface IVipBigPointApi
{
    /// <summary>
    /// 获取签到信息
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ck"></param>
    /// <returns></returns>
    [Get("/x/vip/vip_center/sign_in/three_days_sign")]
    Task<BiliApiResponse<ThreeDaySignResponse>> GetThreeDaySignAsync(
        [Query] ThreeDaySignRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 获取任务列表
    /// </summary>
    /// <remarks>里面的登录信息是错误的，阿B特色</remarks>
    /// <returns></returns>
    [Obsolete("Using IMallApi.GetCombineAsync instead.")]
    [Get("/x/vip_point/task/combine")]
    Task<BiliApiResponse<VipBigPointCombine>> GetCombineAsync([Header("Cookie")] string ck);

    /// <summary>
    /// 签到任务
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Obsolete("Using IMallApi.Sign2Async instead.")]
    [Post("/pgc/activity/score/task/sign")]
    Task<BiliApiResponse> SignAsync(
        [Body(BodySerializationMethod.UrlEncoded)] SignRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 领取任务
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Obsolete]
    [Post("/pgc/activity/score/task/receive")]
    Task<BiliApiResponse> Receive(
        [Body] ReceiveOrCompleteTaskRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 领取任务
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Post("/pgc/activity/score/task/receive/v2")]
    Task<BiliApiResponse> ReceiveV2(
        [Body(BodySerializationMethod.UrlEncoded)] ReceiveOrCompleteTaskRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Post("/pgc/activity/score/task/complete")]
    Task<BiliApiResponse> CompleteAsync(
        [Body] ReceiveOrCompleteTaskRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Post("/pgc/activity/score/task/complete/v2")]
    Task<BiliApiResponse> CompleteV2(
        [Body(BodySerializationMethod.UrlEncoded)] ReceiveOrCompleteTaskRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 完成浏览页面任务
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ck"></param>
    /// <returns></returns>
    [Post("/pgc/activity/deliver/task/complete")]
    Task<BiliApiResponse> ViewComplete(
        [Body(BodySerializationMethod.UrlEncoded)] ViewRequest request,
        [Header("Cookie")] string ck
    );

    [Get("/x/vip/privilege/my")]
    Task<BiliApiResponse<VouchersInfoResponse>> GetVouchersInfoAsync([Header("Cookie")] string ck);

    /// <summary>
    /// 兑换大会员经验
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Post("/x/vip/experience/add")]
    Task<BiliApiResponse> ObtainVipExperienceAsync(
        [Body(BodySerializationMethod.UrlEncoded)] VipExperienceRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 开始观看剧集任务
    /// NOTE: HTTP verb unknown — stub method without Refit verb attribute
    /// </summary>
    Task<BiliApiResponse<StartOgvWatchResponse>> StartOgvWatchAsync(
        StartOgvWatchRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 完成观看剧集任务
    /// NOTE: HTTP verb unknown — stub method without Refit verb attribute
    /// </summary>
    Task<BiliApiResponse> CompleteOgvWatchAsync(
        CompleteOgvWatchRequest request,
        [Header("Cookie")] string ck
    );
}
