using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the authentication API.
/// </summary>
public interface IAuthenticationRepository
{
	/// <summary>
	/// Logs the user in using the provided <paramref name="email"/> and <paramref name="password"/>.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="email">The email.</param>
	/// <param name="password">The password.</param>
	/// <returns>The <see cref="AuthenticationData"/>.</returns>
	Task<AuthenticationData> Login(CancellationToken ct, string email, string password);

	/// <summary>
	/// Refreshes the user token.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="unauthorizedToken">The unauthorized token.</param>
	/// <returns>The <see cref="AuthenticationData"/>.</returns>
	Task<AuthenticationData> RefreshToken(CancellationToken ct, AuthenticationData unauthorizedToken);

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
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="email">The email.</param>
	Task ResetPassword(CancellationToken ct, string email);
}
