using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate;

/// <summary>
/// Represents the configuration options that can't be overriden.
/// All its values can only be set in code (and cannot be read from an appsettings.json file).
/// </summary>
public class ReadOnlyConfigurationOptions
{
	/// <summary>
	/// Gets or sets the folder path containing the override files (such as appsettings.override.json).
	/// </summary>
	public string ConfigurationOverrideFolderPath { get; set; }

	/// <summary>
	/// Gets or sets the default environment for which the app was built for.
	/// </summary>
	public string DefaultEnvironment { get; set; }
}
