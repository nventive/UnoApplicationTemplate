using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;
using ReviewService;

namespace ApplicationTemplate;

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
			.AddSingleton(s => CreateDataPersister(s, defaultValue: new ReviewSettings()))
			.AddSingleton(s => CreateObservableDataPersister(s, defaultValue: ApplicationSettings.Default));
	}

	public static IDataPersister<T> CreateDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
	{
		// Tests projects must not use any real persistence (files on disk).
		return new MemoryDataPersister<T>(defaultValue)
			.ToObservablePersister(services.GetRequiredService<IBackgroundScheduler>());
	}

	public static IObservableDataPersister<T> CreateObservableDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
	{
		return CreateDataPersister(services, defaultValue)
			.ToObservablePersister(services.GetRequiredService<IBackgroundScheduler>());
	}
}
