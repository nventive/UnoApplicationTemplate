using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApplicationTemplate;

public sealed class JwtDataJsonConverter<TPayload> : JsonConverter<JwtData<TPayload>>
	where TPayload : class
{
	public override JwtData<TPayload> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var token = reader.GetString();

		return new JwtData<TPayload>(token, options);
	}

	public override void Write(Utf8JsonWriter writer, JwtData<TPayload> value, JsonSerializerOptions options)
	{
		if (value?.Token is null)
		{
			writer.WriteNullValue();
		}
		else
		{
			writer.WriteStringValue(value.Token);
		}
	}
}
