using System.Reactive.Concurrency;
using ApplicationTemplate.DataAccess.PlatformServices;
using Chinook.DynamicMvvm;
using CPS.DataAccess.PlatformServices;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Dispatching;
using ReviewService;

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
			.AddSingleton(s => App.Instance.NavigationMultiFrame.DispatcherQueue)
			.AddSingleton(s => Shell.Instance.ExtendedSplashScreen)
			.AddSingleton<IDispatcherScheduler>(s => new MainDispatcherScheduler(
				s.GetRequiredService<DispatcherQueue>(),
				DispatcherQueuePriority.Normal
			))
			.AddSingleton<IDispatcherFactory, DispatcherFactory>()
			.AddSingleton<IDiagnosticsService, DiagnosticsService>()
			.AddSingleton<ILauncherService>(s => new LauncherService(s.GetRequiredService<DispatcherQueue>()))
			.AddSingleton<IVersionProvider, VersionProvider>()
			.AddSingleton<IAppStoreUriProvider, AppStoreUriProvider>()
			.AddSingleton<IDeviceInformationProvider, DeviceInformationProvider>()
			.AddSingleton<IExtendedSplashscreenController, ExtendedSplashscreenController>(s => new ExtendedSplashscreenController(Shell.Instance.DispatcherQueue))
			.AddSingleton<IConnectivityProvider, ConnectivityProvider>()
			.AddSingleton<IEmailService, EmailService>()
			.AddSingleton<IMemoryProvider, MemoryProvider>()
			.AddSingleton<IReviewPrompter, ReviewPrompter>()
			.AddMessageDialog()
			.AddToastService();
	}

	private static IServiceCollection AddMessageDialog(this IServiceCollection services)
	{
		return services.AddSingleton<IMessageDialogService>(s =>
//-:cnd:noEmit
#if __WINDOWS__ || __IOS__ || __ANDROID__
			new MessageDialogService.MessageDialogService(
				s.GetRequiredService<DispatcherQueue>(),
//-:cnd:noEmit
#if __WINDOWS__
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key],
					WinRT.Interop.WindowNative.GetWindowHandle(App.Instance.CurrentWindow)
				)
#else
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key]
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

	private static IServiceCollection AddToastService(this IServiceCollection services)
	{
		return services.AddSingleton<IToastService>(s =>
//-:cnd:noEmit
#if __ANDROID__ || __WINDOWS__
			new ToastService(s.GetRequiredService<DispatcherQueue>())
#else
			new ToastService(
				s.GetRequiredService<IDispatcherScheduler>(),
				Shell.Instance.Panel,
				new Microsoft.UI.Xaml.DataTemplate(() =>
				{
					var border = new Microsoft.UI.Xaml.Controls.Border
					{
						IsHitTestVisible = false,
						CornerRadius = 10,
						Margin = new Microsoft.UI.Xaml.Thickness(20),
						Padding = new Microsoft.UI.Xaml.Thickness(20, 10),
						VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Bottom,
						HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
						Background = CommunityToolkit.WinUI.Helpers.ColorHelper.ToColor("#CC000000"),
					};

					var textBlock = new Microsoft.UI.Xaml.Controls.TextBlock
					{
						Foreground = Microsoft.UI.Colors.White,
						FontSize = 17,
						TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
						TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
						HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
						VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
					};

					textBlock.SetBinding("Text", "Message");
					border.Child = textBlock;

					return border;
				})
			)
#endif
//+:cnd:noEmit
		);
	}
}
