#if __ANDROID__
using System;
using System.Globalization;
using Android.Content.PM;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The Android implementation of <see cref="IVersionProvider"/>.
/// </summary>
public sealed class VersionProvider : IVersionProvider
{
	private readonly Lazy<PackageInfo> _packageInfo;

	public VersionProvider()
	{
		_packageInfo = new Lazy<PackageInfo>(() =>
		{
			var context = Android.App.Application.Context;
			return context.PackageManager.GetPackageInfo(context.PackageName, 0);
		});
	}

	/// <inheritdoc/>
	public string BuildString => _packageInfo.Value.LongVersionCode.ToString(CultureInfo.InvariantCulture);

	/// <inheritdoc/>
	public Version Version
	{
		get
		{
			var versionName = _packageInfo.Value.VersionName;
			return Version.TryParse(versionName, out var version) ? version : new Version(1, 0, 0);
		}
	}

	/// <inheritdoc/>
	public string VersionString => Version.ToString(3);
}
#endif
