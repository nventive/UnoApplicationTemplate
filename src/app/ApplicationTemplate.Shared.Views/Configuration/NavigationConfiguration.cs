using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Presentation;
using ApplicationTemplate.Views.Content;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
			{ typeof(DadJokesPageViewModel), typeof(DadJokesPage) },
		};

		private static void DisableAnimations(FrameSectionsNavigator frameSectionsNavigator)
		{
			frameSectionsNavigator.DefaultSetActiveSectionTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
			frameSectionsNavigator.DefaultOpenModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
			frameSectionsNavigator.DefaultCloseModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
		}
	}
}
