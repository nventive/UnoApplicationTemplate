using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MallardMessageHandlers;
using Microsoft.Extensions.Options;

namespace ApplicationTemplate.DataAccess;

public sealed class UserProfileApiClientMock : BaseMock, IUserProfileApiClient
{
	private readonly IAuthenticationTokenProvider<AuthenticationData> _tokenProvider;
	private readonly IOptionsMonitor<MockOptions> _mockOptionsMonitor;

	public UserProfileApiClientMock(
		IAuthenticationTokenProvider<AuthenticationData> tokenProvider,
		JsonSerializerOptions serializerOptions,
		IOptionsMonitor<MockOptions> mockOptionsMonitor)
		: base(serializerOptions)
	{
		_tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		_mockOptionsMonitor = mockOptionsMonitor;
	}

	public async Task<UserProfileData> Get(CancellationToken ct)
	{
		var authenticationData = await _tokenProvider.GetToken(ct, request: null);

		if (authenticationData == default(AuthenticationData))
		{
			return default(UserProfileData);
		}

		await SimulateDelay(ct);

		return await GetTaskFromEmbeddedResource<UserProfileData>();
	}

	public async Task Update(CancellationToken ct, UserProfileData userProfile)
	{
		await SimulateDelay(ct);
	}

	private async Task SimulateDelay(CancellationToken ct)
	{
		if (_mockOptionsMonitor.CurrentValue.IsDelayForSimulatedApiCallsEnabled)
		{
			// We add a delay to simulate a long API call
			await Task.Delay(TimeSpan.FromSeconds(2), ct);
		}
	}
}
