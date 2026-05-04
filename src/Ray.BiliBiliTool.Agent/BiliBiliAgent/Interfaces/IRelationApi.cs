using System.ComponentModel;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.Relation;
using Refit;

namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Interfaces;

/// <summary>
/// 关注相关接口
/// </summary>
[Headers("Host: api.bilibili.com", "Referer: https://space.bilibili.com/")]
public interface IRelationApi
{
    /// <summary>
    /// 获取关注列表
    /// </summary>
    /// <returns></returns>
    [Get("/x/relation/followings")]
    Task<BiliApiResponse<GetFollowingsResponse>> GetFollowings(
        [Query] GetFollowingsRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 获取特别关注列表
    /// </summary>
    /// <returns></returns>
    [Headers("Cache-Control: no-cache", "Pragma: no-cache")]
    [Get("/x/relation/tag")]
    Task<BiliApiResponse<List<UpInfo>>> GetFollowingsByTag(
        [Query] GetSpecialFollowingsRequest request,
        [Header("Cookie")] string ck
    );

    /// <summary>
    /// 获取关注分组
    /// </summary>
    /// <returns></returns>
    [Headers("Sec-Fetch-Mode: no-cors", "Sec-Fetch-Dest: script")]
    [Get("/x/relation/tags?jsonp=jsonp")]
    Task<BiliApiResponse<List<TagDto>>> GetTags(
        [Header("Cookie")] string ck,
        [Header("Referer")] string referer = RelationApiConstant.GetTagsReferer
    );

    /// <summary>
    /// 添加关注分组（tag）
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Headers("Origin: https://space.bilibili.com")]
    [Post("/x/relation/tag/create?cross_domain=true")]
    Task<BiliApiResponse<CreateTagResponse>> CreateTag(
        [Body(BodySerializationMethod.UrlEncoded)] CreateTagRequest request,
        [Header("Cookie")] string ck,
        [Header("Referer")] string referer = RelationApiConstant.GetTagsReferer
    );

    /// <summary>
    /// 批量拷贝关注up到某指定分组
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Headers("Origin: https://space.bilibili.com")]
    [Post("/x/relation/tags/copyUsers")]
    Task<BiliApiResponse> CopyUpsToGroup(
        [Body(BodySerializationMethod.UrlEncoded)] CopyUserToGroupRequest request,
        [Header("Cookie")] string ck,
        [Header("Referer")] string referer = RelationApiConstant.CopyReferer
    );

    /// <summary>
    /// 修改关系
    /// </summary>
    /// <returns></returns>
    [Headers("Origin: https://space.bilibili.com")]
    [Post("/x/relation/modify")]
    Task<BiliApiResponse> ModifyRelation(
        [Body(BodySerializationMethod.UrlEncoded)] ModifyRelationRequest request,
        [Header("Cookie")] string ck,
        [Header("Referer")] string referer = RelationApiConstant.ModifyReferer
    );
}

public enum FollowingsOrderType
{
    /// <summary>
    /// 最常访问频率倒序
    /// </summary>
    [DefaultValue("attention")]
    AttentionDesc,

    /// <summary>
    /// 关注时间倒序
    /// </summary>
    [DefaultValue("")]
    TimeDesc,
}

public class RelationApiConstant
{
    /// <summary>
    /// GetTags接口中的Referer
    /// {0}为UserId
    /// </summary>
    public const string GetTagsReferer = "https://space.bilibili.com/{0}/fans/follow";

    /// <summary>
    /// CopyUpsToGroup接口中的Referer
    /// {0}为UserId
    /// </summary>
    public const string CopyReferer = "https://space.bilibili.com/{0}/fans/follow?tagid=-1";

    /// <summary>
    /// ModifyRelation接口种的Referer
    /// </summary>
    public const string ModifyReferer = "https://space.bilibili.com/{0}/fans/follow?tagid={1}";
}
