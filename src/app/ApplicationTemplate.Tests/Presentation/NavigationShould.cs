using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Chinook.SectionsNavigation;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTemplate.Tests;

public partial class NavigationShould : NavigationTestsBase
{
	private readonly ITestOutputHelper _testOutputHelper;

	public NavigationShould(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

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
		var currentSection = await AssertSetActiveSection<SettingsPageViewModel, LoginPageViewModel>(() => new SettingsPageViewModel(), p => p.Logout, sourceSection);

		// Assert
		Assert.NotEqual(sourceSection, currentSection);
	}

	[Fact]
	public async Task NavigateToDiagnosticsPageAndBack()
	{
		var diagnosticsViewModel = await AssertNavigateFromTo<SettingsPageViewModel, DiagnosticsPageViewModel>(() => new SettingsPageViewModel(), p => p.NavigateToDiagnosticsPage);

		await AssertNavigateTo<SettingsPageViewModel>(() => diagnosticsViewModel.NavigateBack);
	}

	[Fact]
	// From LoginPage to DadJokesPage
	public async Task NavigateFromLoginToDadJokesPage()
	{
		// Arrange
		var sourceSection = "Login";

		// Act
		var currentSection = await AssertSetActiveSection<LoginPageViewModel, DadJokesPageViewModel>(() => new LoginPageViewModel(false), p => p.NavigateToHome, sourceSection);

		// Assert
		Assert.NotEqual(sourceSection, currentSection);
		Assert.Equal("Home", currentSection);
	}

	[Fact]
	// From LoginPage to CreateAccountPage
	public async Task NavigateFromLoginToCreateAccountPage()
	{
		// Act
		var currentSection = await AssertNavigateFromTo<LoginPageViewModel, CreateAccountPageViewModel>(() => new LoginPageViewModel(false), p => p.NavigateToCreateAccountPage);

		// Assert
		Assert.IsType<CreateAccountPageViewModel>(currentSection);
	}

	[Fact]
	// From LoginPage to ForgotPasswordPage
	public async Task NavigateFromLoginToForgotPasswordPage()
	{
		// Act
		var currentSection = await AssertNavigateFromTo<LoginPageViewModel, ForgotPasswordPageViewModel>(() => new LoginPageViewModel(false), p => p.NavigateToForgotPasswordPage);

		// Assert
		Assert.IsType<ForgotPasswordPageViewModel>(currentSection);
	}

	[Fact]
	// From CreateAccountPage to DadJokesPage
	public async Task NavigateFromCreateAccountToDadJokesPage()
	{
		// TODO: The form has to be validated before we can navigate

		// Act
		var currentSection = await AssertNavigateFromTo<CreateAccountPageViewModel, DadJokesPageViewModel>(() => new CreateAccountPageViewModel(), p => p.CreateAccount);

		// Assert
		Assert.IsType(typeof(DadJokesPageViewModel), currentSection);
	}

	[Fact]
	// From CreateAccountPage to BackPage
	public async Task NavigateToCreateAccountAndBack()
	{
		var createAccountViewModel = await AssertNavigateFromTo<LoginPageViewModel, CreateAccountPageViewModel>(() => new LoginPageViewModel(false), p => p.NavigateToCreateAccountPage);

		await AssertNavigateTo<LoginPageViewModel>(() => createAccountViewModel.NavigateBack);
	}
}
