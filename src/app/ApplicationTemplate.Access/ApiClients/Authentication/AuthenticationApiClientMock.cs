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
	private const int TokenExpirationSeconds = 600;

	private readonly JsonSerializerOptions _serializerOptions;
	private readonly IOptionsMonitor<MockOptions> _mockOptionsMonitor;
	private readonly TimeProvider _timeProvider;

	public AuthenticationApiClientMock(JsonSerializerOptions serializerOptions, IOptionsMonitor<MockOptions> mockOptionsMonitor, TimeProvider timeProvider)
	{
		_serializerOptions = serializerOptions;
		_mockOptionsMonitor = mockOptionsMonitor;
		_timeProvider = timeProvider;
	}

	public async Task<AuthenticationData> CreateAccount(CancellationToken ct, string email, string password)
	{
		await SimulateDelay(ct);

		// We authenticate the user on account creation, since we don't have a backend to register and validate the user.
		return CreateAuthenticationData(email: email);
	}

	public async Task ResetPassword(CancellationToken ct, string email)
	{
		await SimulateDelay(ct);
	}

	public async Task<AuthenticationData> Login(CancellationToken ct, string email, string password)
	{
		await SimulateDelay(ct);

		return CreateAuthenticationData(email: email);
	}

	public async Task<AuthenticationData> RefreshToken(CancellationToken ct, AuthenticationData unauthorizedToken)
	{
		ArgumentNullException.ThrowIfNull(unauthorizedToken);

		await SimulateDelay(ct);

		return CreateAuthenticationData(token: unauthorizedToken.AccessToken.Payload);
	}

	/// <summary>
	/// Creates a JSON Web Token.
	/// </summary>
	/// <remarks>
	/// This function has been made public and static for testing purposes.
	/// </remarks>
	/// <param name="token">The token to use.</param>
	/// <param name="email">The email or unique name to store in the token.</param>
	/// <param name="now">The current date and time to use for the authentication token.</param>
	/// <param name="serializerOptions">The serializer options to use for the token serialization.</param>
	/// <returns>The JSON Web token.</returns>
	public static string CreateJsonWebToken(AuthenticationToken token = null, string email = null, DateTimeOffset? now = null, JsonSerializerOptions serializerOptions = null)
	{
		const string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"; // alg=HS256, type=JWT
		const string signature = "QWqnPP8W6ymexz74P6quP-oG-wxr7vMGqrEL8y_tV6M"; // dummy stuff

		now ??= DateTimeOffset.Now;

		token ??= new AuthenticationToken()
		{
			Email = email,
			Expiration = now.Value.AddSeconds(TokenExpirationSeconds),
			IssuedAt = now.Value,
		};

		string payload;
		using (var stream = new MemoryStream())
		{
			var test = JsonSerializer.Serialize(token, serializerOptions);

			JsonSerializer.Serialize(stream, token, serializerOptions);
			payload = Convert.ToBase64String(stream.ToArray());
		}

		return header + '.' + payload + '.' + signature;
	}

	private AuthenticationData CreateAuthenticationData(AuthenticationToken token = null, string email = null)
	{
		var now = _timeProvider.GetLocalNow();
		var encodedJwt = CreateJsonWebToken(token, email, now, _serializerOptions);

		return new AuthenticationData()
		{
			AccessToken = new JwtData<AuthenticationToken>(encodedJwt, _serializerOptions),
			RefreshToken = Guid.NewGuid().ToString(format: null, CultureInfo.InvariantCulture),
		};
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
