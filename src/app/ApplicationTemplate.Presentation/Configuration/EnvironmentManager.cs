using System;
using System.IO;
using System.Linq;
using static ApplicationTemplate.ConfigurationConfiguration;

namespace ApplicationTemplate;

/// <summary>
/// This implementation of <see cref="IEnvironmentManager"/> uses local files to support the override features.
/// </summary>
public sealed class EnvironmentManager : IEnvironmentManager
{
//-:cnd:noEmit
#if PRODUCTION
	public const string DefaultEnvironment = "PRODUCTION";
#elif DEBUG
	public const string DefaultEnvironment = "DEVELOPMENT";
#else
	public const string DefaultEnvironment = "STAGING";
#endif
//+:cnd:noEmit

	private readonly string _folderPath;
	private readonly string _overrideSettingFilePath;

	/// <summary>
	/// Initializes a new instance of the <see cref="EnvironmentManager"/> class.
	/// </summary>
	/// <param name="folderPath">The folder in which to put the environment override file.</param>
	public EnvironmentManager(string folderPath)
	{
		_folderPath = folderPath;
		_overrideSettingFilePath = GetSettingFilePath(_folderPath);

		AvailableEnvironments = GetAll();
		Current = File.Exists(_overrideSettingFilePath)
			? File.ReadAllText(_overrideSettingFilePath)
			: Default;
		Next = Current;
	}

	public string Current { get; }

	public string Next { get; private set; }

	public string Default => DefaultEnvironment;

	public string[] AvailableEnvironments { get; }

	public void ClearOverride()
	{
		File.Delete(_overrideSettingFilePath);

		Next = Default;
	}

	public void Override(string environment)
	{
		ArgumentNullException.ThrowIfNull(environment);

		environment = environment.ToUpperInvariant();

		if (!AvailableEnvironments.Contains(environment))
		{
			throw new InvalidOperationException($"Environment '{environment}' is unknown and cannot be set.");
		}

		using (var writer = File.CreateText(GetSettingFilePath(_folderPath)))
		{
			writer.Write(environment);
		}

		Next = environment;
	}

	public static string[] GetAll()
	{
		var environmentsFromAppSettings = AppSettingsFile
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
