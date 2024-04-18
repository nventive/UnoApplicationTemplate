namespace ApplicationTemplate.Tests;

/// <summary>
/// This implementation of <see cref="IEnvironmentManager"/> doesn't support the override features.
/// </summary>
public class TestEnvironmentManager : IEnvironmentManager
{
	public TestEnvironmentManager(string currentEnvironment = "DEVELOPMENT", string defaultEnvironment = "DEVELOPMENT")
	{
		if (currentEnvironment is "PRODUCTION" || defaultEnvironment is "PRODUCTION")
		{
			throw new ArgumentException("The production environment should not be used for tests.");
		}

		Current = currentEnvironment;
		Default = defaultEnvironment;
	}

	public string Current { get; }

	public string Next => Current;

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
