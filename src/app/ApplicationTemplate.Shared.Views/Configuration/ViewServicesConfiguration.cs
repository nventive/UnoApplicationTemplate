using System.Reactive.Concurrency;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Dispatching;
using Windows.UI.Core;

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
			/*
			.AddSingleton<IDispatcherScheduler>(s => new MainDispatcherScheduler(
				Shell.Instance.DispatcherQueue,
				DispatcherQueuePriority.Normal
			))
			*/
			.AddSingleton<IDispatcherFactory, DispatcherFactory>()
			/*
			.AddSingleton<IDiagnosticsService, DiagnosticsService>()
			*/
			.AddSingleton<IBrowserService>(s => new DispatcherBrowserDecorator(new BrowserService(), App.Instance.Shell.DispatcherQueue))
			.AddSingleton<IExtendedSplashscreenController, ExtendedSplashscreenController>(s => new ExtendedSplashscreenController(Shell.Instance.DispatcherQueue))
			.AddSingleton<IConnectivityProvider, ConnectivityProvider>();
			/*
			.AddMessageDialog();
			*/
	}

#if false
	private static IServiceCollection AddMessageDialog(this IServiceCollection services)
	{
		return services.AddSingleton<IMessageDialogService>(s =>
//-:cnd:noEmit
#if WINDOWS || __IOS__ || __ANDROID__
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
#endif
}
