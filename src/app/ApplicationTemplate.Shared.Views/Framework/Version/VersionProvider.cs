using System;

namespace ApplicationTemplate;

public sealed partial class VersionProvider : IVersionProvider
{
	public string VersionString => Version.ToString();
}
