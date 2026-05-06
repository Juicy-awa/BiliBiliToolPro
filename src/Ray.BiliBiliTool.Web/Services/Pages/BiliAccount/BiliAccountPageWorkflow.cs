using Microsoft.Extensions.Configuration;

namespace Ray.BiliBiliTool.Web.Services.Pages.BiliAccount;

public class BiliAccountPageWorkflow(IConfiguration configuration) : IBiliAccountPageWorkflow
{
    public Task<List<BiliAccountDto>> GetAllAccountsAsync()
    {
        var cookieList = configuration.GetSection("BiliBiliCookies").Get<List<string>>() ?? [];
        var accounts = new List<BiliAccountDto>();

        for (int i = 0; i < cookieList.Count; i++)
        {
            var cookieStr = cookieList[i];
            var userId = ParseUserId(cookieStr);
            accounts.Add(new BiliAccountDto(i, userId, cookieStr));
        }

        return Task.FromResult(accounts);
    }

    private static string ParseUserId(string cookieStr)
    {
        try
        {
            var items = cookieStr.Split(";", StringSplitOptions.TrimEntries);
            foreach (var item in items)
            {
                var eqIndex = item.IndexOf("=", StringComparison.Ordinal);
                if (eqIndex <= 0)
                    continue;

                var key = item[..eqIndex].Trim();
                var value = item[(eqIndex + 1)..].Trim();

                if (key == "DedeUserID" && !string.IsNullOrEmpty(value))
                    return value;
            }
        }
        catch
        {
            // Parsing failed — fall through to unknown
        }

        return "(unknown)";
    }
}
