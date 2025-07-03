// src/app/ApplicationTemplate.Access/PlatformServices/Version/VersionProvider.Windows.cs
using System;
using Windows.ApplicationModel;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	public class VersionProvider : IVersionProvider
	{
		public string BuildString => Package.Current.Id.Version.Revision.ToString();
		public Version Version => Package.Current.Id.Version;
		public string VersionString => $"{Version.Major}.{Version.Minor}.{Version.Build}";
	}
}
