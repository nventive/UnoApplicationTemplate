using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using Chinook.DynamicMvvm;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Nventive.Persistence;
using Serilog;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace ApplicationTemplate.Tests
{
	/// <summary>
	/// Gives access to the services and their configuration.
	/// </summary>
	public class IntegrationTestBase : IAsyncLifetime
	{
		protected static readonly CancellationToken DefaultCancellationToken = CancellationToken.None;
		protected static readonly CancellationToken AnyCancellationToken = It.IsAny<CancellationToken>();
		private readonly ITestOutputHelper _output;
		private CoreStartup _coreStartup;

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestBase"/> class.
		/// </summary>
		/// <remarks>
		/// This is called before every test.
		/// </remarks>
		/// <param name="output">Optional output parameter. Provide it when you want to consult the logs of this test.</param>
		public IntegrationTestBase(ITestOutputHelper output = null)
		{
			_output = output;
			InitializeServices(ConfigureHost);
		}

		/// <summary>
		/// Initializes different services used by the app.
		/// </summary>
		/// <param name="extraHostConfiguration">Add specific configurations for app initialization.</param>
		protected void InitializeServices(Action<IHostBuilder> extraHostConfiguration = null)
		{
			var coreStartup = new CoreStartup();
			coreStartup.PreInitialize();
			coreStartup.Initialize(contentRootPath: string.Empty, settingsFolderPath: string.Empty, ConfigureLogging, extraHostConfiguration);

			_coreStartup = coreStartup;
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
				.WriteTo.TestOutput(_output, outputTemplate: "{Timestamp:HH:mm:ss.fff} Thread:{ThreadId} {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}");

			var logger = serilogConfiguration.CreateLogger();
			loggingBuilder.AddSerilog(logger);
		}

		public virtual async Task InitializeAsync()
		{
			await _coreStartup.Start();

			// This sometimes fails when parallelization is enabled. (That's one of the reasons why parallelization is disabled.)
			ViewModelBase.DefaultServiceProvider.Should().BeSameAs(_coreStartup.ServiceProvider, because: "We want the ViewModels of this test to use the services that we just initialized.");
		}

		/// <summary>
		/// A chance to configure the <paramref name="host"/> after its default configuration.
		/// </summary>
		/// <param name="host">The host builder.</param>
		protected void ConfigureHost(IHostBuilder host)
		{
			AddThreadConfiguration(host);
		}

		/// <summary>
		/// Adds instances of thread related interfaces in the IoC.
		/// </summary>
		/// <param name="host">The host builder.</param>
		private void AddThreadConfiguration(IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				services
					.AddSingleton<IScheduler>(s => TaskPoolScheduler.Default)
					.AddSingleton<IDispatcherScheduler>(s => new DispatcherSchedulerAdapter(
						s.GetRequiredService<IScheduler>()
					));
			});
		}

		/// <summary>
		/// Returns the requested service.
		/// </summary>
		/// <typeparam name="TService">Type of service.</typeparam>
		/// <returns>The requested service.</returns>
		protected virtual TService GetService<TService>()
		{
			return _coreStartup.ServiceProvider.GetRequiredService<TService>();
		}

		/// <summary>
		/// Replaces the registration for type <typeparamref name="TService"/> with a mocked implementation.
		/// </summary>
		/// <typeparam name="TService">The type of service.</typeparam>
		/// <param name="services">The service collection.</param>
		/// <param name="mockSetup">The mock configuration.</param>
		/// <returns>the redistered services.</returns>
		protected virtual IServiceCollection ReplaceWithMock<TService>(IServiceCollection services, Action<Mock<TService>> mockSetup)
			 where TService : class
		{
			var mockedService = new Mock<TService>();

			mockSetup(mockedService);

			return services.Replace(ServiceDescriptor.Singleton(mockedService.Object));
		}

		public virtual async Task DisposeAsync()
		{
			_coreStartup.Dispose();
		}
	}

	/// <summary>
	/// Gives access to the services and their configuration with a specific reference to the tested service.
	/// </summary>
	/// <typeparam name="TSUT">The type of the service under test.</typeparam>
	public class IntegrationTestBase<TSUT> : IntegrationTestBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestBase{TSUT}"/> class.
		/// </summary>
		/// <param name="output">Optional output parameter. Provide it when you want to consult the logs of this test.</param>
		public IntegrationTestBase(ITestOutputHelper output = null) : base(output)
		{
		}

		protected virtual TSUT SUT => GetService<TSUT>();
	}
}
