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
				App.Window.DispatcherQueue,
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
		return services.AddSingleton<IMessageDialogService>(s =>
			new MessageDialogService.MessageDialogService(
				App.Window.DispatcherQueue,
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key],
					WinRT.Interop.WindowNative.GetWindowHandle(App.Window)
				)
			)
		);
	}
}
