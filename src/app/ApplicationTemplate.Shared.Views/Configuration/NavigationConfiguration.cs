﻿using System;
using System.Collections.Generic;
using ApplicationTemplate.Presentation;
using ApplicationTemplate.Views.Content;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Views
{
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
			{ typeof(HomePageViewModel), typeof(HomePage) },
			{ typeof(PostsPageViewModel), typeof(PostsPage) },
			{ typeof(EditPostPageViewModel), typeof(EditPostPage) },
			{ typeof(DiagnosticsPageViewModel), typeof(DiagnosticsPage) },
			{ typeof(WelcomePageViewModel), typeof(WelcomePage) },
			{ typeof(CreateAccountPageViewModel), typeof(CreateAccountPage) },
			{ typeof(ForgotPasswordPageViewModel), typeof(ForgotPasswordPage) },
			{ typeof(LoginPageViewModel), typeof(LoginPage) },
			{ typeof(OnboardingPageViewModel), typeof(OnboardingPage) },
			{ typeof(SettingsPageViewModel), typeof(SettingsPage) },
			{ typeof(LicensesPageViewModel), typeof(LicensesPage) },
			{ typeof(WebViewPageViewModel), typeof(WebViewPage) },
			{ typeof(EnvironmentPickerPageViewModel), typeof(EnvironmentPickerPage) },
			{ typeof(EditProfilePageViewModel), typeof(EditProfilePage) },
			{ typeof(ChuckNorrisSearchPageViewModel), typeof(ChuckNorrisSearchPage) },
			{ typeof(ChuckNorrisFavoritesPageViewModel), typeof(ChuckNorrisFavoritesPage) },
		};

		private static void DisableAnimations(FrameSectionsNavigator frameSectionsNavigator)
		{
			frameSectionsNavigator.DefaultSetActiveSectionTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
			frameSectionsNavigator.DefaultOpenModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
			frameSectionsNavigator.DefaultCloseModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
		}
	}
}
