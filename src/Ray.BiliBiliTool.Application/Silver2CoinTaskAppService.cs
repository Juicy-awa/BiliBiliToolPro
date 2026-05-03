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

public class Silver2CoinTaskAppService(
    ILogger<Silver2CoinTaskAppService> logger,
    IOptionsMonitor<Silver2CoinTaskOptions> silver2CoinTaskOptions,
    IAccountDomainService accountDomainService,
    ILoginDomainService loginDomainService,
    IConfiguration configuration,
    ILiveDomainService liveDomainService,
    ICoinDomainService coinDomainService,
    CookieStrFactory<BiliCookie> cookieStrFactory
)
    : BaseMultiAccountsAppService(logger, cookieStrFactory, loginDomainService, configuration),
        ISilver2CoinTaskAppService
{
    [TaskInterceptor("�����Ӷһ�Ӳ������", TaskLevel.One)]
    protected override async Task DoTaskAccountAsync(
        BiliCookie ck,
        CancellationToken cancellationToken = default
    )
    {
        await TaskFlowDiagnosticScope.ExecuteAsync(
            logger,
            "�����Ӷһ�Ӳ������",
            async () =>
            {
                if (!silver2CoinTaskOptions.CurrentValue.IsEnable)
                {
                    logger.LogInformation("������Ϊ�رգ�����");
                    return;
                }

                await SetCookiesAsync(ck, cancellationToken);
                await Login(ck);

                await ExchangeSilver2Coin(ck);
            }
        );
    }

    /// <summary>
    /// ��¼
    /// </summary>
    /// <returns></returns>
    [TaskInterceptor("��¼")]
    private async Task Login(BiliCookie ck)
    {
        await accountDomainService.LoginByCookie(ck);
    }

    /// <summary>
    /// ֱ�����ĵ������Ӷһ�Ӳ��
    /// </summary>
    [TaskInterceptor("�����Ӷһ�Ӳ��", rethrowWhenException: false)]
    private async Task ExchangeSilver2Coin(BiliCookie ck)
    {
        var success = await liveDomainService.ExchangeSilver2Coin(ck);
        if (!success)
            return;

        //����һ��ɹ������ӡӲ�����
        var coinBalance = coinDomainService.GetCoinBalance(ck);
        logger.LogInformation("��Ӳ���� {coin}", coinBalance);
    }
}
