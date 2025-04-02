using System;
using System.Text.Json.Serialization;
using MallardMessageHandlers;

namespace ApplicationTemplate.DataAccess;

public sealed class AuthenticationData : IAuthenticationToken
{
	[JsonPropertyName("access_token")]
	[JsonConverter(typeof(JwtDataJsonConverter<AuthenticationToken>))]
	public JwtData<AuthenticationToken> AccessToken { get; init; }

	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; init; }

	[JsonIgnore]
	string IAuthenticationToken.AccessToken => AccessToken?.Token;

	[JsonIgnore]
	public bool CanBeRefreshed => !string.IsNullOrEmpty(RefreshToken);

	[JsonIgnore]
	public string Email => AccessToken?.Payload?.Email;

	[JsonIgnore]
	public DateTimeOffset? Expiration => AccessToken?.Payload?.Expiration;
}
