using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeneratedSerializers;
using Uno.Extensions;

namespace ApplicationTemplate.Client
{
	public class AuthenticationEndpointMock : IAuthenticationEndpoint
	{
		private readonly IObjectSerializer _serializer;

		public AuthenticationEndpointMock(IObjectSerializer serializer)
		{
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
		}

		public async Task<AuthenticationData> CreateAccount(CancellationToken ct, string email, string password)
		{
			// We add a delay to simulate a long API call
			await Task.Delay(TimeSpan.FromSeconds(2));

			// We authenticate the user on account creation, since we don't have a backend to register and validate the user
			return CreateAuthenticationData();
		}

		public async Task ResetPassword(CancellationToken ct, string email)
		{
			// We add a delay to simulate a long API call
			await Task.Delay(TimeSpan.FromSeconds(2));
		}

		public async Task<AuthenticationData> Login(CancellationToken ct, string email, string password)
		{
			// We add a delay to simulate a long API call
			await Task.Delay(TimeSpan.FromSeconds(2));

			return CreateAuthenticationData();
		}

		public async Task<AuthenticationData> RefreshToken(CancellationToken ct, AuthenticationData unauthorizedToken)
		{
			if (unauthorizedToken is null)
			{
				throw new ArgumentNullException(nameof(unauthorizedToken));
			}

			// We add a delay to simulate a long API call
			await Task.Delay(TimeSpan.FromSeconds(2));

			return CreateAuthenticationData(unauthorizedToken.AccessToken.Payload);
		}

		private AuthenticationData CreateAuthenticationData(AuthenticationToken token = null, TimeSpan? timeToLive = null)
		{
			var encodedJwt = CreateJsonWebToken(token, timeToLive);
			var jwt = new JwtData<AuthenticationToken>(encodedJwt, _serializer);

			return new AuthenticationData.Builder
			{
				AccessToken = jwt,
				RefreshToken = Guid.NewGuid().ToStringInvariant(),
				Expiration = jwt.Payload.Expiration,
			};
		}

		private string CreateJsonWebToken(AuthenticationToken token = null, TimeSpan? timeToLive = null)
		{
			const string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"; // alg=HS256, type=JWT
			const string signature = "QWqnPP8W6ymexz74P6quP-oG-wxr7vMGqrEL8y_tV6M"; // dummy stuff

			var now = DateTimeOffset.Now;

			token = (token ?? AuthenticationToken.Default)
				.WithExpiration(now + (timeToLive ?? TimeSpan.FromMinutes(10)))
				.WithIssuedAt(now);

			string payload;
			using (var stream = new MemoryStream())
			{
				_serializer.WriteToStream(token, typeof(AuthenticationToken), stream, canDisposeStream: false);
				payload = Convert.ToBase64String(stream.ToArray());
			}

			return header + '.' + payload + '.' + signature;
		}
	}
}
