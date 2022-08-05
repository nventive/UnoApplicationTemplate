using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate
{
	/// <summary>
	/// This implementation of <see cref="JsonConfigurationProvider"/> overrides its file when <see cref="Set(string, string)"/> is called.
	/// </summary>
	public class WritableJsonConfigurationProvider : JsonConfigurationProvider
	{
		private ILogger _logger;

		public WritableJsonConfigurationProvider(JsonConfigurationSource source, ILogger logger) : base(source)
		{
			_logger = logger;
		}

		public override void Set(string key, string value)
		{
			base.Set(key, value);

			var filePath = Source.FileProvider.GetFileInfo(Source.Path).PhysicalPath;
			var stopwatch = new Stopwatch();

			using (var writer = File.CreateText(filePath))
			{
				try
				{
					stopwatch.Start();
					var json = JsonSerializer.Serialize(Data, options: SerializationConfiguration.DefaultJsonSerializerOptions);
					stopwatch.Stop();
					writer.Write(json);
				}
				catch
				{
					stopwatch.Stop();
					if (writer.BaseStream.Position == 0)
					{
						writer.Dispose();
						// Don't keep the file if it's empty because it won't load properly on next launch.
						File.Delete(filePath);
					}
					throw;
				}
			}

			_logger.LogDebug("Serialized ­­­­{PairCount} key-value-pairs in {ElapsedMilliseconds}ms.", Data.Count, stopwatch.ElapsedMilliseconds);

			OnReload();
		}
	}
}
