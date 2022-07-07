﻿using System;
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
using Uno.UI;
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
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
//+:cnd:noEmit

		FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}

	protected override void InitializeViewServices(IHostBuilder hostBuilder)
	{
		hostBuilder.AddViewServices();
	}

	protected override void OnInitialized(IServiceProvider services)
	{
		AsyncWebView.AsyncWebView.Logger = services.GetRequiredService<ILogger<AsyncWebView.AsyncWebView>>();
	}

	protected override async Task StartViewServices(IServiceProvider services, bool isFirstStart)
	{
		if (isFirstStart)
		{
			// Start your view services here.
			await SetShellViewModel();
			await AddSystemBackButtonSource(services);

			HandleSystemBackVisibility(services);

			// Set StatusBar color depending on current ViewModel
			SetStatusBarColor(services);
		}
	}

	private static async Task SetShellViewModel()
	{
		await App.Instance.Shell.Dispatcher.RunAsync((CoreDispatcherPriority)CoreDispatcherPriority.Normal, SetDataContextUI);

		void SetDataContextUI() // Runs on UI thread
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

			void UpdateBackButtonUI() // Runs on UI thread
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

	protected override ILogger GetOrCreateLogger(IServiceProvider serviceProvider)
	{
		return serviceProvider.GetRequiredService<ILogger<Startup>>();
	}

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
}
