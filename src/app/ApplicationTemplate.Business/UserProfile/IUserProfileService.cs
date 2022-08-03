using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business
{
	public interface IUserProfileService
	{
		/// <summary>
		/// Gets the current user profile.
		/// </summary>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <returns><see cref="UserProfile"/></returns>
		Task<UserProfile> GetCurrent(CancellationToken ct);

		/// <summary>
		/// Updates the current user profile.
		/// </summary>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <param name="userProfile"><see cref="UserProfile"/></param>
		/// <returns><see cref="Task"/></returns>
		Task Update(CancellationToken ct, UserProfile userProfile);
	}
}
