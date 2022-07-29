using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Nventive.Persistence;

namespace ApplicationTemplate
{
	public class JsonSerializerToSettingsSerializerAdapter : ISettingsSerializer
	{
		private JsonSerializerOptions _options;

		public JsonSerializerToSettingsSerializerAdapter(JsonSerializerOptions options)
		{
			_options = options;
		}

		public object FromString(string source, Type targetType)
		{
			return JsonSerializer.Deserialize(source, targetType, _options);
		}

		public string ToString(object value, Type valueType)
		{
			return JsonSerializer.Serialize(value, valueType, _options);
		}
	}
}
