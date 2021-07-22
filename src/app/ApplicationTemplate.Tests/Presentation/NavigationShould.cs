using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public partial class NavigationShould : NavigationTestsBase
	{
		[Fact]
		public async Task NavigateEverywhere()
		{
			await AssertNavigateFromTo<OnboardingPageViewModel, WelcomePageViewModel>(p => p.NavigateToWelcomePage);

			await AssertNavigateFromTo<WelcomePageViewModel, CreateAccountPageViewModel>(p => p.NavigateToCreateAccountPage);

			await AssertNavigateFromTo<CreateAccountPageViewModel, WelcomePageViewModel>(p => p.NavigateBack);

			await AssertNavigateFromTo<WelcomePageViewModel, LoginPageViewModel>(p => p.NavigateToLoginPage);

			await AssertNavigateFromTo<LoginPageViewModel, WelcomePageViewModel>(p => p.NavigateBack);

			await AssertNavigateFromTo<WelcomePageViewModel, HomePageViewModel>(p => p.NavigateToHomePage);

			await AssertNavigateFromTo<HomePageViewModel, PostsPageViewModel>(p => p.NavigateToPostsPage);

			await AssertNavigateFromTo<PostsPageViewModel, EditPostPageViewModel>(p => p.NavigateToNewPost);

			await AssertNavigateFromTo<EditPostPageViewModel, PostsPageViewModel>(p => p.NavigateBack);

			await AssertNavigateFromTo<PostsPageViewModel, EditPostPageViewModel>(p => p.NavigateToNewPost);

			await AssertNavigateFromTo<EditPostPageViewModel, PostsPageViewModel>(p => p.NavigateBack);

			await AssertNavigateFromTo<PostsPageViewModel, HomePageViewModel>(p => p.NavigateBack);

			await AssertNavigateFromTo<HomePageViewModel, SettingsPageViewModel>(p => p.NavigateToSettingsPage);

			await AssertNavigateFromTo<SettingsPageViewModel, DiagnosticsPageViewModel>(p => p.NavigateToDiagnosticsPage);

			await AssertNavigateFromTo<DiagnosticsPageViewModel, SettingsPageViewModel>(p => p.NavigateBack);

			await AssertNavigateFromTo<SettingsPageViewModel, HomePageViewModel>(p => p.NavigateBack);
		}
	}
}
