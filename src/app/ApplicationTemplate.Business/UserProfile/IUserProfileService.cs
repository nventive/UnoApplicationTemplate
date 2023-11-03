using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business;

/// <summary>
/// Provides access to the user profile.
/// </summary>
public interface IUserProfileService
{
	/// <summary>
	/// Gets the current user profile.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>The current <see cref="UserProfile"/>.</returns>
	Task<UserProfile> GetCurrent(CancellationToken ct);

	/// <summary>
	/// Updates the current user profile.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="userProfile">The <see cref="UserProfile"/>.</param>
	Task Update(CancellationToken ct, UserProfile userProfile);
}
