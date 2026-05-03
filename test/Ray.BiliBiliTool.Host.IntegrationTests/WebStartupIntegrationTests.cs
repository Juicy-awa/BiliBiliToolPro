using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Ray.BiliBiliTool.Application.Contracts;
using Ray.BiliBiliTool.Config.Options;
using Ray.BiliBiliTool.Host.IntegrationTests.Support;
using Ray.BiliBiliTool.Infrastructure.EF;

namespace Ray.BiliBiliTool.Host.IntegrationTests;

public class WebStartupIntegrationTests
{
    [Fact]
    public Task Web_startup_boots_and_exposes_critical_services()
    {
        using var factory = new WebHostFactory();
        using var scope = factory.Services.CreateScope();

        scope.ServiceProvider.GetRequiredService<ILoginTaskAppService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IDailyTaskAppService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<ISchedulerFactory>().Should().NotBeNull();
        scope
            .ServiceProvider.GetRequiredService<IOptionsMonitor<DailyTaskOptions>>()
            .Should()
            .NotBeNull();
        scope.ServiceProvider.GetRequiredService<DbInitializer>().Should().NotBeNull();

        var dbContext = scope.ServiceProvider.GetRequiredService<BiliDbContext>();
        dbContext.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.Sqlite");

        return Task.CompletedTask;
    }
}
