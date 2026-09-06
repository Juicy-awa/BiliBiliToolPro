namespace Ray.BiliBiliTool.Agent.QingLong.Dtos;

public class QingLongGenericResponse<T>
{
    public int Code { get; set; }

    /// <summary>ql OpenAPI 错误信息（code!=200 时返回）</summary>
    public string? Message { get; set; }

    public T? Data { get; set; }
}
