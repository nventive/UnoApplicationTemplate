using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationTemplate.Presentation;
using Chinook.BackButtonManager;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.UI.Core;

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
		Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif
//+:cnd:noEmit
	}

	protected override void InitializeViewServices(IHostBuilder hostBuilder)
	{
		// TODO: Configure your platform-specific service implementations from here.

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
		// Configures a default refresh command for all DataLoaderView controls.
		DataLoaderView.DefaultRefreshCommandProvider = GetDataLoaderViewRefreshCommand;

		HandleUnhandledExceptions(services);

		ICommand GetDataLoaderViewRefreshCommand(DataLoaderView dataLoaderView)
		{
			return services
				.GetRequiredService<IDynamicCommandBuilderFactory>()
				.CreateFromTask(
					name: "DataLoaderViewRefreshCommand",
					execute: async (ct) =>
					{
						var context = new DataLoaderContext();

						context["IsForceRefreshing"] = true;

						await dataLoaderView.DataLoader.Load(ct, context);
					},
					viewModel: (IViewModel)App.Instance.Shell.DataContext)
				.Build();
		}
	}

	protected override async Task StartViewServices(IServiceProvider services, bool isFirstStart)
	{
		if (isFirstStart)
		{
			// TODO: Start your platform-specific services from here.

			await SetShellViewModel();

			await AddSystemBackButtonSource(services);

			HandleSystemBackVisibility(services);

			// Set StatusBar color depending on current ViewModel.
			SetStatusBarColor(services);
		}
	}

	protected override ILogger GetOrCreateLogger(IServiceProvider serviceProvider)
	{
		return serviceProvider.GetRequiredService<ILogger<Startup>>();
	}

	private static void HandleUnhandledExceptions(IServiceProvider services)
	{
		void OnError(Exception e, bool isTerminating = false) => ErrorConfiguration.OnUnhandledException(e, isTerminating, services);

//-:cnd:noEmit
#if __WINDOWS__ || __ANDROID__ || __IOS__

		Application.Current.UnhandledException += (s, e) =>
		{
			OnError(e.Exception);
			e.Handled = true;
		};
#endif
//+:cnd:noEmit
//-:cnd:noEmit
#if __ANDROID__
		Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
		{
			OnError(e.Exception);
			e.Handled = true;
		};
#endif
//+:cnd:noEmit
	}

	private static async Task SetShellViewModel()
	{
		await App.Instance.Shell.DispatcherQueue.EnqueueAsync(SetDataContextUI);

		static void SetDataContextUI() // Runs on UI thread.
		{
			var shellViewModel = new ShellViewModel();

			shellViewModel.AttachToView(App.Instance.Shell);

			App.Instance.Shell.DataContext = shellViewModel;
		}
	}

	/// <summary>
	/// Sets the visibility of the system UI's back button based on the navigation controller.
	/// </summary>
	private void HandleSystemBackVisibility(IServiceProvider services)
	{
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
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
			var dispatcherQueue = services.GetRequiredService<DispatcherQueue>();
			_ = dispatcherQueue.EnqueueAsync(UpdateBackButtonUI);

			void UpdateBackButtonUI() // Runs on UI thread.
			{
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = canNavigateBackOrCloseModal
					? AppViewBackButtonVisibility.Visible
					: AppViewBackButtonVisibility.Collapsed;
			}
		}
#endif
//+:cnd:noEmit
	}

	/// <summary>
	/// Adds SystemNavigation's back requests to the IBackButtonManager. Those requests are usually sent via the top bars back buttons.
	/// </summary>
	private async Task AddSystemBackButtonSource(IServiceProvider services)
	{
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
		var dispatcherQueue = services.GetRequiredService<DispatcherQueue>();
		var backButtonManager = services.GetRequiredService<IBackButtonManager>();
		await dispatcherQueue.EnqueueAsync(AddSystemBackButtonSourceInner, DispatcherQueuePriority.High);

		// Runs on main thread.
		void AddSystemBackButtonSourceInner()
		{
			var source = new SystemNavigationBackButtonSource();
			backButtonManager.AddSource(source);
		}
#endif
//+:cnd:noEmit
	}

	private void SetStatusBarColor(IServiceProvider services)
	{
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
		var dispatcher = services.GetRequiredService<IDispatcherScheduler>();

		// These are pages with a different background color, needing a different status bar color.
		var vmsAlternateColor = new Type[]
		{
			typeof(OnboardingPageViewModel),
			typeof(LoginPageViewModel),
			typeof(ForgotPasswordFormViewModel),
			typeof(ResetPasswordPageViewModel),
			typeof(SentEmailConfirmationPageViewModel),
		};

		services
			.GetRequiredService<ISectionsNavigator>()
			.ObserveProcessedState()
			.ObserveOn(dispatcher)
			.Subscribe(onNext: state =>
			{
				var currentVmType = state.CurrentState.GetCurrentOrNextViewModelType();

				// We set the default status bar color to white.
				var statusBarColor = Microsoft.UI.Colors.White;

				if (App.Instance.CurrentWindow.Content is FrameworkElement root && root.ActualTheme == ElementTheme.Dark)
				{
					// For dark theme, the status bar is black except for the pages in vmsAlternateColor.
					if (!vmsAlternateColor.Contains(currentVmType))
					{
						statusBarColor = Microsoft.UI.Colors.Black;
					}
				}
				else
				{
					// For light theme, the status bar is white except for the pages in vmsAlternateColor.
					if (vmsAlternateColor.Contains(currentVmType))
					{
						statusBarColor = Microsoft.UI.Colors.Black;
					}
				}

				Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = statusBarColor;
			},
			onError: e => Logger.LogError(e, "Failed to set status bar color."));
#endif
//+:cnd:noEmit
	}
}
