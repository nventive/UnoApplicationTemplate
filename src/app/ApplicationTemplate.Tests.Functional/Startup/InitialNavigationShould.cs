using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationTemplate.Client;
using ApplicationTemplate.Tests;

namespace Startup;

public class WhenDefaultSettings_InitialNavigationShould : FunctionalTestBase
{
	[Fact]
	public async Task GoToOnboarding()
	{
		// Assert
		ActiveViewModel.Should().BeOfType<OnboardingPageViewModel>();
	}
}

public class WhenOnboardingIsCompleted_InitialNavigationShould : FunctionalTestBase
{
	public override ApplicationSettings ApplicationSettings { get; } = new ApplicationSettings(isOnboardingCompleted: true);

	[Fact]
	public async Task GoToLogin()
	{
		// Assert
		ActiveViewModel.Should().BeOfType<LoginPageViewModel>();
	}
}

public class WhenLoggedIn_InitialNavigationShould : FunctionalTestBase
{
	public override ApplicationSettings ApplicationSettings { get; } = new ApplicationSettings
	{
		IsOnboardingCompleted = true,
		AuthenticationData = new AuthenticationData()
	};

	[Fact]
	public async Task GoToDadJokes()
	{
		// Assert
		ActiveViewModel.Should().BeOfType<DadJokesPageViewModel>();
	}
}
