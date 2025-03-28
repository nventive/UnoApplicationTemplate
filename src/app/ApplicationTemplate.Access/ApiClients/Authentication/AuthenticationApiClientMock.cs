using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ApplicationTemplate.DataAccess;

public sealed class AuthenticationApiClientMock : IAuthenticationApiClient
{
	private readonly JsonSerializerOptions _serializerOptions;
	private readonly IOptionsMonitor<MockOptions> _mockOptionsMonitor;

	public AuthenticationApiClientMock(JsonSerializerOptions serializerOptions, IOptionsMonitor<MockOptions> mockOptionsMonitor)
	{
		_serializerOptions = serializerOptions;
		_mockOptionsMonitor = mockOptionsMonitor;
	}

	public async Task<AuthenticationData> CreateAccount(CancellationToken ct, string email, string password)
	{
		await SimulateDelay(ct);

		// We authenticate the user on account creation, since we don't have a backend to register and validate the user
		return CreateAuthenticationData();
	}

	public async Task ResetPassword(CancellationToken ct, string email)
	{
		await SimulateDelay(ct);
	}

	public async Task<AuthenticationData> Login(CancellationToken ct, string email, string password)
	{
		await SimulateDelay(ct);

		return CreateAuthenticationData();
	}

	public async Task<AuthenticationData> RefreshToken(CancellationToken ct, AuthenticationData unauthorizedToken)
	{
		ArgumentNullException.ThrowIfNull(unauthorizedToken);

		await SimulateDelay(ct);

		return CreateAuthenticationData(unauthorizedToken.AccessToken.Payload);
	}

	private AuthenticationData CreateAuthenticationData(AuthenticationToken token = null)
	{
		var encodedJwt = CreateJsonWebToken(token);

		return new AuthenticationData()
		{
			AccessToken = new JwtData<AuthenticationToken>(encodedJwt, _serializerOptions),
			RefreshToken = Guid.NewGuid().ToString(format: null, CultureInfo.InvariantCulture),
		};
	}

	private string CreateJsonWebToken(AuthenticationToken token = null)
	{
		const string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"; // alg=HS256, type=JWT
		const string signature = "QWqnPP8W6ymexz74P6quP-oG-wxr7vMGqrEL8y_tV6M"; // dummy stuff

		token ??= new AuthenticationToken(default, DateTimeOffset.MinValue, DateTimeOffset.MinValue);

		string payload;
		using (var stream = new MemoryStream())
		{
			JsonSerializer.Serialize(stream, token, _serializerOptions);
			payload = Convert.ToBase64String(stream.ToArray());
		}

		return header + '.' + payload + '.' + signature;
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
