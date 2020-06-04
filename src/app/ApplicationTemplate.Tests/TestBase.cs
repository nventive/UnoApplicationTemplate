using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace ApplicationTemplate.Tests
{
	public class TestBase : IAsyncLifetime
	{
		protected static readonly CancellationToken DefaultCancellationToken = CancellationToken.None;

		private readonly CoreStartup _coreStartup = new CoreStartup();

		public TestBase()
		{
			_coreStartup.PreInitialize();

			_coreStartup.Initialize();

			ConfigureSecurityProtocol();
		}

		public virtual async Task InitializeAsync()
		{
			await _coreStartup.Start();
		}

		protected virtual TService GetService<TService>()
		{
			return _coreStartup.ServiceProvider.GetRequiredService<TService>();
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
