using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed partial class VersionProvider : IVersionProvider
{
	public string VersionString => Version.ToString();
}
