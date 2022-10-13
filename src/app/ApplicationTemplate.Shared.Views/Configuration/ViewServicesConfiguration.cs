using System;
using System.Reactive;
using System.Reactive.Concurrency;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
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
				s.GetRequiredService<DispatcherQueue>(),
				DispatcherQueuePriority.Normal
			))
			.AddSingleton<IDispatcherFactory, DispatcherFactory>()
			.AddSingleton<IDiagnosticsService, DiagnosticsService>()
			.AddSingleton<IBrowser>(s => new DispatcherBrowserDecorator(new BrowserImplementation(), App.Instance.Shell.DispatcherQueue))
			.AddSingleton<IExtendedSplashscreenController, ExtendedSplashscreenController>()
			.AddMessageDialog();
	}

	private static IServiceCollection AddMessageDialog(this IServiceCollection services)
	{
		//-:cnd:noEmit
#if WINDOWS

		Window currentWindow = new Window();
		IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(currentWindow);
		DispatcherQueue dispatcher = DispatcherQueue.GetForCurrentThread();


#endif
		return services.AddSingleton<IMessageDialogService>(s =>
			//-:cnd:noEmit
#if WINDOWS || __IOS__ || __ANDROID__
			//+:cnd:noEmit
			new MessageDialogService.MessageDialogService(
#if WINDOWS
				dispatcher,
#else
				() => s.GetRequiredService<CoreDispatcher>(),
#endif
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key],
					windowHandle
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
