using System;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.DataAccess;

public sealed class AuthenticationToken
{
	public AuthenticationToken(string email, DateTimeOffset expiration, DateTimeOffset issuedAt)
	{
		Email = email;
		Expiration = expiration;
		IssuedAt = issuedAt;
	}

	[JsonPropertyName("unique_name")]
	public string Email { get; init; }

	[JsonPropertyName("exp")]
	[JsonConverter(typeof(UnixTimestampJsonConverter))]
	public DateTimeOffset Expiration { get; init; }

	[JsonPropertyName("iat")]
	[JsonConverter(typeof(UnixTimestampJsonConverter))]
	public DateTimeOffset IssuedAt { get; init; }
}
