using System;

namespace ApplicationTemplate;

public interface IVersionProvider
{
	/// <summary>
	/// Gets the application version build string.
	/// </summary>
	/// <returns>The application version build number string, or "-1" if it's undefined.</returns>
	public string BuildString { get; }

	/// <summary>
	/// Gets the application version.
	/// </summary>
	/// <returns>The application version.</returns>
	public Version Version { get; }

	/// <summary>
	/// Gets the application version string (Major.Minor.Build).
	/// </summary>
	/// <returns>The application version string.</returns>
	public string VersionString { get; }
}
