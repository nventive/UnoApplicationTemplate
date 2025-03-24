using System;
using System.Reactive.Concurrency;
using System.Text.Json;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;
using ReviewService;

namespace ApplicationTemplate.Views;

/// <summary>
/// This class is used for persistence configuration.
/// - Configures the review settings.
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
			.AddSingleton(s => CreateDataPersister<ReviewSettings>(s, SerializationConfiguration.NoSourceGenerationJsonSerializerOptions))
			.AddSingleton(s => CreateSecureDataPersister<ApplicationSettings>(s));
	}

	private static IObservableDataPersister<T> CreateSecureDataPersister<T>(IServiceProvider services)
	{
//-:cnd:noEmit
#if __ANDROID__
//+:cnd:noEmit
		return new SettingsStorageObservableDataPersisterAdapter<T>(
			storage: new KeyStoreSettingsStorage(
				services.GetRequiredService<ISettingsSerializer>(),
				Uno.UI.ContextHelper.Current.GetFileStreamPath(typeof(T).Name).AbsolutePath
			),
			key: typeof(T).Name,
			comparer: null,
			concurrencyProtection: false
		);
//-:cnd:noEmit
#elif __IOS__
//+:cnd:noEmit
		return new SettingsStorageObservableDataPersisterAdapter<T>(
			storage: new KeychainSettingsStorage(services.GetRequiredService<ISettingsSerializer>()),
			key: typeof(T).Name,
			comparer: null,
			concurrencyProtection: false
		);
//-:cnd:noEmit
#else
//+:cnd:noEmit
		return CreateObservableDataPersister<T>(services);
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}

	private static IDataPersister<T> CreateDataPersister<T>(IServiceProvider services, JsonSerializerOptions jsonSerializerOptions = null)
	{
		return UnoDataPersister.CreateFromFile<T>(
			FolderType.WorkingData,
			typeof(T).Name + ".json",
			async (ct, s) => await JsonSerializer.DeserializeAsync<T>(s, options: jsonSerializerOptions ?? services.GetService<JsonSerializerOptions>(), ct),
			async (ct, v, s) => await JsonSerializer.SerializeAsync<T>(s, v, options: jsonSerializerOptions ?? services.GetService<JsonSerializerOptions>(), ct)
		);
	}

	private static IObservableDataPersister<T> CreateObservableDataPersister<T>(IServiceProvider services)
	{
		return CreateDataPersister<T>(services)
			.ToObservablePersister(services.GetRequiredService<IBackgroundScheduler>());
	}
}
