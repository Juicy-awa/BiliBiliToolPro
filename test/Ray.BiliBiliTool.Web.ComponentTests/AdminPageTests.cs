using System.Security.Claims;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Ray.BiliBiliTool.Web.Components.Pages;
using Ray.BiliBiliTool.Web.Services;
using Xunit;

namespace Ray.BiliBiliTool.Web.ComponentTests;

/// <summary>
/// Baseline component tests for the Admin page.
/// Pin current observable behavior — username loading and client-side
/// validation branches — before Phase 14 migrates the change-password
/// orchestration behind <c>IAdminPageWorkflow</c>.
/// </summary>
public class AdminPageTests : TestContext
{
    private const string TestUsername = "testadmin";

    public AdminPageTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAuthService>(new FakeAuthService(TestUsername));
    }

    [Fact]
    public void Admin_OnInitialized_DisplaysUsernameFromAuthService()
    {
        var cut = RenderComponent<Admin>();

        // The username field is bound to _username which is populated from IAuthService
        cut.Markup.Should().Contain(TestUsername);
    }

    [Fact]
    public void Admin_SubmitWithAllFieldsEmpty_ShowsEmptyPasswordError()
    {
        var cut = RenderComponent<Admin>();

        // All fields default to ""; _newPassword == _confirmPassword passes the mismatch check,
        // then IsNullOrWhiteSpace("") == true triggers the empty-password branch.
        cut.Find("button.mud-button-filled").Click();

        cut.Markup.Should().Contain("Password cannot be empty");
    }

    [Fact]
    public void Admin_RendersExpectedNumberOfInputFields()
    {
        var cut = RenderComponent<Admin>();

        // Admin form: username, current password, new password, confirm password
        cut.FindAll("input").Count.Should().BeGreaterThanOrEqualTo(4);
    }

    private sealed class FakeAuthService(string username) : IAuthService
    {
        public Task<ClaimsIdentity> LoginAsync(string u, string p) =>
            Task.FromResult(new ClaimsIdentity());

        public Task ChangePasswordAsync(string u, string c, string n) => Task.CompletedTask;

        public Task<string> GetAdminUserNameAsync() => Task.FromResult(username);
    }
}
