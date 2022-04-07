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
			Func<MenuViewModel> vmBuilder = () => new MenuViewModel();

			await AssertNavigateFromTo<MenuViewModel, SettingsPageViewModel>(p => p.ShowSettingsSection);

			await AssertNavigateFromTo<MenuViewModel, PostsPageViewModel>(p => p.ShowPostsSection);

			await AssertNavigateFromTo<MenuViewModel, DadJokesPageViewModel>(p => p.ShowHomeSection);
		}

		[Fact]
		public async Task NavigateFromWelcomePageToDadJokesPage()
		{
			var onboardingViewModel = await AssertNavigateFromTo<WelcomePageViewModel, OnboardingPageViewModel>(p => p.NavigateToOnboarding);

			await AssertNavigateTo<DadJokesPageViewModel>(() => onboardingViewModel.NavigateToJokes);
		}

		[Fact]
		public async Task NavigateToLoginPageAndBack()
		{
			var loginVM = await AssertNavigateFromTo<SettingsPageViewModel, LoginPageViewModel>(p => p.NavigateToLoginPage);

			await AssertNavigateTo<SettingsPageViewModel>(() => loginVM.NavigateBack);
		}

		[Fact]
		public async Task NavigateToDiagnosticsPageAndBack()
		{
			var diagnosticsViewModel = await AssertNavigateFromTo<SettingsPageViewModel, DiagnosticsPageViewModel>(
				p => p.NavigateToDiagnosticsPage
			);

			await AssertNavigateTo<SettingsPageViewModel>(() => diagnosticsViewModel.NavigateBack);
		}
	}
}
