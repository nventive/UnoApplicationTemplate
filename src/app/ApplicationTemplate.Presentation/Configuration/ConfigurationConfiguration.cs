using System;
using System.Collections.Generic;
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
	public const string AppSettingsFileNameWithExtensions = AppSettingsFileName + ".json";
	public const string AppSettingsOverrideFileNameWithExtension = AppSettingsFileName + ".override.json";

	/// <summary>
	/// Populates the <see cref="IConfiguration"/> of the host using appsettings.json files.
	/// </summary>
	/// <param name="hostBuilder">The host builder.</param>
	/// <param name="folderPath">The folder path pointing to override files.</param>
	/// <param name="environmentManager">The environment manager.</param>
	public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder, string folderPath, IEnvironmentManager environmentManager)
	{
		ArgumentNullException.ThrowIfNull(hostBuilder);

		return hostBuilder
			.AddConfiguration(environmentManager)
			.ConfigureHostConfiguration(b => b
				// Readonly configuration (from code, not from any file)
				.AddReadOnlyConfiguration(folderPath, environmentManager)

				// appsettings.json
				.AddBaseConfiguration()

				// appsettings.{staging, production, etc.}.json
				.AddEnvironmentConfiguration(environmentManager)

				// appsettings.override.json
				.AddUserOverrideConfiguration(folderPath)
			);
	}

	/// <summary>
	/// Adds the read-only configuration source containing the values that cannot come from appsettings.json files.
	/// </summary>
	/// <param name="configurationBuilder">The configuration builder.</param>
	/// <param name="folderPath">The folder containing the configuration override files.</param>
	/// <param name="environmentManager">The environment manager.</param>
	private static IConfigurationBuilder AddReadOnlyConfiguration(this IConfigurationBuilder configurationBuilder, string folderPath, IEnvironmentManager environmentManager)
	{
		return configurationBuilder.AddInMemoryCollection(GetCodeConfiguration(folderPath));

		IEnumerable<KeyValuePair<string, string>> GetCodeConfiguration(string folderPath)
		{
			var prefix = ApplicationTemplateConfigurationExtensions.DefaultOptionsName<ReadOnlyConfigurationOptions>() + ":";

			yield return new KeyValuePair<string, string>(prefix + nameof(ReadOnlyConfigurationOptions.ConfigurationOverrideFolderPath), folderPath);
			yield return new KeyValuePair<string, string>(prefix + nameof(ReadOnlyConfigurationOptions.DefaultEnvironment), environmentManager.Default);
		}
	}

	/// <summary>
	/// Adds the base configuration source.
	/// </summary>
	/// <param name="configurationBuilder">The configuration builder.</param>
	private static IConfigurationBuilder AddBaseConfiguration(this IConfigurationBuilder configurationBuilder)
	{
		var generalAppSettingsFileName = AppSettingsFileNameWithExtensions;
		var generalAppSettings = AppSettingsFile.GetAll().SingleOrDefault(s => s.FileName.EndsWith(generalAppSettingsFileName, StringComparison.OrdinalIgnoreCase));

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
	/// <param name="environmentManager">The environment manager.</param>
	private static IConfigurationBuilder AddEnvironmentConfiguration(this IConfigurationBuilder configurationBuilder, IEnvironmentManager environmentManager)
	{
		var currentEnvironment = environmentManager.Current;
		var environmentAppSettingsFileName = $"{AppSettingsFileName}.{currentEnvironment}.json";
		var environmentAppSettings = AppSettingsFile.GetAll().SingleOrDefault(s => s.FileName.EndsWith(environmentAppSettingsFileName, StringComparison.OrdinalIgnoreCase));

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
		return configurationBuilder.Add(new WritableJsonConfigurationSource(Path.Combine(folderPath, AppSettingsOverrideFileNameWithExtension)));
	}

	/// <summary>
	/// Registers the <see cref="IConfiguration"/> as a singleton.
	/// </summary>
	/// <param name="hostBuilder">The host builder.</param>
	/// <param name="environmentManager">The environment manager.</param>
	private static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder, IEnvironmentManager environmentManager)
	{
		ArgumentNullException.ThrowIfNull(hostBuilder);

		return hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) => serviceCollection
			.AddSingleton(serviceProvider => hostBuilderContext.Configuration)
			.AddSingleton(environmentManager)
			.BindOptionsToConfiguration<ReadOnlyConfigurationOptions>(hostBuilderContext.Configuration)
		);
	}

	public sealed class AppSettingsFile
	{
		private static AppSettingsFile[] _appSettingsFiles;

		private readonly Assembly _assembly;
		private readonly Lazy<string> _environment;

		public AppSettingsFile(string fileName, Assembly assembly)
		{
			FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
			_assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			_environment = new Lazy<string>(GetEnvironment);
		}

		public string FileName { get; }

		/// <summary>
		/// Gets the environment from the file name (like "production" in "appsettings.production.json").
		/// </summary>
		/// <returns>The environment string if available or null otherwise.</returns>
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

		public static AppSettingsFile[] GetAll()
		{
			if (_appSettingsFiles == null)
			{
				var executingAssembly = Assembly.GetExecutingAssembly();

				_appSettingsFiles = executingAssembly
					.GetManifestResourceNames()
					.Where(fileName => fileName.ToUpperInvariant().Contains(AppSettingsFileName.ToUpperInvariant(), StringComparison.Ordinal))
					.Select(fileName => new AppSettingsFile(fileName, executingAssembly))
					.ToArray();
			}

			return _appSettingsFiles;
		}
	}
}
