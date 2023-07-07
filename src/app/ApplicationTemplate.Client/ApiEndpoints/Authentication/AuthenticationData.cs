using System;
using System.Text.Json.Serialization;
using MallardMessageHandlers;

namespace ApplicationTemplate.Client;

public class AuthenticationData : IAuthenticationToken
{
	public AuthenticationData() { }

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
	public string AccessToken { get; set; }

	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; set; }

	public DateTimeOffset Expiration { get; set; }

	public string Email => AccessTokenPayload?.Email;

	[JsonIgnore]
	public bool CanBeRefreshed => !string.IsNullOrEmpty(RefreshToken);

	[JsonIgnore]
	string IAuthenticationToken.AccessToken => AccessToken;
}
