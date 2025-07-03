#if __IOS__
using System;
using Foundation;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The iOS implementation of <see cref="IVersionProvider"/>.
/// </summary>
public sealed class VersionProvider : IVersionProvider
{
	private readonly Lazy<NSDictionary> _infoDictionary;

	public VersionProvider()
	{
		_infoDictionary = new Lazy<NSDictionary>(() => NSBundle.MainBundle.InfoDictionary);
	}

	/// <inheritdoc/>
	public string BuildString
	{
		get
		{
			var buildNumber = _infoDictionary.Value["CFBundleVersion"]?.ToString();
			return buildNumber ?? "-1";
		}
	}

	/// <inheritdoc/>
	public Version Version
	{
		get
		{
			var versionString = _infoDictionary.Value["CFBundleShortVersionString"]?.ToString();
			return Version.TryParse(versionString, out var version) ? version : new Version(1, 0, 0);
		}
	}

	/// <inheritdoc/>
	public string VersionString => Version.ToString(3);
}
#endif
