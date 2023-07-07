using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.Client;

public class AuthenticationToken
{
	public AuthenticationToken(string unique_name, DateTimeOffset exp, DateTimeOffset iat)
	{
		Email = unique_name;
		Expiration = exp;
		IssuedAt = iat;
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
