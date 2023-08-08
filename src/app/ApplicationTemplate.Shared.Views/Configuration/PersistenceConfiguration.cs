using System;
using System.Reactive.Concurrency;
using System.Text.Json;
using ApplicationTemplate.Client;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;
using ReviewService;

namespace ApplicationTemplate.Views;

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
			.AddSingleton(s => CreateDataPersister(s, defaultValue: new ReviewSettings(), SerializationConfiguration.NoSourceGenerationJsonSerializerOptions))
			.AddSingleton(s => CreateSecureDataPersister(s, defaultValue: ApplicationSettings.Default));
	}

	private static IObservableDataPersister<T> CreateSecureDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
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
		return CreateObservableDataPersister(services, defaultValue);
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}

	private static IDataPersister<T> CreateDataPersister<T>(IServiceProvider services, T defaultValue = default(T), JsonSerializerOptions jsonSerializerOptions = null)
	{
		return UnoDataPersister.CreateFromFile<T>(
			FolderType.WorkingData,
			typeof(T).Name + ".json",
			async (ct, s) => await JsonSerializer.DeserializeAsync<T>(s, options: jsonSerializerOptions ?? services.GetService<JsonSerializerOptions>(), ct),
			async (ct, v, s) => await JsonSerializer.SerializeAsync<T>(s, v, options: jsonSerializerOptions ?? services.GetService<JsonSerializerOptions>(), ct)
		);
	}

	private static IObservableDataPersister<T> CreateObservableDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
	{
		return CreateDataPersister(services, defaultValue)
			.ToObservablePersister(services.GetRequiredService<IBackgroundScheduler>());
	}
}
