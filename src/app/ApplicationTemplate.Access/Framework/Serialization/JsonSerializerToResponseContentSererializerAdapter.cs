using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MallardMessageHandlers;

namespace ApplicationTemplate;

public sealed class JsonSerializerToResponseContentSererializerAdapter : IResponseContentDeserializer
{
	private readonly JsonSerializerOptions _serializerOptions;

	public JsonSerializerToResponseContentSererializerAdapter(JsonSerializerOptions serializerOptions)
	{
		_serializerOptions = serializerOptions;
	}

	public async Task<TResponse> Deserialize<TResponse>(CancellationToken ct, HttpContent content)
	{
		ArgumentNullException.ThrowIfNull(content);

		using (var stream = await content.ReadAsStreamAsync())
		{
			return await JsonSerializer.DeserializeAsync<TResponse>(stream, _serializerOptions);
		}
	}
}
