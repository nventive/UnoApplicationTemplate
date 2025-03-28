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
	private readonly IUserProfileApiClient _profileRepository;

	public UserProfileService(IUserProfileApiClient profileRepository)
	{
		_profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
	}

	public async Task<UserProfile> GetCurrent(CancellationToken ct)
	{
		return UserProfile.FromData(await _profileRepository.Get(ct));
	}

	/// <inheritdoc/>
	public async Task Update(CancellationToken ct, UserProfile userProfile)
	{
		await _profileRepository.Update(ct, userProfile.ToData());
	}
}
