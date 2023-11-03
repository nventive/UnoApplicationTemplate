using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using MallardMessageHandlers;

namespace ApplicationTemplate.Business;

/// <summary>
/// Manages the authentication.
/// </summary>
public interface IAuthenticationService : IAuthenticationTokenProvider<AuthenticationData>
{
	/// <summary>
	/// Gets and observes the current <see cref="AuthenticationData"/>.
	/// </summary>
	/// <returns>An observable sequence containing the current <see cref="AuthenticationData"/>.</returns>
	IObservable<AuthenticationData> GetAndObserveAuthenticationData();

	/// <summary>
	/// Gets a boolean indicating whether or not the user is currently authenticated.
	/// </summary>
	/// <returns>Whether or not the user is currently authenticated.</returns>
	IObservable<bool> GetAndObserveIsAuthenticated();

	/// <summary>
	/// Raised when the user has been automatically logged out because the session expired.
	/// </summary>
	/// <returns>A observable sequence notifying whenever the session expires.</returns>
	IObservable<Unit> ObserveSessionExpired();

	/// <summary>
	/// Logs the user in using the provided <paramref name="email"/> and <paramref name="password"/>.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="email">The email.</param>
	/// <param name="password">The password.</param>
	/// <returns>The <see cref="AuthenticationData"/>.</returns>
	Task<AuthenticationData> Login(CancellationToken ct, string email, string password);

	/// <summary>
	/// Logs the user out.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	Task Logout(CancellationToken ct);

	/// <summary>
	/// Creates a user account.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="email">The email.</param>
	/// <param name="password">The password.</param>
	/// <returns>The <see cref="AuthenticationData"/>.</returns>
	Task<AuthenticationData> CreateAccount(CancellationToken ct, string email, string password);

	/// <summary>
	/// Resets the password.
	/// </summary>
	/// <param name="ct">THe <see cref="CancellationToken"/>.</param>
	/// <param name="email">The email.</param>
	Task ResetPassword(CancellationToken ct, string email);
}
