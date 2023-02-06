namespace ApplicationTemplate;

/// <summary>
/// Extensions for <see cref="IVersionProvider"/>.
/// </summary>
public static class VersionProviderExtensions
{
	/// <summary>
	/// Gets the full version string (Major.Minor.Build (Revision)).
	/// </summary>
	/// <param name="versionProvider"><see cref=""/>.</param>
	/// <returns>The full version string (Major.Minor.Build (Revision)).</returns>
	public static string GetFullVersionString(this IVersionProvider versionProvider)
	{
		var version = versionProvider.Version;
		return $"{version.Major}.{version.Minor}.{version.Build} ({version.Revision})";
	}
}
