namespace Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos.VipBigPoint;

public class ReceiveOrCompleteTaskRequest
{
    public ReceiveOrCompleteTaskRequest(string taskCode)
    {
        TaskCode = taskCode;
    }

    public string TaskCode { get; set; }
}
