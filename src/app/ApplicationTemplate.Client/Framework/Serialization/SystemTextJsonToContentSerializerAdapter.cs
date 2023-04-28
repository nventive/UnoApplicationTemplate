using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace ApplicationTemplate;

/// <summary>
/// This serializer adapter enables usage of the
/// System.Text.Json serializers with Refit.
/// </summary>
public class SystemTextJsonToContentSerializerAdapter : IHttpContentSerializer
{
	private static readonly MediaTypeHeaderValue _jsonMediaType = new MediaTypeHeaderValue("application/json") { CharSet = Encoding.UTF8.WebName };

	private readonly JsonSerializerOptions _serializerOptions;

	public SystemTextJsonToContentSerializerAdapter(JsonSerializerOptions serializerOptions = null)
	{
		_serializerOptions = serializerOptions;
	}

	public async Task<T> FromHttpContentAsync<T>(HttpContent content, CancellationToken ct)
	{
		if (content is null)
		{
			throw new ArgumentNullException(nameof(content));
		}

		using (var stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
		{
			return await JsonSerializer.DeserializeAsync<T>(stream, _serializerOptions).ConfigureAwait(false);
		}
	}

	public string GetFieldNameForProperty(PropertyInfo propertyInfo)
	{
		throw new NotImplementedException();
	}

	public HttpContent ToHttpContent<T>(T item)
	{
		var json = JsonSerializer.Serialize(item, _serializerOptions);
		var content = new StringContent(json);
		content.Headers.ContentType = _jsonMediaType;

		return content;
	}
}
