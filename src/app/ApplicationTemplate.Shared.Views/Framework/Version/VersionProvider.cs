using System;
using System.Globalization;
using Windows.ApplicationModel;

namespace ApplicationTemplate;

public sealed class VersionProvider : IVersionProvider
{
	public int Build => Version.Build;

	public string BuildString => Build.ToString(CultureInfo.InvariantCulture);

	public Version Version
	{
		get
		{
			var packageVersion = Package.Current.Id.Version;
			return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
		}
	}

	public string VersionString => Version.ToString();
}
