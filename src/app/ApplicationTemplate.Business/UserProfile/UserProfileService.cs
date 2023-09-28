using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate.Business;

public partial class UserProfileService : IUserProfileService
{
	private readonly IUserProfileEndpoint _profileEndpoint;

	public UserProfileService(IUserProfileEndpoint profileEndpoint)
	{
		_profileEndpoint = profileEndpoint ?? throw new ArgumentNullException(nameof(profileEndpoint));
	}

	public async Task<UserProfile> GetCurrent(CancellationToken ct)
	{
		return UserProfile.FromData(await _profileEndpoint.Get(ct));
	}

	/// <inheritdoc/>
	public async Task Update(CancellationToken ct, UserProfile userProfile)
	{
		await _profileEndpoint.Update(ct, userProfile.ToData());
	}
}
