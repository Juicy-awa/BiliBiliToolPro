using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.Article;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

[Headers("Host: api.bilibili.com")]
public interface IArticleApi
{
    [Headers("Referer: https://www.bilibili.com/", "Origin: https://space.bilibili.com")]
    [Get("/x/space/wbi/article")]
    Task<BiliApiResponse<SearchUpArticlesResponse>> SearchUpArticlesByUpIdAsync(
        [Query] SearchArticlesByUpIdDto request
    );

    /// <summary>
    /// 获取专栏详情
    /// </summary>
    /// <param name="cvid"></param>
    /// <returns></returns>
    [Get("/x/article/viewinfo?id={cvid}")]
    Task<BiliApiResponse<SearchArticleInfoResponse>> SearchArticleInfoAsync(long cvid);

    /// <summary>
    /// 为专栏文章投币
    /// </summary>
    /// <param name="request"></param>
    /// <param name="refer"></param>
    /// <returns></returns>
    [Headers("Content-Type: application/x-www-form-urlencoded", "Origin: https://www.bilibili.com")]
    [Post("/x/web-interface/coin/add")]
    Task<BiliApiResponse> AddCoinForArticleAsync(
        [Body(BodySerializationMethod.UrlEncoded)] AddCoinForArticleRequest request,
        [Header("Cookie")] string ck,
        [Header("referer")]
            string refer =
            "https://www.bilibili.com/read/cv5806746/?from=search&spm_id_from=333.337.0.0"
    );

    /// <summary>
    /// 为专栏文章点赞
    /// </summary>
    /// <param name="cvid"></param>
    /// <param name="csrf"></param>
    /// <returns></returns>
    [Headers(
        "Content-Type: application/x-www-form-urlencoded",
        "Referer: https://www.bilibili.com/read/cv{cvid}/?from=search&spm_id_from=333.337.0.0",
        "Origin: https://www.bilibili.com"
    )]
    [Post("/x/article/like?id={cvid}&type=1&csrf={csrf}")]
    Task<BiliApiResponse> LikeAsync(long cvid, string csrf, [Header("Cookie")] string ck);
}
