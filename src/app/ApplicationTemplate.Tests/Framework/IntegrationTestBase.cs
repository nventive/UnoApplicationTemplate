using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Nventive.Persistence;
using Xunit;

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

		private CoreStartup _coreStartup;

		// This is called before every test
		public IntegrationTestBase()
		{
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
			coreStartup.Initialize(contentRootPath: string.Empty, settingsFolderPath: string.Empty, extraHostConfiguration);

			_coreStartup = coreStartup;

			ConfigureSecurityProtocol();
		}

		public virtual async Task InitializeAsync()
		{
			await _coreStartup.Start();
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

		/// <summary>
		/// This will fix an exception in test projects.
		/// The error is the following: Unable to read data from the transport connection : An existing connection was forcibly closed by the remote host.
		/// </summary>
		private static void ConfigureSecurityProtocol()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		public virtual async Task DisposeAsync()
		{
			await ClearStorage();
		}

		protected virtual async Task ClearStorage()
		{
			var dataPersister = GetService<IObservableDataPersister<ApplicationSettings>>();

			await dataPersister.Update(DefaultCancellationToken, d => d.RemoveAndCommit());
		}
	}

	/// <summary>
	/// Gives access to the services and their configuration with a specific reference to the tested service.
	/// </summary>
	/// <typeparam name="TSUT">The type of the service under test.</typeparam>
	public class IntegrationTestBase<TSUT> : IntegrationTestBase
	{
		protected virtual TSUT SUT => GetService<TSUT>();
	}
}
