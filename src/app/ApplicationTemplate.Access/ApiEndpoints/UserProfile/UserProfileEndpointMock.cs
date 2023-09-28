using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MallardMessageHandlers;

namespace ApplicationTemplate.DataAccess;

public class UserProfileEndpointMock : BaseMock, IUserProfileEndpoint
{
	private readonly IAuthenticationTokenProvider<AuthenticationData> _tokenProvider;

	public UserProfileEndpointMock(
		IAuthenticationTokenProvider<AuthenticationData> tokenProvider,
		JsonSerializerOptions serializerOptions)
		: base(serializerOptions)
	{
		_tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
	}

	public async Task<UserProfileData> Get(CancellationToken ct)
	{
		var authenticationData = await _tokenProvider.GetToken(ct, request: null);

		if (authenticationData == default(AuthenticationData))
		{
			return default(UserProfileData);
		}

		await Task.Delay(TimeSpan.FromSeconds(2), ct);

		return await GetTaskFromEmbeddedResource<UserProfileData>();
	}

	public async Task Update(CancellationToken ct, UserProfileData userProfile)
	{
		await Task.Delay(TimeSpan.FromSeconds(2), ct);
	}
}
