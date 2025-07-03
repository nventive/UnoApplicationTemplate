#if __WINDOWS__
using System;
using System.Globalization;
using Windows.ApplicationModel;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The Windows implementation of <see cref="IVersionProvider"/>.
/// </summary>
public sealed class VersionProvider : IVersionProvider
{
	private readonly Lazy<PackageVersion> _packageVersion;

	public VersionProvider()
	{
		_packageVersion = new Lazy<PackageVersion>(() => Package.Current.Id.Version);
	}

	/// <inheritdoc/>
	public string BuildString => _packageVersion.Value.Revision.ToString(CultureInfo.InvariantCulture);

	/// <inheritdoc/>
	public Version Version => new(
		_packageVersion.Value.Major,
		_packageVersion.Value.Minor,
		_packageVersion.Value.Build
	);

	/// <inheritdoc/>
	public string VersionString => Version.ToString(3);
}
#endif
