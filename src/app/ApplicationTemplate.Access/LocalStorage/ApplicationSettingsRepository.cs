using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nventive.Persistence;

namespace ApplicationTemplate.DataAccess;

public partial class ApplicationSettingsRepository : IApplicationSettingsRepository
{
	private readonly IObservableDataPersister<ApplicationSettings> _dataPersister;

	public ApplicationSettingsRepository(IObservableDataPersister<ApplicationSettings> dataPersister)
	{
		_dataPersister = dataPersister ?? throw new ArgumentNullException(nameof(dataPersister));
	}

	/// <inheritdoc />
	public async Task<ApplicationSettings> GetCurrent(CancellationToken ct)
	{
		var result = await _dataPersister.Load(ct);

		return result.Value ?? ApplicationSettings.Default;
	}

	/// <inheritdoc />
	public IObservable<ApplicationSettings> GetAndObserveCurrent()
	{
		return _dataPersister.GetAndObserve().Select(r => r.Value ?? ApplicationSettings.Default);
	}

	/// <inheritdoc />
	public async Task CompleteOnboarding(CancellationToken ct)
	{
		await Update(ct, s => s with { IsOnboardingCompleted = true });
	}

	/// <inheritdoc />
	public async Task SetAuthenticationData(CancellationToken ct, AuthenticationData authenticationData)
	{
		await Update(ct, s => s with { AuthenticationData = authenticationData });
	}

	/// <inheritdoc />
	public async Task SetFavoriteQuotes(CancellationToken ct, ImmutableDictionary<string, FavoriteJokeData> quotes)
	{
		await Update(ct, s => s with { FavoriteQuotes = quotes });
	}

	/// <inheritdoc />
	public async Task DiscardUserSettings(CancellationToken ct)
	{
		await Update(ct, s => s with
		{
			FavoriteQuotes = ImmutableDictionary<string, FavoriteJokeData>.Empty,
			AuthenticationData = default(AuthenticationData)
		});
	}

	private async Task Update(CancellationToken ct, Func<ApplicationSettings, ApplicationSettings> updateFunction)
	{
		await _dataPersister.Update(ct, context =>
		{
			var settings = context.GetReadValueOrDefault(ApplicationSettings.Default);

			context.Commit(updateFunction(settings));
		});
	}
}
