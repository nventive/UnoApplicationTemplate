using System;
using System.Text.Json.Serialization;
using MallardMessageHandlers;

namespace ApplicationTemplate.Client;

public sealed class AuthenticationData : IAuthenticationToken
{
	public AuthenticationData(string accessToken = default, string refreshToken = default, DateTimeOffset expiration = default)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
		Expiration = expiration;
	}

	[JsonIgnore]
	public AuthenticationToken AccessTokenPayload => AccessToken == null ? null : new JwtData<AuthenticationToken>(AccessToken).Payload;

	[JsonPropertyName("access_token")]
	public string AccessToken { get; }

	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; }

	public DateTimeOffset Expiration { get; }

	public string Email => AccessTokenPayload?.Email;

	[JsonIgnore]
	public bool CanBeRefreshed => !string.IsNullOrEmpty(RefreshToken);

	[JsonIgnore]
	string IAuthenticationToken.AccessToken => AccessToken;
}
