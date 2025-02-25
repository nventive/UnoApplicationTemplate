//-:cnd:noEmit
#if __ANDROID__ || __IOS__
using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed partial class VersionProvider : IVersionProvider
{
	public string BuildString => Microsoft.Maui.ApplicationModel.AppInfo.BuildString;

	public Version Version
	{
		get
		{
			return Microsoft.Maui.ApplicationModel.AppInfo.Version;
		}
	}
}
#endif
//+:cnd:noEmit
