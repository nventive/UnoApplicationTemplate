using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeneratedSerializers;
using MallardMessageHandlers;

namespace ApplicationTemplate.Client
{
	public class UserProfileEndpointMock : BaseMock, IUserProfileEndpoint
	{
		private readonly IAuthenticationTokenProvider<AuthenticationData> _tokenProvider;

		public UserProfileEndpointMock(
			IObjectSerializer serializer,
			IAuthenticationTokenProvider<AuthenticationData> tokenProvider
		) : base(serializer)
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
}
