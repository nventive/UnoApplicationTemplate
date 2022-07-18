using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;

namespace ApplicationTemplate
{
	/// <summary>
	/// This class is used for persistence configuration.
	/// - Configures the application settings.
	/// </summary>
	public static class PersistenceConfiguration
	{
		/// <summary>
		/// Adds the persistence services to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">Service collection.</param>
		/// <returns><see cref="IServiceCollection"/>.</returns>
		public static IServiceCollection AddPersistence(this IServiceCollection services)
		{
			return services
				.AddSingleton(s => CreateDataPersister(s, defaultValue: ApplicationSettings.Default));
		}

		private static IObservableDataPersister<T> CreateDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
		{
			// Tests projects must not use any real persistence (files on disc).
			return new MemoryDataPersister<T>(defaultValue)
				.ToObservablePersister(services.GetRequiredService<IBackgroundScheduler>());
		}
	}
}
