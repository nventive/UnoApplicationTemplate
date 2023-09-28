using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.DataAccess;

public class AuthenticationToken
{
	public AuthenticationToken() { }

	public AuthenticationToken(string email, DateTimeOffset expiration, DateTimeOffset issuedAt)
	{
		Email = email;
		Expiration = expiration;
		IssuedAt = issuedAt;
	}

	[JsonPropertyName("unique_name")]
	public string Email { get; set; }

	[JsonPropertyName("exp")]
	[JsonConverter(typeof(UnixTimestampJsonConverter))]
	public DateTimeOffset Expiration { get; set; }

	[JsonPropertyName("iat")]
	[JsonConverter(typeof(UnixTimestampJsonConverter))]
	public DateTimeOffset IssuedAt { get; set; }
}
