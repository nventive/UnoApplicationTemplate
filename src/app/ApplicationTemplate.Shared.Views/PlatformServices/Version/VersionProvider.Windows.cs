//-:cnd:noEmit
#if __WINDOWS__
using System;
using System.Globalization;
using Windows.ApplicationModel;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed partial class VersionProvider : IVersionProvider
{
	public string BuildString => Version.Revision.ToString(CultureInfo.InvariantCulture);

	public Version Version
	{
		get
		{
			var packageVersion = Package.Current.Id.Version;
			return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build);
		}
	}
}
#endif
//+:cnd:noEmit
