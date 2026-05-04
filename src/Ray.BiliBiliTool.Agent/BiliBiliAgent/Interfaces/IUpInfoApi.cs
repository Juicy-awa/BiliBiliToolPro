using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

/// <summary>
/// 用户信息接口API
/// </summary>
[Headers(
    "Referer: https://www.bilibili.com/",
    "Origin: https://www.bilibili.com",
    "Host: api.bilibili.com"
)]
public interface IUpInfoApi
{
    /// <summary>
    /// 获取用户空间信息
    /// </summary>
    /// <param name="userId">uid</param>
    /// <returns></returns>
    [Get("/x/space/wbi/acc/info")]
    Task<BiliApiResponse<GetSpaceInfoResponse>> GetSpaceInfo(
        [Query] GetSpaceInfoDto request,
        [Header("Cookie")] string ck
    );
}
