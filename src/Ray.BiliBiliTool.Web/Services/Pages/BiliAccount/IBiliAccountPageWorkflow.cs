namespace Ray.BiliBiliTool.Web.Services.Pages.BiliAccount;

public interface IBiliAccountPageWorkflow
{
    Task<List<BiliAccountDto>> GetAllAccountsAsync();
}
