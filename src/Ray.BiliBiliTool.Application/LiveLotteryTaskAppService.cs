using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ray.BiliBiliTool.Agent;
using Ray.BiliBiliTool.Application.Attributes;
using Ray.BiliBiliTool.Application.Contracts;
using Ray.BiliBiliTool.Application.Diagnostics;
using Ray.BiliBiliTool.Config.Options;
using Ray.BiliBiliTool.DomainService.Interfaces;
using Ray.BiliBiliTool.Infrastructure.Cookie;

namespace Ray.BiliBiliTool.Application;

public class LiveLotteryTaskAppService(
    ILiveDomainService liveDomainService,
    IOptionsMonitor<LiveLotteryTaskOptions> liveLotteryTaskOptions,
    ILogger<LiveLotteryTaskAppService> logger,
    IAccountDomainService accountDomainService,
    ILoginDomainService loginDomainService,
    IConfiguration configuration,
    CookieStrFactory<BiliCookie> cookieStrFactory
)
    : BaseMultiAccountsAppService(logger, cookieStrFactory, loginDomainService, configuration),
        ILiveLotteryTaskAppService
{
    private readonly LiveLotteryTaskOptions _liveLotteryTaskOptions =
        liveLotteryTaskOptions.CurrentValue;

    [TaskInterceptor("��ѡʱ�̳齱", TaskLevel.One)]
    protected override async Task DoTaskAccountAsync(
        BiliCookie ck,
        CancellationToken cancellationToken = default
    )
    {
        await TaskFlowDiagnosticScope.ExecuteAsync(
            logger,
            "��ѡʱ�̳齱",
            async () =>
            {
                if (!liveLotteryTaskOptions.CurrentValue.IsEnable)
                {
                    logger.LogInformation("������Ϊ�رգ�����");
                    return;
                }

                await SetCookiesAsync(ck, cancellationToken);
                await LogUserInfo(ck);
                await LotteryTianXuan(ck);
                await AutoGroupFollowings(ck);
            }
        );
    }

    [TaskInterceptor("��ӡ�û���Ϣ")]
    private async Task LogUserInfo(BiliCookie ck)
    {
        await accountDomainService.LoginByCookie(ck);
    }

    [TaskInterceptor("�齱")]
    private async Task LotteryTianXuan(BiliCookie ck)
    {
        await liveDomainService.TianXuan(ck);
    }

    [TaskInterceptor("�Զ������ע������")]
    private async Task AutoGroupFollowings(BiliCookie ck)
    {
        if (_liveLotteryTaskOptions.AutoGroupFollowings)
        {
            await liveDomainService.GroupFollowing(ck);
        }
        else
        {
            logger.LogInformation("����δ����������");
        }
    }
}
