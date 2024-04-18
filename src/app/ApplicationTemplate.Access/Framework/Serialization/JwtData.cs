using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ApplicationTemplate;

/// <summary>
/// Specialized type to hold RFC 7519 JSON Web Token (JWT) information.
/// </summary>
/// <typeparam name="TPayload">The type of the Payload to deserialize.</typeparam>
public class JwtData<TPayload>
	where TPayload : class
{
	private readonly JsonSerializerOptions _jsonSerializerOptions;

	private IDictionary<string, string> _header;
	private TPayload _payload;

	/// <summary>
	/// Initializes a new instance of the <see cref="JwtData{TPayload}"/> class.
	/// </summary>
	/// <remarks>
	/// Header and Payload will be deserialized only if a JSON serializer is supplied.
	/// </remarks>
	/// <param name="token">The raw token.</param>
	/// <param name="jsonSerializerOptions">Should be a JSON serializer. Using a serializer for another format won't be RFC 7519 compliant.</param>
	public JwtData(string token, JsonSerializerOptions jsonSerializerOptions = null)
	{
		_jsonSerializerOptions = jsonSerializerOptions;
		Token = token;

		var parts = token?.Split(new[] { '.' });
		RawHeader = parts?.Length > 0 ? Base64DecodeToString(parts[0]) : null;
		RawPayload = parts?.Length > 1 ? Base64DecodeToString(parts[1]) : null;
		Signature = parts?.Length > 2 ? Base64Decode(parts[2]) : null;
	}

	/// <summary>
	/// Gets the raw token as received as constructor parameter.
	/// </summary>
	public string Token { get; }

	/// <summary>
	/// Gets the decoded (but non-deserialized) header part of the JWT - in JSON text.
	/// </summary>
	public string RawHeader { get; }

	/// <summary>
	/// Gets the decoded (but non-deserialized) payload part of the JWT - in JSON text.
	/// </summary>
	public string RawPayload { get; }

	/// <summary>
	/// Gets the deserialized header.
	/// </summary>
	public IDictionary<string, string> Header
		=> _header ?? (_header = JsonSerializer.Deserialize(RawHeader, typeof(IDictionary<string, string>), _jsonSerializerOptions) as IDictionary<string, string>);

	/// <summary>
	/// Gets the deserialized payload.
	/// </summary>
	public TPayload Payload
		=> _payload ?? (_payload = JsonSerializer.Deserialize(RawPayload, typeof(TPayload), _jsonSerializerOptions) as TPayload);

	/// <summary>
	/// Gets the decoded signature of the JWT.
	/// </summary>
	public byte[] Signature { get; }

	private static string Base64DecodeToString(string input)
	{
		return Encoding.UTF8.GetString(Base64Decode(input));
	}

	private static byte[] Base64Decode(string input)
	{
		var output = input?.Replace('-', '+').Replace('_', '/') ?? string.Empty;

		// Pad with trailing '='s
		switch (output.Length % 4)
		{
			case 0: break; // No pad chars in this case
			case 2:
				output += "==";
				break; // Two pad chars
			case 3:
				output += "=";
				break; // One pad char
			default:
				throw new ArgumentException("Illegal base64url string!", nameof(input));
		}

		return Convert.FromBase64String(output);
	}
}
