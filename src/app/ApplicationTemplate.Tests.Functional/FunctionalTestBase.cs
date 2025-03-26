using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using ApplicationTemplate.DataAccess.PlatformServices;
using Chinook.BackButtonManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;
using Xunit.Abstractions;
using static Microsoft.Extensions.Configuration.ApplicationTemplateConfigurationExtensions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace ApplicationTemplate.Tests;

/// <summary>
/// Gives access to the services and their configuration.
/// </summary>
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "IAsyncLifetime provides a DisposeAsync method that is automatically called.")]
public abstract class FunctionalTestBase : IAsyncLifetime
{
	private readonly ITestOutputHelper _output;
	private readonly CoreStartup _coreStartup;
	private readonly FunctionalTestBackButtonSource _backButtonSource = new();
	private readonly Lazy<ShellViewModel> _shellViewModel = new(() => new ShellViewModel());

	/// <summary>
	/// Initializes a new instance of the <see cref="FunctionalTestBase"/> class.
	/// </summary>
	/// <remarks>
	/// This is called before every test.
	/// </remarks>
	/// <param name="output">Optional output parameter. Provide it when you want to consult the logs of this test.</param>
	protected FunctionalTestBase(ITestOutputHelper output = null)
	{
		_output = output;
		_coreStartup = new CoreStartup();
		_coreStartup.PreInitialize();
		_coreStartup.Initialize(contentRootPath: string.Empty, settingsFolderPath: string.Empty, environmentManager: new TestEnvironmentManager(), ConfigureLogging, Configure);

		_coreStartup.ServiceProvider.GetRequiredService<IBackButtonManager>().AddSource(_backButtonSource);

		void Configure(IHostBuilder hostBuilder)
		{
			// Add the mock options overrides.
			hostBuilder.ConfigureAppConfiguration(configurationBuilder =>
				configurationBuilder.AddInMemoryCollection(GetTestConfigurationValues())
			);

			// Override the application settings if the derived class overrides the ApplicationSettings property.
			OverrideApplicationSettings(hostBuilder);

			ConfigureTestServices(hostBuilder);

			// Add the extra configuration from the specific test. (ConfigureHost is a virtual method.)
			ConfigureHost(hostBuilder);

			IEnumerable<KeyValuePair<string, string>> GetTestConfigurationValues()
			{
				// Override the IsMockEnabled value based on the USE_REAL_APIS environment variable.
				_ = bool.TryParse(Environment.GetEnvironmentVariable("USE_REAL_APIS"), out var useReadApis);
				var isMockEnabledKey = $"{DefaultOptionsName<MockOptions>()}:{nameof(MockOptions.IsMockEnabled)}";
				yield return new KeyValuePair<string, string>(isMockEnabledKey, (!useReadApis).ToString());

				// Override the IsDelayForSimulatedApiCallsEnabled value to false to avoid unnecessary delays in automated tests.
				var isDelayForSimulatedApiCallsEnabledKey = $"{DefaultOptionsName<MockOptions>()}:{nameof(MockOptions.IsDelayForSimulatedApiCallsEnabled)}";
				yield return new KeyValuePair<string, string>(isDelayForSimulatedApiCallsEnabledKey, "false");
			}

			void OverrideApplicationSettings(IHostBuilder hostBuilder)
			{
				if (ApplicationSettings != ApplicationSettings.Default)
				{
					hostBuilder.ConfigureServices(services =>
					{
						services.AddSingleton(s => PersistenceConfiguration.CreateObservableDataPersister(s, ApplicationSettings));
					});
				}
			}
		}
	}

	/// <summary>
	/// Gets the <see cref="ShellViewModel"/> associated with this test. It represents the ViewModel of the root of the app.
	/// </summary>
	public ShellViewModel Shell => _shellViewModel.Value;

	/// <summary>
	/// Gets the <see cref="MenuViewModel"/> associated with this test.
	/// This is useful to change the section of the app.
	/// </summary>
	public MenuViewModel Menu
	{
		get
		{
			var menu = Shell.Menu;

			return menu.MenuState is "Closed"
				? throw new InvalidOperationException("The menu is currently closed and therefore cannot be accessed. You must be on a page that displays the menu in order to access it. This is to replicate the behavior of the app.")
				: menu;
		}
	}

	/// <summary>
	/// Gets the active ViewModel from the <see cref="ISectionsNavigator"/>.
	/// </summary>
	/// <remarks>
	/// Use <see cref="GetAndAssertActiveViewModel{TViewModel}"/> to get the active ViewModel as a specific type in order to interact with its members.
	/// </remarks>
	public INavigableViewModel ActiveViewModel => GetService<ISectionsNavigator>().GetActiveViewModel();

	/// <summary>
	/// Asserts that the active ViewModel is of the expected type and returns it.
	/// </summary>
	/// <typeparam name="TViewModel">The expected ViewModel type.</typeparam>
	/// <returns>The active ViewModel .</returns>
	public TViewModel GetAndAssertActiveViewModel<TViewModel>()
		where TViewModel : INavigableViewModel
	{
		return Assert.IsType<TViewModel>(ActiveViewModel);
	}

	/// <summary>
	/// Simulates the back button press.
	/// </summary>
	public void NavigateBackUsingHardwareButton()
	{
		_backButtonSource.RaiseBackRequested();
	}

	/// <summary>
	/// Waits for navigation out of or to a specific ViewModel type to be completed.
	/// This is for when you cannot let the test continue until the navigation is completed.
	/// It should be used when you know that the navigation is going to happen.
	/// </summary>
	/// <param name="viewModelType">The type of viewmodel we are navigating to or from.</param>
	/// <param name="isDestination">Whether the viewmodel is the destination or the origin.</param>
	protected async Task WaitForNavigation(Type viewModelType, bool isDestination)
	{
		var sectionNavigator = GetService<ISectionsNavigator>();

		var timeoutTask = Task.Delay(1000);

		// Waits for the navigation to be completed, we need the StartWith in case the navigation already finished before we started observing.
		var navTask = sectionNavigator.ObserveCurrentState()
			.StartWith(sectionNavigator.State)
			.Where(x => x.LastRequestState != NavigatorRequestState.Processing
				&& x.GetCurrentOrNextViewModelType() != null
				&& x.GetCurrentOrNextViewModelType() == viewModelType == isDestination)
			// The CancellationToken is so that this returns a task.
			.FirstAsync(CancellationToken.None);

		await Task.WhenAny(navTask, timeoutTask);
	}

	/// <summary>
	/// Configures the services required for functional testing.
	/// </summary>
	/// <param name="host">The host builder.</param>
	private static void ConfigureTestServices(IHostBuilder host)
	{
		host.ConfigureServices(services =>
		{
			services
				.AddSingleton<IScheduler>(s => TaskPoolScheduler.Default)
				.AddSingleton<IDispatcherScheduler>(s => new DispatcherSchedulerAdapter(
					s.GetRequiredService<IScheduler>()
				))
				.AddSingleton(s => Substitute.For<ILauncherService>());
		});
	}

	private void ConfigureLogging(HostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder, bool isAppLogging)
	{
		if (_output is null)
		{
			return;
		}

		var serilogConfiguration = new LoggerConfiguration()
			.ReadFrom.Configuration(hostBuilderContext.Configuration)
			.Enrich.With(new ThreadIdEnricher())
			.WriteTo.TestOutput(_output, outputTemplate: "{Timestamp:HH:mm:ss.fff} Thread:{ThreadId} {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}", formatProvider: CultureInfo.InvariantCulture);

		var logger = serilogConfiguration.CreateLogger();
		loggingBuilder.AddSerilog(logger);
	}

	/// <summary>
	/// Override this method in your test to add extra configuration such as services overrides.
	/// </summary>
	/// <param name="hostBuilder">The host builder.</param>
	protected virtual void ConfigureHost(IHostBuilder hostBuilder)
	{
	}

	/// <summary>
	/// Replaces the registration for type <typeparamref name="TService"/> with a mocked implementation.
	/// </summary>
	/// <typeparam name="TService">The type of service.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <param name="mockedService">The mocked Service.</param>
	/// <returns>the registered services.</returns>
	protected virtual IServiceCollection ReplaceWithMock<TService>(IServiceCollection services, out TService mockedService)
		 where TService : class
	{
		mockedService = Substitute.For<TService>();

		return services.Replace(ServiceDescriptor.Singleton<TService>(mockedService));
	}

	/// <summary>
	/// Gets the application settings to use for this test.
	/// </summary>
	public virtual ApplicationSettings ApplicationSettings { get; } = ApplicationSettings.Default;

	/// <summary>
	/// Returns the requested service.
	/// </summary>
	/// <typeparam name="TService">Type of service.</typeparam>
	/// <returns>The requested service.</returns>
	protected virtual TService GetService<TService>()
	{
		return _coreStartup.ServiceProvider.GetRequiredService<TService>();
	}

	async Task IAsyncLifetime.InitializeAsync()
	{
		await _coreStartup.Start();

		// This sometimes fails when parallelization is enabled. (That's one of the reasons why parallelization is disabled.)
		ViewModelBase.DefaultServiceProvider.Should().BeSameAs(_coreStartup.ServiceProvider, because: "We want the ViewModels of this test to use the services that we just initialized.");
	}

	async Task IAsyncLifetime.DisposeAsync()
	{
		// To prevent scheduled tasks in view models from running after the test is completed.
		// Once the test is finished, the view models should no longer be active.
		// Therefore, we clear the stack navigator, which disposes of all view models.
		await GetService<ISectionsNavigator>().CloseModals(CancellationToken.None);
		await GetService<ISectionsNavigator>().ClearSections(CancellationToken.None);

		_coreStartup.Dispose();
		_backButtonSource.Dispose();
	}

	private sealed class FunctionalTestBackButtonSource : IBackButtonSource
	{
		public string Name => "FunctionalTestBackButtonSource";

		public event BackRequestedEventHandler BackRequested;

		public void Dispose()
		{
			BackRequested = null;
		}

		public void RaiseBackRequested()
		{
			BackRequested?.Invoke(this, new BackRequestedEventArgs());
		}
	}
}
