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

			await AssertNavigateFromTo<OnboardingPageViewModel, LoginPageViewModel>(() => new OnboardingPageViewModel(), p => p.NavigateToLogin);

			await AssertNavigateTo<PostsPageViewModel>(() => vmBuilder.ShowPostsSection);

			await AssertNavigateTo<SettingsPageViewModel>(() => vmBuilder.ShowSettingsSection);

			await AssertNavigateTo<DadJokesPageViewModel>(() => vmBuilder.ShowHomeSection);
		}

		[Fact]
		public async Task NavigateFromOnboardingToLoginPage()
		{
			await AssertNavigateFromTo<OnboardingPageViewModel, LoginPageViewModel>(() => new OnboardingPageViewModel(), p => p.NavigateToLogin);
		}

		[Fact]
		public async Task NavigateFromSettingsToLoginPage()
		{
			await AssertNavigateFromTo<SettingsPageViewModel, LoginPageViewModel>(() => new SettingsPageViewModel(), p => p.NavigateToLoginPage);
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
