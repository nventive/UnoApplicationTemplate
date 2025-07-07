// src/app/ApplicationTemplate.Shared.Views/Configuration/ViewServicesConfiguration.cs
using System.Reactive.Concurrency;
using ApplicationTemplate.DataAccess.PlatformServices;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Dispatching;
using ReviewService;

namespace ApplicationTemplate.Views;

/// 
/// This class is used for view services.
/// - Configures view services.
/// 
public static class ViewServicesConfiguration
{
	public static IServiceCollection AddViewServices(this IServiceCollection services)
	{
		return services
			.AddSingleton(s => App.Instance.NavigationMultiFrame.DispatcherQueue)
			.AddSingleton(s => Shell.Instance.ExtendedSplashScreen)
			.AddSingleton(s => new MainDispatcherScheduler(
				s.GetRequiredService(),
				DispatcherQueuePriority.Normal
			))
			.AddSingleton()
			.AddSingleton()
			.AddSingleton(s => new LauncherService(s.GetRequiredService()))
			.AddSingleton()
			.AddSingleton()
			.AddSingleton()
			.AddSingleton(s => new ExtendedSplashscreenController(Shell.Instance.DispatcherQueue))
			.AddSingleton()
			.AddSingleton()
			.AddSingleton()
			.AddSingleton()
			.AddSingleton()
			.AddMessageDialog();
	}

	private static IServiceCollection AddMessageDialog(this IServiceCollection services)
	{
		return services.AddSingleton(s =>
			//-:cnd:noEmit
#if __WINDOWS__ || __IOS__ || __ANDROID__
			new MessageDialogService.MessageDialogService(
				s.GetRequiredService(),
				//-:cnd:noEmit
#if __WINDOWS__
                new MessageDialogBuilderDelegate(
                    key => s.GetRequiredService()[key],
                    WinRT.Interop.WindowNative.GetWindowHandle(App.Instance.CurrentWindow)
                )
#else
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService()[key]
				)
#endif
			//+:cnd:noEmit
			)
#else
            new AcceptOrDefaultMessageDialogService()
#endif
		//+:cnd:noEmit
		);
	}
}
