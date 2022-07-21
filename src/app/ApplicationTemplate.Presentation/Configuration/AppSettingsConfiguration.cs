using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Extensions;

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

	/// <summary>
	/// This class is used for app settings configuration.
	/// - Loads the environment specific app settings.
	/// - Configures the host.
	/// </summary>
	public static class AppSettingsConfiguration
	{
		public const string AppSettingsFileName = "appsettings";

//-:cnd:noEmit
#if PRODUCTION
//+:cnd:noEmit
		public const string DefaultEnvironment = "PRODUCTION";
//-:cnd:noEmit
#elif DEBUG
//+:cnd:noEmit
		public const string DefaultEnvironment = "DEVELOPMENT";
//-:cnd:noEmit
#else
//+:cnd:noEmit
		public const string DefaultEnvironment = "STAGING";
//-:cnd:noEmit
#endif
//+:cnd:noEmit

		public static IHostBuilder AddAppSettings(this IHostBuilder hostBuilder, string folderPath)
		{
			if (hostBuilder is null)
			{
				throw new ArgumentNullException(nameof(hostBuilder));
			}

			return hostBuilder
				.AddConfiguration()
				.ConfigureHostConfiguration(b => b
					.AddGeneralAppSettings()
					.AddEnvironmentAppSettings(folderPath)
					.Add(new WritableJsonConfigurationSource(Path.Combine(folderPath, AppSettingsFileName + ".override.json")))
				);
		}

		private static IConfigurationBuilder AddGeneralAppSettings(this IConfigurationBuilder configurationBuilder)
		{
			var generalAppSettingsFileName = $"{AppSettingsFileName}.json";
			var generalAppSettings = AppSettings.GetAll().SingleOrDefault(s => s.FileName.EndsWith(generalAppSettingsFileName, StringComparison.OrdinalIgnoreCase));

			if (generalAppSettings != null)
			{
				configurationBuilder.AddJsonStream(generalAppSettings.GetContent());
			}

			return configurationBuilder;
		}

		private static IConfigurationBuilder AddEnvironmentAppSettings(this IConfigurationBuilder configurationBuilder, string folderPath)
		{
			var currentEnvironment = AppEnvironment.GetCurrent(folderPath);
			var environmentAppSettingsFileName = $"{AppSettingsFileName}.{currentEnvironment}.json";
			var environmentAppSettings = AppSettings.GetAll().SingleOrDefault(s => s.FileName.EndsWith(environmentAppSettingsFileName, StringComparison.OrdinalIgnoreCase));

			if (environmentAppSettings != null)
			{
				configurationBuilder.AddJsonStream(environmentAppSettings.GetContent());
			}

			return configurationBuilder;
		}

		public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
		{
			if (hostBuilder is null)
			{
				throw new ArgumentNullException(nameof(hostBuilder));
			}

			return hostBuilder.ConfigureServices((ctx, s) =>
				s.AddSingleton(a => ctx.Configuration)
			);
		}

		public class AppEnvironment
		{
			private static string _currentEnvironment;

			/// <summary>
			/// Gets the current environment.
			/// </summary>
			/// <returns>Current environment</returns>
			public static string GetCurrent(string folderPath)
			{
				if (_currentEnvironment == null)
				{
					var filePath = GetSettingFilePath(folderPath);

					_currentEnvironment = File.Exists(filePath)
						? File.ReadAllText(filePath)
						: DefaultEnvironment;
				}

				return _currentEnvironment.ToUpperInvariant();
			}

			/// <summary>
			/// Sets the current environment to <paramref name="environment"/>.
			/// </summary>
			/// <param name="environment">Environment</param>
			public static void SetCurrent(string folderPath, string environment)
			{
				if (environment == null)
				{
					throw new ArgumentNullException(nameof(environment));
				}

				environment = environment.ToUpperInvariant();

				var availableEnvironments = GetAll();

				if (!availableEnvironments.Contains(environment))
				{
					throw new InvalidOperationException($"Environment '{environment}' is unknown and cannot be set.");
				}

				using (var writer = File.CreateText(GetSettingFilePath(folderPath)))
				{
					writer.Write(environment);
				}

				_currentEnvironment = environment;
			}

			public static string[] GetAll()
			{
				var environmentsFromAppSettings = AppSettings
					.GetAll()
					.Select(s => s.Environment)
					.Where(s => !string.IsNullOrEmpty(s))
					.Select(s => s.ToUpperInvariant())
					.OrderBy(name => name)
					.ToArray();

				if (environmentsFromAppSettings.Length == 0)
				{
					// Return the default environment if there are no environment specific appsettings.
					return new string[] { DefaultEnvironment };
				}

				return environmentsFromAppSettings;
			}

			private static string GetSettingFilePath(string folderPath)
			{
				return Path.Combine(folderPath, "environment");
			}
		}

		public class AppSettings
		{
			private static AppSettings[] _appSettings;

			private readonly Assembly _assembly;
			private readonly Lazy<string> _environment;

			public AppSettings(string fileName, Assembly assembly)
			{
				FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
				_assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
				_environment = new Lazy<string>(GetEnvironment);
			}

			public string FileName { get; }

			public string Environment => _environment.Value;

			private string GetEnvironment()
			{
				var environmentMatch = Regex.Match(FileName, "appsettings.(\\w+).json");

				return environmentMatch.Groups.Count > 1
					? environmentMatch.Groups[1].Value
					: null;
			}

			public Stream GetContent()
			{
				using (var resourceFileStream = _assembly.GetManifestResourceStream(FileName))
				{
					if (resourceFileStream != null)
					{
						var memoryStream = new MemoryStream();

						resourceFileStream.CopyTo(memoryStream);
						memoryStream.Seek(0, SeekOrigin.Begin);

						return memoryStream;
					}

					return null;
				}
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not available for desktop.")]
			public static AppSettings[] GetAll()
			{
				if (_appSettings == null)
				{
					var executingAssembly = Assembly.GetExecutingAssembly();

					_appSettings = executingAssembly
						.GetManifestResourceNames()
						.Where(fileName => fileName.ToUpperInvariant().Contains(AppSettingsFileName.ToUpperInvariant()))
						.Select(fileName => new AppSettings(fileName, executingAssembly))
						.ToArray();
				}

				return _appSettings;
			}
		}
	}
}
