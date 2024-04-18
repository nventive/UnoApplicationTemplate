using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationTemplate.Tests;

namespace Authentication;

public sealed class AuthenticationShould : FunctionalTestBase
{
	[Fact]
	public async Task NavigateToDadJokes_WhenLoggingIn()
	{
		// Arrange
		var vm = await this.ReachLoginPage();

		// Act
		await this.Login(vm);

		// Assert
		ActiveViewModel.Should().BeOfType<DadJokesPageViewModel>();
	}

	[Fact]
	public async Task NavigateToLoginPage_WhenLoggingOutFromSettingsPage()
	{
		// Arrange
		await this.Login(await this.ReachLoginPage());

		await Menu.ShowSettingsSection.Execute();
		var settingsVm = GetAndAssertActiveViewModel<SettingsPageViewModel>();

		// Act
		await settingsVm.Logout.Execute();

		// Assert
		ActiveViewModel.Should().BeOfType<LoginPageViewModel>();
	}
}
