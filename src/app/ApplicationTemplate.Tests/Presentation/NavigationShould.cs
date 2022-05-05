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

			await AssertNavigateFromTo<LoginPageViewModel, DadJokesPageViewModel>(() => new LoginPageViewModel(isFirstLogin: false), p => p.NavigateToHome);

			await AssertNavigateTo<PostsPageViewModel>(() => vmBuilder.ShowPostsSection);

			await AssertNavigateTo<SettingsPageViewModel>(() => vmBuilder.ShowSettingsSection);

			await AssertNavigateTo<DadJokesPageViewModel>(() => vmBuilder.ShowHomeSection);
		}

		[Fact]
		public async Task NavigateFromOnboardingToLoginPage()
		{
			await AssertNavigateFromTo<OnboardingPageViewModel, LoginPageViewModel>(() => new OnboardingPageViewModel(), p => p.NavigateToNextPage);
		}

		[Fact]
		public async Task NavigateFromSettingsToLoginPage()
		{
			// Arrange
			var sourceSection = "Settings";

			// Act
			var currentSection = await AssertSetActiceSection<SettingsPageViewModel, LoginPageViewModel>(() => new SettingsPageViewModel(), p => p.Logout, sourceSection);

			// Assert
			Assert.NotEqual(sourceSection, currentSection);
		}
	}
}
