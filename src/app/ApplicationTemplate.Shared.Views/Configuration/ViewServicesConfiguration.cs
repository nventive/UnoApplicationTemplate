using System.Reactive.Concurrency;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Windows.UI.Core;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace ApplicationTemplate.Views;

/// <summary>
/// This class is used for view services.
/// - Configures view services.
/// </summary>
public static class ViewServicesConfiguration
{
	public static IServiceCollection AddViewServices(this IServiceCollection services)
	{
		return services
			.AddSingleton(s => App.Instance.NavigationMultiFrame.Dispatcher)
			.AddSingleton(s => Shell.Instance.ExtendedSplashScreen)
			.AddSingleton<IDispatcherScheduler>(s => new MainDispatcherScheduler(
				s.GetRequiredService<CoreDispatcher>(),
				CoreDispatcherPriority.Normal
			))
			.AddSingleton<IViewModelViewFactory, ViewModelViewFactory>()
			.AddSingleton<IDiagnosticsService, DiagnosticsService>()
			.AddSingleton<IBrowser>(s => new DispatcherBrowserDecorator(new BrowserImplementation(), App.Instance.Shell.Dispatcher))
			.AddSingleton<IExtendedSplashscreenController, ExtendedSplashscreenController>()
			.AddMessageDialog();
	}

	private static IServiceCollection AddMessageDialog(this IServiceCollection services)
	{
		return services.AddSingleton<IMessageDialogService>(s =>
//-:cnd:noEmit
#if WINDOWS_UWP || __IOS__ || __ANDROID__
//+:cnd:noEmit
			new MessageDialogService.MessageDialogService(
				() => s.GetRequiredService<CoreDispatcher>(),
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key]
				)
			)
//-:cnd:noEmit
#else
//+:cnd:noEmit
			new AcceptOrDefaultMessageDialogService()
//-:cnd:noEmit
#endif
//+:cnd:noEmit
		);
	}
}
