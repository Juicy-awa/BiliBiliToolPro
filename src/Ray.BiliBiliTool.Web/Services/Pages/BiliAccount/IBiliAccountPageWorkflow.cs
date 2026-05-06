namespace Ray.BiliBiliTool.Web.Services.Pages.BiliAccount;

public interface IBiliAccountPageWorkflow
{
    Task<List<BiliAccountDto>> GetAllAccountsAsync();
    Task AddAsync(string cookieStr);
    Task UpdateAsync(int index, string cookieStr);
    Task DeleteAsync(int index);
    Task ReorderAsync(int fromIndex, int toIndex);
}
