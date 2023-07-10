using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

	public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
	{
		if (content is null)
		{
			throw new ArgumentNullException(nameof(content));
		}

		var item = await content.ReadFromJsonAsync<T>(_serializerOptions, cancellationToken).ConfigureAwait(false);
		return item;
	}

	public string GetFieldNameForProperty(PropertyInfo propertyInfo)
	{
		if (propertyInfo is null)
		{
			throw new ArgumentNullException(nameof(propertyInfo));
		}

		return propertyInfo.GetCustomAttributes<JsonPropertyNameAttribute>(true)
			.Select(a => a.Name)
			.FirstOrDefault();
	}

	public HttpContent ToHttpContent<T>(T item)
	{
		var content = JsonContent.Create(item, options: _serializerOptions);
		return content;
	}
}
