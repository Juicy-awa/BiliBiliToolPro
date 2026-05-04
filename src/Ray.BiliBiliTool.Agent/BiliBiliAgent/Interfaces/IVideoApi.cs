using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.Video;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

/// <summary>
/// 视频相关接口
/// </summary>
[Headers("Host: api.bilibili.com")]
public interface IVideoApi
{
    /// <summary>
    /// 分享视频
    /// </summary>
    /// <param name="request"></param>
    /// <remarks>ck中必须要有buvid3，否则几率性-403</remarks>
    /// <returns></returns>
    [Headers("Origin: https://www.bilibili.com")]
    [Post("/x/web-interface/share/add")]
    Task<BiliApiResponse> ShareVideo(
        [Body(BodySerializationMethod.UrlEncoded)] ShareVideoRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 上传视频观看进度
    /// 每15秒上报一次
    /// </summary>
    /// <returns></returns>
    //[Header("Content-Length", "186")]
    [Headers(
        "Content-Type: application/x-www-form-urlencoded; charset=UTF-8",
        "Referer: https://www.bilibili.com/",
        "Origin: https://www.bilibili.com"
    )]
    [Post("/x/click-interface/web/heartbeat?aid={aid}&played_time={playedTime}")]
    Task<BiliApiResponse> UploadVideoHeartbeat(
        [Body(BodySerializationMethod.UrlEncoded)] UploadVideoHeartbeatRequest request,
        [Header("Cookie")] string ck
    );

    #region 投币相关
    /// <summary>
    /// 为视频投币
    /// </summary>
    /// <param name="aid"></param>
    /// <param name="multiply"></param>
    /// <param name="select_like"></param>
    /// <param name="csrf"></param>
    /// <returns></returns>
    [Headers("Content-Type: application/x-www-form-urlencoded", "Origin: https://www.bilibili.com")]
    //[Headers("Referer: https://www.bilibili.com/")]
    [Post("/x/web-interface/coin/add")]
    Task<BiliApiResponse> AddCoinForVideo(
        [Body(BodySerializationMethod.UrlEncoded)] AddCoinRequest request,
        [Header("Cookie")] string ck,
        [Header("referer")]
            string refer =
            "https://www.bilibili.com/video/BV123456/?spm_id_from=333.1007.tianma.1-1-1.click&vd_source=80c1601a7003934e7a90709c18dfcffd"
    );

    /// <summary>
    /// 获取当前用户对<paramref name="aid"/>视频的投币信息
    /// </summary>
    /// <param name="aid"></param>
    /// <returns></returns>
    [Headers("Referer: https://www.bilibili.com/")]
    [Get("/x/web-interface/archive/coins")]
    Task<BiliApiResponse<DonatedCoinsForVideo>> GetDonatedCoinsForVideo(
        [Query] GetAlreadyDonatedCoinsRequest request,
        [Header("Cookie")] string ck
    );
    #endregion

    /// <summary>
    /// 搜索指定Up的视频列表
    /// </summary>
    /// <param name="upId"></param>
    /// <param name="pageSize">[1,100]验证不通过接口会报异常</param>
    /// <param name="pageNumber"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    [Headers("Referer: https://www.bilibili.com/", "Origin: https://space.bilibili.com")]
    //[Get("/x/space/wbi/arc/search?mid={upId}&ps={pageSize}&tid=0&pn={pageNumber}&keyword={keyword}&order=pubdate&platform=web&web_location=1550101&order_avoided=true&w_rid=5df06b1c48e2be86a96e9d0f99bf06f4&wts=1684854929")]
    [Get("/x/space/wbi/arc/search")]
    Task<BiliApiResponse<SearchUpVideosResponse>> SearchVideosByUpId(
        [Query] SearchVideosByUpIdDto request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 通过ssid获取番剧的具体信息
    /// </summary>
    /// <param name="Ssid"></param>
    /// <returns></returns>
    [Get("/pgc/view/web/season?season_id={ssid}")]
    Task<GetBangumiBySsidResponse> GetBangumiBySsid(long ssid, [Header("Cookie")] string ck);
}

/// <summary>
/// 不需要传递Cookie的接口
/// </summary>
public interface IVideoWithoutCookieApi : IVideoApi
{
    /// <summary>
    /// 获取视频详情
    /// </summary>
    /// <param name="aid"></param>
    /// <returns></returns>
    [Get("/x/web-interface/view?aid={aid}")]
    Task<BiliApiResponse<VideoDetail>> GetVideoDetail(string aid);

    /// <summary>
    /// 获取某分区下X日内排行榜
    /// </summary>
    /// <param name="rid"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    [Headers("Referer: https://www.bilibili.com/", "Origin: https://www.bilibili.com")]
    [Get("/x/web-interface/ranking/region?rid={rid}&day={day}")]
    [Obsolete]
    Task<BiliApiResponse<List<RankingInfo>>> GetRegionRankingVideos(int rid, int day);

    /// <summary>
    /// 获取排行榜
    /// </summary>
    /// <returns></returns>
    [Headers("Referer: https://www.bilibili.com/", "Origin: https://www.bilibili.com", "dnt: 1")]
    [Get("/x/web-interface/ranking/v2?rid=0&type=all")]
    Task<BiliApiResponse<Ranking>> GetRegionRankingVideosV2();
}
