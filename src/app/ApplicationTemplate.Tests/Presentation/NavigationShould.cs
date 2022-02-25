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
		public async Task MenuNavigationShould()
		{
			// Arrange
			Func<MenuViewModel> vmBuilder = () => new MenuViewModel();

			await AssertNavigateFromTo<MenuViewModel, SettingsPageViewModel>(vmBuilder, p => p.ShowSettings);

			await AssertNavigateFromTo<MenuViewModel, PostsPageViewModel>(vmBuilder, p => p.ShowPosts);

			await AssertNavigateFromTo<MenuViewModel, DadJokesPageViewModel>(vmBuilder, p => p.ShowHome);
		}

		[Fact]
		public async Task OnboardingAndWelcomePageNavigationShould()
		{
			await AssertNavigateFromTo<WelcomePageViewModel, OnboardingPageViewModel>(() => new WelcomePageViewModel(), p => p.NavigateToOnboarding);

			await AssertNavigateFromTo<OnboardingPageViewModel, DadJokesPageViewModel>(() => new OnboardingPageViewModel(), p => p.NavigateToJokes);
		}

		[Fact]
		public async Task LoginNavigationShould()
		{
			var loginVM = await AssertNavigateFromTo<SettingsPageViewModel, LoginPageViewModel>(() => new SettingsPageViewModel(), p => p.NavigateToLoginPage);

			await AssertNavigateTo<SettingsPageViewModel>(() => loginVM.NavigateBack);
		}

		[Fact]
		public async Task SettingsNavigationShould()
		{
			// Arrange
			Func<SettingsPageViewModel> vmBuilder = () => new SettingsPageViewModel();
			var settingsVM = (SettingsPageViewModel) await NavigateAndClear(DefaultCancellationToken, vmBuilder);

			// Act and assert
			var diagnosticsVM = await AssertNavigateTo<DiagnosticsPageViewModel>(() => settingsVM.NavigateToDiagnosticsPage);

			settingsVM = await AssertNavigateTo<SettingsPageViewModel>(() => diagnosticsVM.NavigateBack);
		}

		[Fact]
		public async Task DiagnosticsNavigationShould()
		{
			// Arrange
			var settingsViewModel = (SettingsPageViewModel) await NavigateAndClear(DefaultCancellationToken, () => new SettingsPageViewModel());

			// Act and assert
			await AssertNavigateFromToAfter<DiagnosticsPageViewModel, SettingsPageViewModel>(
				() => settingsViewModel.NavigateToDiagnosticsPage,
				p => p.NavigateBack
			);
		}
	}
}
