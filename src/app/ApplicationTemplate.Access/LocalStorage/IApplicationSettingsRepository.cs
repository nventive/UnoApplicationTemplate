using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the local storage.
/// </summary>
public interface IApplicationSettingsRepository
{
	/// <summary>
	/// Gets and observes the current <see cref="ApplicationSettings"/>.
	/// </summary>
	/// <returns>An observable sequence yielding the current <see cref="ApplicationSettings"/>.</returns>
	IObservable<ApplicationSettings> GetAndObserveCurrent();

	/// <summary>
	/// Gets the current <see cref="ApplicationSettings"/>.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>The current <see cref="ApplicationSettings"/>.</returns>
	Task<ApplicationSettings> GetCurrent(CancellationToken ct);

	/// <summary>
	/// Discards any settings that are related to the user.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	Task DiscardUserSettings(CancellationToken ct);

	/// <summary>
	/// Flags that the onboarding has been completed.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	Task CompleteOnboarding(CancellationToken ct);

	/// <summary>
	/// Sets the current <see cref="AuthenticationData"/>.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="authenticationData">The <see cref="AuthenticationData"/>.</param>
	Task SetAuthenticationData(CancellationToken ct, AuthenticationData authenticationData);

	/// <summary>
	/// Sets the favorite quotes.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="quotes">The favorite quotes.</param>
	Task SetFavoriteQuotes(CancellationToken ct, ImmutableDictionary<string, FavoriteJokeData> quotes);
}
