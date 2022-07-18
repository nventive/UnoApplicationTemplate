using System;
using System.Text.Json.Serialization;
using MallardMessageHandlers;

namespace ApplicationTemplate.Client
{
	public class AuthenticationData : IAuthenticationToken
	{
		[JsonPropertyName("access_token")]
		[JsonConverter(typeof(JwtDataJsonConverter<AuthenticationToken>))]
		public JwtData<AuthenticationToken> AccessToken { get; init; }

		[JsonPropertyName("refresh_token")]
		public string RefreshToken { get; init; }

		public DateTimeOffset? Expiration { get; init; }

		public string Email => AccessToken?.Payload?.Email;

		[JsonIgnore]
		public bool CanBeRefreshed => !string.IsNullOrEmpty(RefreshToken);

		[JsonIgnore]
		string IAuthenticationToken.AccessToken => AccessToken?.Token;
	}
}
