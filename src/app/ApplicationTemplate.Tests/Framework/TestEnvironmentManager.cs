using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Tests;

/// <summary>
/// This implementation of <see cref="IEnvironmentManager"/> doesn't support the override features.
/// </summary>
public class TestEnvironmentManager : IEnvironmentManager
{
	public TestEnvironmentManager(string currentEnvironment = "DEVELOPMENT", string defaultEnvironment = "DEVELOPMENT")
	{
		Current = currentEnvironment;
		Default = defaultEnvironment;
	}

	public string Current { get; }

	public string Default { get; }

	public string[] AvailableEnvironments { get; } = new string[] { "DEVELOPMENT", "STAGING", "PRODUCTION" };

	public void ClearOverride()
	{
		throw new NotSupportedException("Overriding the environment is not supported in tests.");
	}

	public void Override(string environment)
	{
		throw new NotSupportedException("Overriding the environment is not supported in tests.");
	}
}
