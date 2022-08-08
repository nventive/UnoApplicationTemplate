using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationTemplate;

/// <summary>
/// This class is used to populate the <see cref="IConfiguration"/> of the host.
/// - Loads the options from the various appsettings.xxx.json files.
/// - Configures the host.
/// </summary>
public static class ConfigurationConfiguration
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

	/// <summary>
	/// Populates the <see cref="IConfiguration"/> of the host using appsettings.json files.
	/// </summary>
	/// <param name="hostBuilder">The host builder.</param>
	/// <param name="folderPath">The folder path pointing to override files.</param>
	public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder, string folderPath)
	{
		if (hostBuilder is null)
		{
			throw new ArgumentNullException(nameof(hostBuilder));
		}

		return hostBuilder
			.AddConfiguration()
			.ConfigureHostConfiguration(b => b
				// appsettings.json
				.AddBaseConfiguration()

				// appsettings.{staging, production, etc.}.json
				.AddEnvironmentConfiguration(folderPath)

				// appsettings.override.json
				.AddUserOverrideConfiguration(folderPath)
			);
	}

	/// <summary>
	/// Adds the base configuration source.
	/// </summary>
	/// <param name="configurationBuilder">The configuration builder.</param>
	private static IConfigurationBuilder AddBaseConfiguration(this IConfigurationBuilder configurationBuilder)
	{
		var generalAppSettingsFileName = $"{AppSettingsFileName}.json";
		var generalAppSettings = AppSettings.GetAll().SingleOrDefault(s => s.FileName.EndsWith(generalAppSettingsFileName, StringComparison.OrdinalIgnoreCase));

		if (generalAppSettings != null)
		{
			configurationBuilder.AddJsonStream(generalAppSettings.GetContent());
		}

		return configurationBuilder;
	}

	/// <summary>
	/// Adds the environment specific configuration source.
	/// The environment can be overriden by the user.
	/// </summary>
	/// <param name="configurationBuilder">The configuration builder.</param>
	/// <param name="folderPath">The folder path indicating where the override file should be when the user overrode the environment.</param>
	private static IConfigurationBuilder AddEnvironmentConfiguration(this IConfigurationBuilder configurationBuilder, string folderPath)
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

	/// <summary>
	/// Adds the optional <i>user-override</i> configuration source.
	/// </summary>
	/// <param name="configurationBuilder">The configuration builder.</param>
	/// <param name="folderPath">The folder path indicating where the override file should be.</param>
	private static IConfigurationBuilder AddUserOverrideConfiguration(this IConfigurationBuilder configurationBuilder, string folderPath)
	{
		return configurationBuilder.Add(new WritableJsonConfigurationSource(Path.Combine(folderPath, AppSettingsFileName + ".override.json")));
	}

	/// <summary>
	/// Registers the <see cref="IConfiguration"/> as a singleton.
	/// </summary>
	/// <param name="hostBuilder">The host builder.</param>
	private static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
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
		private static string _currentFolderPath;

		/// <summary>
		/// Gets the current environment.
		/// </summary>
		/// <param name="folderPath">The path to the directory containing the environment override file.</param>
		/// <returns>The current environment.</returns>
		public static string GetCurrent(string folderPath)
		{
			if (_currentEnvironment == null)
			{
				_currentFolderPath = folderPath;
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
		/// <param name="environment">The environment override.</param>
		public static void SetCurrent(string environment)
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

			using (var writer = File.CreateText(GetSettingFilePath(_currentFolderPath)))
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
