using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationTemplate
{
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
