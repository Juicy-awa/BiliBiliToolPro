using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ray.BiliBiliTool.Agent;
using Ray.BiliBiliTool.Agent.BiliBiliAgent.Dtos;
using Ray.BiliBiliTool.Application.Attributes;
using Ray.BiliBiliTool.Application.Contracts;
using Ray.BiliBiliTool.Application.Diagnostics;
using Ray.BiliBiliTool.Config.Options;
using Ray.BiliBiliTool.DomainService.Interfaces;
using Ray.BiliBiliTool.Infrastructure.Cookie;

namespace Ray.BiliBiliTool.Application;

public class VipPrivilegeTaskAppService(
    ILogger<VipPrivilegeTaskAppService> logger,
    IOptionsMonitor<VipPrivilegeOptions> vipPrivilegeOptions,
    IAccountDomainService accountDomainService,
    IVipPrivilegeDomainService vipPrivilegeDomainService,
    ILoginDomainService loginDomainService,
    IConfiguration configuration,
    CookieStrFactory<BiliCookie> cookieStrFactory
)
    : BaseMultiAccountsAppService(logger, cookieStrFactory, loginDomainService, configuration),
        IVipPrivilegeTaskAppService
{
    [TaskInterceptor("��ȡ���Ա��������", TaskLevel.One)]
    protected override async Task DoTaskAccountAsync(
        BiliCookie ck,
        CancellationToken cancellationToken = default
    )
    {
        await TaskFlowDiagnosticScope.ExecuteAsync(
            logger,
            "���Ա��������",
            async () =>
            {
                if (!vipPrivilegeOptions.CurrentValue.IsEnable)
                {
                    logger.LogInformation("������Ϊ�رգ�����");
                    return;
                }

                await SetCookiesAsync(ck, cancellationToken);
                UserInfo userInfo = await Login(ck);

                await ReceiveVipPrivilege(userInfo, ck);
            }
        );
    }

    /// <summary>
    /// ��¼
    /// </summary>
    /// <returns></returns>
    [TaskInterceptor("��¼")]
    private async Task<UserInfo> Login(BiliCookie ck)
    {
        UserInfo userInfo = await accountDomainService.LoginByCookie(ck);
        return userInfo;
    }

    /// <summary>
    /// ÿ����ȡ���Ա����
    /// </summary>
    [TaskInterceptor("��ȡ", rethrowWhenException: false)]
    private async Task ReceiveVipPrivilege(UserInfo userInfo, BiliCookie ck)
    {
        var suc = await vipPrivilegeDomainService.ReceiveVipPrivilege(userInfo, ck);

        //�����ȡ�ɹ�����Ҫˢ���˻���Ϣ������B����
        if (suc)
        {
            try
            {
                await accountDomainService.LoginByCookie(ck);
            }
            catch (Exception ex)
            {
                logger.LogError("��ȡ�����ɹ�����֮��ˢ���û���Ϣʱ�쳣����Ϣ��{msg}", ex.Message);
            }
        }
    }
}
