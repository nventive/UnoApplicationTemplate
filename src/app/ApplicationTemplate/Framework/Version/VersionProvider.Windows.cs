//-:cnd:noEmit
#if 	WINDOWS10_0_18362_0_OR_GREATER
using System;
using System.Globalization;
using Windows.ApplicationModel;

namespace ApplicationTemplate;

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
