using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Chinook.SectionsNavigation;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public partial class NavigationShould : NavigationTestsBase
	{
		[Fact]
		public async Task NavigateToDifferentMenuSections()
		{
			// Arrange
			MenuViewModel vmBuilder = new MenuViewModel();

			var menuViewModel = await AssertNavigateFromTo<OnboardingPageViewModel, DadJokesPageViewModel>(
				() => new OnboardingPageViewModel(),
				p => p.NavigateToJokes
			);

			await AssertNavigateTo<PostsPageViewModel>(() => vmBuilder.ShowPostsSection);

			await AssertNavigateTo<SettingsPageViewModel>(() => vmBuilder.ShowSettingsSection);

			await AssertNavigateTo<DadJokesPageViewModel>(() => vmBuilder.ShowHomeSection);
		}

		[Fact]
		public async Task NavigateFromWelcomePageToDadJokesPage()
		{
			var onboardingViewModel = await AssertNavigateFromTo<WelcomePageViewModel, OnboardingPageViewModel>(() => new WelcomePageViewModel(), p => p.NavigateToOnboarding);

			await AssertNavigateTo<DadJokesPageViewModel>(() => onboardingViewModel.NavigateToJokes);
		}

		[Fact]
		public async Task NavigateToLoginPageAndBack()
		{
			var loginVM = await AssertNavigateFromTo<SettingsPageViewModel, LoginPageViewModel>(() => new SettingsPageViewModel(), p => p.NavigateToLoginPage);

			await AssertNavigateTo<SettingsPageViewModel>(() => loginVM.NavigateBack);
		}

		[Fact]
		public async Task NavigateToDiagnosticsPageAndBack()
		{
			var diagnosticsViewModel = await AssertNavigateFromTo<SettingsPageViewModel, DiagnosticsPageViewModel>(
				() => new SettingsPageViewModel(),
				p => p.NavigateToDiagnosticsPage
			);

			await AssertNavigateTo<SettingsPageViewModel>(() => diagnosticsViewModel.NavigateBack);
		}
	}
}
