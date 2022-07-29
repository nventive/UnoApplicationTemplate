using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.Client
{
	public record AuthenticationToken
	{
		public static AuthenticationToken Default { get; } = new AuthenticationToken(default, DateTimeOffset.MinValue, DateTimeOffset.MinValue);

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
		public DateTimeOffset Expiration { get; init; } = DateTimeOffset.MinValue;

		[JsonPropertyName("iat")]
		[JsonConverter(typeof(UnixTimestampJsonConverter))]
		public DateTimeOffset IssuedAt { get; init; } = DateTimeOffset.MinValue;
	}
}
