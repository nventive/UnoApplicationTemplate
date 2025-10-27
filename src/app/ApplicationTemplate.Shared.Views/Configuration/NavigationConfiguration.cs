using System;
using System.Collections.Generic;
using ApplicationTemplate.Presentation;
using ApplicationTemplate.Presentation.ViewModels.Agentic;
using ApplicationTemplate.Views.Content;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Views;

/// <summary>
/// This class is used for navigation configuration.
/// - Configures the navigator.
/// </summary>
public static class NavigationConfiguration
{
	public static IServiceCollection AddNavigation(this IServiceCollection services)
	{
		return services.AddSingleton<ISectionsNavigator>(s =>
			new FrameSectionsNavigator(
				App.Instance.NavigationMultiFrame,
				GetPageRegistrations()
			)
		);
	}

	private static IReadOnlyDictionary<Type, Type> GetPageRegistrations() => new Dictionary<Type, Type>()
	{
		// TODO: Add your ViewModel and Page associations here.
		{ typeof(WelcomePageViewModel), typeof(WelcomePage) },
		{ typeof(PostsPageViewModel), typeof(PostsPage) },
		{ typeof(EditPostPageViewModel), typeof(EditPostPage) },
		{ typeof(DiagnosticsPageViewModel), typeof(DiagnosticsPage) },
		{ typeof(CreateAccountPageViewModel), typeof(CreateAccountPage) },
		{ typeof(ForgotPasswordPageViewModel), typeof(ForgotPasswordPage) },
		{ typeof(LoginPageViewModel), typeof(LoginPage) },
		{ typeof(OnboardingPageViewModel), typeof(OnboardingPage) },
		{ typeof(SettingsPageViewModel), typeof(SettingsPage) },
		{ typeof(LicensesPageViewModel), typeof(LicensesPage) },
		{ typeof(EnvironmentPickerPageViewModel), typeof(EnvironmentPickerPage) },
		{ typeof(EditProfilePageViewModel), typeof(EditProfilePage) },
		{ typeof(DadJokesPageViewModel), typeof(DadJokesPage) },
		{ typeof(DadJokesFiltersPageViewModel), typeof(DadJokesFiltersPage) },
		{ typeof(SentEmailConfirmationPageViewModel), typeof(SentEmailConfirmationPage) },
		{ typeof(ResetPasswordPageViewModel), typeof(ResetPasswordPage) },
		{ typeof(ForcedUpdatePageViewModel), typeof(ForcedUpdatePage) },
		{ typeof(KillSwitchPageViewModel), typeof(KillSwitchPage) },
		{ typeof(AgenticChatPageViewModel), typeof(AgenticChatPage) },
		{ typeof(DrawingModalViewModel), typeof(DrawingModalPage) },
	};

	/// <summary>
	/// Disable navigation animations.
	/// </summary>
	/// <remarks>
	/// Do not remove even if it's not used by default.
	/// </remarks>
	/// <param name="frameSectionsNavigator"><see cref="FrameSectionsNavigator"/>.</param>
	private static void DisableAnimations(FrameSectionsNavigator frameSectionsNavigator)
	{
		frameSectionsNavigator.DefaultSetActiveSectionTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
		frameSectionsNavigator.DefaultOpenModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
		frameSectionsNavigator.DefaultCloseModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
	}
}
