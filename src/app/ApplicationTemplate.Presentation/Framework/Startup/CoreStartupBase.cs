using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Uno.Disposables;

namespace ApplicationTemplate;

/// <summary>
/// This class abstracts the core startup of the app.
/// This abstract class is responsible for building the host of the application as well as startup diagnostics.
/// The implementator class is responsible for the application-specific code that initializes the application's services.
/// </summary>
public abstract class CoreStartupBase : IDisposable
{
	private IHost _host;

	protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

	public StartupState State { get; } = new StartupState();

	public IServiceProvider ServiceProvider { get; private set; }

	public Activity BuildCoreHostActivity { get; } = new Activity("BuildCoreHost");

	public Activity BuildHostActivity { get; } = new Activity("BuildHost");

	protected ILogger Logger { get; private set; }

	/// <summary>
	/// Pre-initializes the application.
	/// This must be called as early as possible.
	/// </summary>
	public void PreInitialize()
	{
		if (State.IsPreInitialized)
		{
			throw new InvalidOperationException($"You shouldn't call {nameof(PreInitialize)} more than once.");
		}

		PreInitializeServices();

		State.IsPreInitialized = true;
	}

	protected abstract void PreInitializeServices();

	/// <summary>
	/// Initializes the application.
	/// </summary>
	/// <param name="contentRootPath">Specifies the content root directory to be used by the host.</param>
	/// <param name="settingsFolderPath">The folder path indicating where the override files are.</param>
	/// <param name="environmentManager">The environment manager.</param>
	/// <param name="loggingConfiguration">The delegate to call to configure logging.</param>
	/// <param name="extraHostConfiguration">The extra host configuration.</param>
	public void Initialize(
		string contentRootPath,
		string settingsFolderPath,
		IEnvironmentManager environmentManager,
		LoggingConfigurator loggingConfiguration = null,
		Action<IHostBuilder> extraHostConfiguration = null)
	{
		if (State.IsInitialized)
		{
			throw new InvalidOperationException($"You shouldn't call {nameof(Initialize)} more than once.");
		}

		if (!State.IsPreInitialized)
		{
			throw new InvalidOperationException($"You must call {nameof(PreInitialize)} before calling '{nameof(Initialize)}'.");
		}

		BuildCoreHostActivity.Start();

		// We use 2 hosts. The first one is used during the setup of the second.
		// The first one is mainly used to setup loggers to debug the initialization code.

		var hostServices = CreateHostServices(contentRootPath, settingsFolderPath, loggingConfiguration, environmentManager);
		Logger = GetOrCreateLogger(hostServices);

		BuildCoreHostActivity.Stop();

		BuildHostActivity.Start();

		var hostBuilder = InitializeServices(new HostBuilder()
			.UseContentRoot(contentRootPath)
			.ConfigureLogging((hostBuilderContext, loggingBuilder) =>
			{
				loggingConfiguration(hostBuilderContext, loggingBuilder, isAppLogging: true);
				loggingBuilder.Services.BindOptionsToConfiguration<LoggingOutputOptions>(hostBuilderContext.Configuration);
			})
			// We add the LoggerFactory so that the configuration providers can use loggers.
			.ConfigureHostConfiguration(b => b.Properties["HostLoggerFactory"] = hostServices.GetService<ILoggerFactory>()),
			settingsFolderPath,
			environmentManager
		);

		extraHostConfiguration?.Invoke(hostBuilder);

		_host = hostBuilder.Build();

		BuildHostActivity.Stop();

		ServiceProvider = _host.Services;

		OnInitialized(ServiceProvider);

		State.IsInitialized = true;

		Logger.LogInformation("Initialized core startup.");
	}

	/// <summary>
	/// Gets a <see cref="ILogger{TCategoryName}"/> typed to the implementator class.
	/// </summary>
	/// <param name="serviceProvider">The service provider from which the implemator class should obtain the logger.</param>
	/// <returns>The <see cref="ILogger{TCategoryName}"/> typed to the implementator class.</returns>
	protected abstract ILogger GetOrCreateLogger(IServiceProvider serviceProvider);

	/// <summary>
	/// Initializes services into the provided <see cref="IHostBuilder"/>.
	/// </summary>
	/// <param name="hostBuilder">The hostbuilder in which services must be added.</param>
	/// <param name="settingsFolderPath">The folder path indicating where the override files are.</param>
	/// <param name="environmentManager">The environment manager.</param>
	/// <returns>The original host builder with the new services added.</returns>
	protected abstract IHostBuilder InitializeServices(IHostBuilder hostBuilder, string settingsFolderPath, IEnvironmentManager environmentManager);

	/// <summary>
	/// This method will be called once the app is initialized.
	/// This is a chance to apply any configuration required to start the app.
	/// </summary>
	/// <param name="services">The services.</param>
	protected abstract void OnInitialized(IServiceProvider services);

	/// <summary>
	/// Starts the application.
	/// This method can be called multiple times.
	/// </summary>
	public async Task Start()
	{
		if (!State.IsInitialized)
		{
			throw new InvalidOperationException($"You must call {nameof(Initialize)} before calling '{nameof(Start)}'.");
		}

		Logger.LogDebug("Starting core startup.");

		var isFirstStart = !State.IsStarted;

		Logger.LogDebug("Starting services (isFirstStart: {IsFirstStart}).", isFirstStart);

		await StartServices(ServiceProvider, isFirstStart);

		Logger.LogInformation("Started services.");

		if (isFirstStart)
		{
			State.IsStarted = true;
		}

		Logger.LogInformation("Started core startup.");
	}

	/// <summary>
	/// Starts the services.
	/// This method can be called multiple times.
	/// This method will run on a background thread.
	/// </summary>
	/// <param name="services">The services.</param>
	/// <param name="isFirstStart">True if it's the first start; false otherwise.</param>
	/// <returns>Task that completes when the services are started.</returns>
	protected abstract Task StartServices(IServiceProvider services, bool isFirstStart);

	private IServiceProvider CreateHostServices(string contentRootPath, string settingsFolderPath, LoggingConfigurator loggingConfiguration, IEnvironmentManager environmentManager)
	{
		var coreHost = new HostBuilder()
			.UseContentRoot(contentRootPath)
			.AddConfiguration(settingsFolderPath, environmentManager)
			.ConfigureLogging((hostBuilderContext, loggingBuilder) =>
			{
				loggingConfiguration(hostBuilderContext, loggingBuilder, isAppLogging: false);
			})
			// Configures the default service provider for the host.
			.UseDefaultServiceProvider((context, options) =>
			{
				// Enable validation check in development mode.
				// Validates the service provider's configuration when it is built, ensuring all services can be created and there are no missing or circular dependencies.
				options.ValidateOnBuild = context.HostingEnvironment.IsDevelopment();
			})
			.Build();

		return coreHost.Services;
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Disposables.Dispose();
			_host.Dispose();
		}
	}
}

public delegate void LoggingConfigurator(HostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder, bool isAppLogging);
