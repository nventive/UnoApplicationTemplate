using System;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.Client;

public sealed class AuthenticationToken
{
	public AuthenticationToken(string email, DateTimeOffset expiration, DateTimeOffset issuedAt)
	{
		Email = email;
		Expiration = expiration;
		IssuedAt = issuedAt;
	}

	public static AuthenticationToken Default { get; } = new AuthenticationToken(default, DateTimeOffset.MinValue, DateTimeOffset.MinValue);

	[JsonPropertyName("unique_name")]
	public string Email { get; }

	[JsonPropertyName("exp")]
	[JsonConverter(typeof(UnixTimestampJsonConverter))]
	public DateTimeOffset Expiration { get; } = DateTimeOffset.MinValue;

	[JsonPropertyName("iat")]
	[JsonConverter(typeof(UnixTimestampJsonConverter))]
	public DateTimeOffset IssuedAt { get; } = DateTimeOffset.MinValue;
}
