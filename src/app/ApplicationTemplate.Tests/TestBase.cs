using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Nventive.Persistence;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace ApplicationTemplate.Tests
{
	public class TestBase : IAsyncLifetime
	{
		protected static readonly CancellationToken DefaultCancellationToken = CancellationToken.None;
		protected static readonly CancellationToken AnyCancellationToken = It.IsAny<CancellationToken>();

		private readonly CoreStartup _coreStartup = new CoreStartup();

		public TestBase()
		{
			_coreStartup.PreInitialize();

			_coreStartup.Initialize(ConfigureHost);

			ConfigureSecurityProtocol();
		}

		public virtual async Task InitializeAsync()
		{
			await _coreStartup.Start();
		}

		/// <summary>
		/// A chance to configure the <paramref name="host"/> after its default configuration.
		/// </summary>
		/// <param name="host">Host builder</param>
		protected virtual void ConfigureHost(IHostBuilder host) { }

		/// <summary>
		/// Returns the requested service.
		/// </summary>
		/// <typeparam name="TService">Type of service</typeparam>
		/// <returns>Requested service</returns>
		protected virtual TService GetService<TService>()
		{
			return _coreStartup.ServiceProvider.GetRequiredService<TService>();
		}

		/// <summary>
		/// Replaces the registration for type <typeparamref name="TService"/> with a mocked implementation.
		/// </summary>
		/// <typeparam name="TService">Type of service</typeparam>
		/// <param name="services">Service collection</param>
		/// <param name="mockSetup">Mock configuration</param>
		/// <returns>Services</returns>
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

	public class TestBase<TSUT> : TestBase
	{
		protected virtual TSUT SUT => GetService<TSUT>();
	}
}
