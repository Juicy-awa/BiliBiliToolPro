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
public interface IUserInfoApi
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    [Get("/x/web-interface/nav")]
    Task<BiliApiResponse<UserInfo>> LoginByCookie([Header("Cookie")] string ck);
}
