using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the user profile API.
/// </summary>
[Headers("Authorization: Bearer")]
public interface IUserProfileRepository
{
	/// <summary>
	/// Returns the current user profile.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>The <see cref="UserProfileData"/>.</returns>
	[Get("/me")]
	Task<UserProfileData> Get(CancellationToken ct);

	/// <summary>
	/// Updates the current user profile.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="userProfile">The user profile.</param>
	[Put("/me")]
	Task Update(CancellationToken ct, UserProfileData userProfile);
}
