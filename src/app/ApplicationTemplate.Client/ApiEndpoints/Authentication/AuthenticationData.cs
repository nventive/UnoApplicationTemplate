using System;
using System.Text.Json.Serialization;
using MallardMessageHandlers;

namespace ApplicationTemplate.Client;

public class AuthenticationData : IAuthenticationToken
{
	public AuthenticationData(
		string accessToken = default,
		string refreshToken = default,
		DateTimeOffset expiration = default)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
		Expiration = expiration;
	}

	[JsonIgnore]
	public AuthenticationToken AccessTokenPayload => AccessToken == null ? null : new JwtData<AuthenticationToken>(AccessToken).Payload;

	[JsonPropertyName("access_token")]
	public string AccessToken { get; init; }

	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; init; }

	public DateTimeOffset Expiration { get; init; }

	public string Email => AccessTokenPayload?.Email;

	[JsonIgnore]
	public bool CanBeRefreshed => !string.IsNullOrEmpty(RefreshToken);

	[JsonIgnore]
	string IAuthenticationToken.AccessToken => AccessToken;
}
