using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate;

/// <summary>
/// This is the abstraction to use as the source of truth for application environment (e.g. Staging, Production, etc.).
/// </summary>
public interface IEnvironmentManager
{
	/// <summary>
	/// Gets the current environment (which can be overridden).
	/// </summary>
	string Current { get; }

	/// <summary>
	/// Gets the default environment (based on the build variables).
	/// </summary>
	string Default { get; }

	/// <summary>
	/// Overrides the current environment with one provided by <see cref="AvailableEnvironments"/>.
	/// </summary>
	/// <param name="environment">The environment to use as override.</param>
	void Override(string environment);

	/// <summary>
	/// Clears the current environment override and returns to the default value.
	/// </summary>
	void ClearOverride();

	/// <summary>
	/// Gets all available environments.
	/// </summary>
	/// <returns>A collection of strings representing each environment.</returns>
	string[] AvailableEnvironments { get; }
}
