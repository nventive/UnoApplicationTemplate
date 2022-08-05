using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApplicationTemplate
{
	/// <summary>
	/// Represents the source of <see cref="WritableJsonConfigurationProvider"/>.
	/// </summary>
	public class WritableJsonConfigurationSource : IConfigurationSource
	{
		public WritableJsonConfigurationSource(string filePath)
		{
			FilePath = filePath;
		}

		public string FilePath { get; }

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			builder.Properties.TryGetValue("HostLoggerFactory", out var value);
			var factory = value as ILoggerFactory;
			var logger = NullLogger.Instance as ILogger;
			if (factory != null)
			{
				logger = factory.CreateLogger<WritableJsonConfigurationProvider>();
			}

			return new WritableJsonConfigurationProvider(GetSource(FilePath, builder), logger);
		}

		private static JsonConfigurationSource GetSource(string filePath, IConfigurationBuilder builder)
		{
			var source = new JsonConfigurationSource()
			{
				Path = filePath,

				// We disable ReloadOnChange because we reload manually after saving the file.
				ReloadOnChange = false,

				// It's optional because it doesn't exists for as long as we don't override a value.
				Optional = true,
			};

			source.ResolveFileProvider();
			source.EnsureDefaults(builder);

			return source;
		}
	}
}
