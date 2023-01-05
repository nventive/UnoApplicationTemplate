using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Chinook.BackButtonManager;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace ApplicationTemplate.Views;

public sealed class Startup : StartupBase
{
	public Startup()
		: base(new CoreStartup())
	{
	}

	protected override void PreInitializeServices()
	{
		LocalizationConfiguration.PreInitialize();

//-:cnd:noEmit
#if __ANDROID__ || __IOS__
//+:cnd:noEmit
		Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}

	protected override void InitializeViewServices(IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureServices(s => s
			.AddSingleton<StartupBase>(this)
			.AddLocalization()
			.AddNavigation()
			.AddViewServices()
			.AddApi()
			.AddPersistence()
		);
	}

	protected override void OnInitialized(IServiceProvider services)
	{
#if false
		AsyncWebView.AsyncWebView.Logger = services.GetRequiredService<ILogger<AsyncWebView.AsyncWebView>>();

		HandleUnhandledExceptions(services);
#endif
	}

#if false
	private static void HandleUnhandledExceptions(IServiceProvider services)
	{
		void OnError(Exception e, bool isTerminating = false) => ErrorConfiguration.OnUnhandledException(e, isTerminating, services);

//-:cnd:noEmit
#if WINDOWS || __ANDROID__ || __IOS__
//+:cnd:noEmit
		Windows.UI.Xaml.Application.Current.UnhandledException += (s, e) =>
		{
			OnError(e.Exception);
			e.Handled = true;
		};
//-:cnd:noEmit
#endif
//+:cnd:noEmit
//-:cnd:noEmit
#if __ANDROID__
//+:cnd:noEmit
		Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
		{
			OnError(e.Exception);
			e.Handled = true;
		};
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}
#endif

	protected override async Task StartViewServices(IServiceProvider services, bool isFirstStart)
	{
		if (isFirstStart)
		{
			// Start your view services here.
			await SetShellViewModel();
#if false
			await AddSystemBackButtonSource(services);

			HandleSystemBackVisibility(services);

			// Set StatusBar color depending on current ViewModel
			SetStatusBarColor(services);
#endif
		}
	}

	private static async Task SetShellViewModel()
	{
		await App.Instance.Shell.DispatcherQueue.RunAsync(DispatcherQueuePriority.Normal, SetDataContextUI);

		void SetDataContextUI() // Runs on UI thread.
		{
			var shellViewModel = new ShellViewModel();

			shellViewModel.AttachToView(App.Instance.Shell);

			App.Instance.Shell.DataContext = shellViewModel;
		}
	}

#if false
	/// <summary>
	/// Sets the visibility of the system UI's back button based on the navigation controller.
	/// </summary>
	private void HandleSystemBackVisibility(IServiceProvider services)
	{
		var multiNavigationController = services.GetRequiredService<ISectionsNavigator>();

		Observable
			.FromEventPattern<SectionsNavigatorStateChangedEventHandler, SectionsNavigatorEventArgs>(
				h => multiNavigationController.StateChanged += h,
				h => multiNavigationController.StateChanged -= h
			)
			.Select(_ => multiNavigationController.CanNavigateBackOrCloseModal())
			.StartWith(multiNavigationController.CanNavigateBackOrCloseModal())
			.DistinctUntilChanged()
			.Subscribe(OnStateChanged);

		void OnStateChanged(bool canNavigateBackOrCloseModal)
		{
			var dispatcher = services.GetRequiredService<CoreDispatcher>();
			_ = dispatcher.RunAsync((CoreDispatcherPriority)CoreDispatcherPriority.Normal, UpdateBackButtonUI);

			void UpdateBackButtonUI() // Runs on UI thread.
			{
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = canNavigateBackOrCloseModal
					? AppViewBackButtonVisibility.Visible
					: AppViewBackButtonVisibility.Collapsed;
			}
		}
	}

	/// <summary>
	/// Adds SystemNavigation's back requests to the IBackButtonManager. Those requests are usually sent via the top bars back buttons.
	/// </summary>
	private async Task AddSystemBackButtonSource(IServiceProvider services)
	{
		var dispatcher = services.GetRequiredService<CoreDispatcher>();
		var backButtonManager = services.GetRequiredService<IBackButtonManager>();
		await dispatcher.RunAsync((CoreDispatcherPriority)CoreDispatcherPriority.High, AddSystemBackButtonSourceInner);

		// Runs on main thread.
		void AddSystemBackButtonSourceInner()
		{
			var source = new SystemNavigationBackButtonSource();
			backButtonManager.AddSource(source);
		}
	}
#endif

	protected override ILogger GetOrCreateLogger(IServiceProvider serviceProvider)
	{
		return serviceProvider.GetRequiredService<ILogger<Startup>>();
	}

#if false
	private void SetStatusBarColor(IServiceProvider services)
	{
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
//+:cnd:noEmit
		var dispatcher = services.GetRequiredService<IDispatcherScheduler>();

		// These are pages with a different background color, needing a different status bar color
		Type[] vmsAlternateColor =
		{
			typeof(OnboardingPageViewModel),
			typeof(LoginPageViewModel),
			typeof(ForgotPasswordFormViewModel),
			typeof(ResetPasswordPageViewModel),
			typeof(SentEmailConfirmationPageViewModel)
		};

		services
			.GetRequiredService<ISectionsNavigator>()
			.ObserveProcessedState()
			.ObserveOn(dispatcher)
			.Subscribe(onNext: state =>
			{
				var currentVmType = state.CurrentState.GetViewModelType();

				// We set the default status bar color to white
				var statusBarColor = Windows.UI.Colors.White;

				if (Window.Current.Content is FrameworkElement root && root.ActualTheme == ElementTheme.Dark)
				{
					// For dark theme, the status bar is black except for the pages in vmsAlternateColor
					if (!vmsAlternateColor.Contains(currentVmType))
					{
						statusBarColor = Windows.UI.Colors.Black;
					}
				}
				else
				{
					// For light theme, the status bar is white except for the pages in vmsAlternateColor
					if (vmsAlternateColor.Contains(currentVmType))
					{
						statusBarColor = Windows.UI.Colors.Black;
					}
				}

				Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = statusBarColor;
			}, e => Logger.LogError(e, "Failed to set status bar color."));
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}
#endif
}
