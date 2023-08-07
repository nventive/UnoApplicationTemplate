using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using ApplicationTemplate.Presentation;
using Chinook.BackButtonManager;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.DynamicMvvm.Deactivation;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using FluentValidation;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReviewService;
using Uno.Disposables;
using Uno.Extensions;

namespace ApplicationTemplate;

public sealed class CoreStartup : CoreStartupBase
{
	protected override void PreInitializeServices()
	{
	}

	protected override IHostBuilder InitializeServices(IHostBuilder hostBuilder, string settingsFolderPath, IEnvironmentManager environmentManager)
	{
		// TODO: Configure your core services from here.
		// Core service implementations can be used on any platform.
		// Platform specific implementations override the core implementations and are configured from the Startup class.

		return hostBuilder
			.AddConfiguration(settingsFolderPath, environmentManager)
			.ConfigureServices((context, s) => s
				.AddDiagnostics(context.Configuration)
				.AddMock(context.Configuration)
				.AddApi(context.Configuration)
				.AddMvvm()
				.AddPersistence()
				.AddNavigationCore()
				.AddErrorHandling()
				.AddSerialization()
				.AddLocalization()
				.AddReviewServices()
				.AddAppServices()
			);
	}

	protected override void OnInitialized(IServiceProvider services)
	{
		// At this point all services are registered and can be used.

		ViewModelBase.DefaultServiceProvider = services;

		InitializeLoggerFactories(services);

		HandleUnhandledExceptions(services);

		ValidatorOptions.Global.LanguageManager = new FluentValidationLanguageManager();
	}

	protected override async Task StartServices(IServiceProvider services, bool isFirstStart)
	{
		if (isFirstStart)
		{
			// TODO: Start your core services and customize the initial navigation logic here.

			await services.GetRequiredService<IReviewService>().TrackApplicationLaunched(CancellationToken.None);

			NotifyUserOnSessionExpired(services);

			services.GetRequiredService<DiagnosticsCountersService>().Start();

			await ExecuteInitialNavigation(CancellationToken.None, services);
		}
	}

	/// <summary>
	/// Executes the initial navigation.
	/// </summary>
	/// <remarks>
	/// This can be invoked from diagnostics to reset the app navigation.
	/// </remarks>
	/// <param name="ct">The cancellation token.</param>
	/// <param name="services">The service provider.</param>
	public static async Task ExecuteInitialNavigation(CancellationToken ct, IServiceProvider services)
	{
		var applicationSettingsService = services.GetRequiredService<IApplicationSettingsService>();
		var sectionsNavigator = services.GetRequiredService<ISectionsNavigator>();
		var authenticationService = services.GetRequiredService<IAuthenticationService>();

		await sectionsNavigator.SetActiveSection(ct, "Login");

		var currentSettings = await applicationSettingsService.GetAndObserveCurrent().FirstAsync(ct);
		if (currentSettings.IsOnboardingCompleted)
		{
			var isAuthenticated = await authenticationService.GetAndObserveIsAuthenticated().FirstAsync(ct);

			if (isAuthenticated)
			{
				await sectionsNavigator.SetActiveSection(ct, "Home", () => new DadJokesPageViewModel());
			}
			else
			{
				await sectionsNavigator.Navigate(ct, () => new LoginPageViewModel(isFirstLogin: false));
			}
		}
		else
		{
			await sectionsNavigator.Navigate(ct, () => new OnboardingPageViewModel());
		}

		services.GetRequiredService<IExtendedSplashscreenController>().Dismiss();
	}

	private void NotifyUserOnSessionExpired(IServiceProvider services)
	{
		var authenticationService = services.GetRequiredService<IAuthenticationService>();
		var messageDialogService = services.GetRequiredService<IMessageDialogService>();

		authenticationService
			.ObserveSessionExpired()
			.SkipWhileSelectMany(async (ct, s) =>
			{
				await messageDialogService.ShowMessage(ct, mb => mb
					.TitleResource("SessionExpired_DialogTitle")
					.ContentResource("SessionExpired_DialogBody")
					.OkCommand()
				);

				var navigationController = services.GetRequiredService<ISectionsNavigator>();

				foreach (var modal in navigationController.State.Modals)
				{
					await navigationController.CloseModal(CancellationToken.None);
				}

				foreach (var stack in navigationController.State.Sections)
				{
					await ClearNavigationStack(CancellationToken.None, stack.Value);
				}

				await services.GetRequiredService<ISectionsNavigator>().SetActiveSection(ct, "Login", () => new LoginPageViewModel(isFirstLogin: false), returnToRoot: true);
			})
			.Subscribe(_ => { }, e => Logger.LogError(e, "Failed to notify user of session expiration."))
			.DisposeWith(Disposables);
	}

	private static async Task ClearNavigationStack(CancellationToken ct, ISectionStackNavigator stack)
	{
		await stack.Clear(ct);
	}

	private static void InitializeLoggerFactories(IServiceProvider services)
	{
		var loggerFactory = services.GetRequiredService<ILoggerFactory>();

		StackNavigationConfiguration.LoggerFactory = loggerFactory;
		SectionsNavigationConfiguration.LoggerFactory = loggerFactory;
		BackButtonManagerConfiguration.LoggerFactory = loggerFactory;
		DynamicMvvmConfiguration.LoggerFactory = loggerFactory;
		DataLoaderConfiguration.LoggerFactory = loggerFactory;
	}

	private static void HandleUnhandledExceptions(IServiceProvider services)
	{
		void OnError(Exception e, bool isTerminating = false) => ErrorConfiguration.OnUnhandledException(e, isTerminating, services);

		TaskScheduler.UnobservedTaskException += (s, e) =>
		{
			OnError(e.Exception);
			e.SetObserved();
		};

		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			var exception = e.ExceptionObject as Exception;

			if (exception == null)
			{
				return;
			}

			OnError(exception, e.IsTerminating);
		};
	}

	/// <summary>
	/// Starts deactivating and reactivating ViewModels based on navigation.
	/// </summary>
	/// <remarks>
	/// To benefit from the deactivation optimizations, you need to do the following:<br/>
	/// 1. Call this method from <see cref="StartServices(IServiceProvider, bool)"/>.<br/>
	/// 2. Change <see cref="ViewModel"/> so that it inherits from <see cref="DeactivatableViewModelBase"/> rather than just <see cref="ViewModelBase"/>.<br/>
	/// 3. Consider using extensions such as GetFromDeactivatableObservable instead of the classic GetFromObservable for your dynamic properties from observables.<br/>
	/// ViewModel deactivation shows benefits when your pages have a lot of live updates.<br/>
	/// Deactivation means that a page ViewModel unsubscribes from its observable or event dependencies while it's not active (i.e. "visible" to the user).<br/>
	/// Reactivation occurs when a page ViewModel becomes active again and causes re-subscriptions.
	/// </remarks>
	/// <param name="services">The <see cref="IServiceProvider"/>.</param>
	private void StartViewModelDeactivation(IServiceProvider services)
	{
		var sectionsNavigator = services.GetRequiredService<ISectionsNavigator>();

		IDeactivatable previousVM = null;
		sectionsNavigator.StateChanged += OnSectionsNavigatorStateChanged;

		void OnSectionsNavigatorStateChanged(object sender, SectionsNavigatorEventArgs args)
		{
			var currentVM = sectionsNavigator.GetActiveStackNavigator()?.GetActiveViewModel() as IDeactivatable;

			if (previousVM != currentVM)
			{
				previousVM?.Deactivate();
				currentVM?.Reactivate();
			}

			previousVM = currentVM;
		}
	}

	protected override ILogger GetOrCreateLogger(IServiceProvider serviceProvider)
	{
		return serviceProvider.GetRequiredService<ILogger<CoreStartup>>();
	}
}
