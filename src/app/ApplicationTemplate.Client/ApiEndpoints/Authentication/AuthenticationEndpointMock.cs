using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Client;

public class AuthenticationEndpointMock : IAuthenticationEndpoint
{
	private readonly JsonSerializerOptions _serializerOptions;

	public AuthenticationEndpointMock(JsonSerializerOptions serializerOptions)
	{
		_serializerOptions = serializerOptions;
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

		return CreateAuthenticationData(email);
	}

	public async Task<AuthenticationData> RefreshToken(CancellationToken ct, AuthenticationData unauthorizedToken)
	{
		if (unauthorizedToken is null)
		{
			throw new ArgumentNullException(nameof(unauthorizedToken));
		}

		// We add a delay to simulate a long API call
		await Task.Delay(TimeSpan.FromSeconds(2));

		return CreateAuthenticationData(token: unauthorizedToken.AccessTokenPayload);
	}

	private AuthenticationData CreateAuthenticationData(string email = null, AuthenticationToken token = null, TimeSpan? timeToLive = null)
	{
		var encodedJwt = CreateJsonWebToken(email, token, timeToLive);
		var jwt = new JwtData<AuthenticationToken>(encodedJwt, _serializerOptions);

		return new AuthenticationData(jwt.Token, Guid.NewGuid().ToString(format: null, CultureInfo.InvariantCulture), jwt.Payload.Expiration);
	}

	private string CreateJsonWebToken(string email = null, AuthenticationToken token = null, TimeSpan? timeToLive = null)
	{
		const string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"; // alg=HS256, type=JWT
		const string signature = "QWqnPP8W6ymexz74P6quP-oG-wxr7vMGqrEL8y_tV6M"; // dummy stuff

		var now = DateTimeOffset.Now;

		token = token ?? new AuthenticationToken(default, DateTimeOffset.MinValue, DateTimeOffset.MinValue);

		string payload;
		using (var stream = new MemoryStream())
		{
			JsonSerializer.Serialize(stream, token, _serializerOptions);
			payload = Convert.ToBase64String(stream.ToArray());
		}

		return header + '.' + payload + '.' + signature;
	}
}
