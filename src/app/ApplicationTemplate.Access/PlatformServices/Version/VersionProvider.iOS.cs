// src/app/ApplicationTemplate.Access/PlatformServices/Version/VersionProvider.iOS.cs
using Foundation;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	public class VersionProvider : IVersionProvider
	{
		public string BuildString => NSBundle.MainBundle.InfoDictionary["CFBundleVersion"]?.ToString() ?? "-1";
		public Version Version => new Version(
			int.Parse((string)NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] ?? "0.0.0"),
			int.Parse((string)NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] ?? "0.0.0"),
			int.Parse((string)NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] ?? "0.0.0"),
			int.Parse((string)NSBundle.MainBundle.InfoDictionary["CFBundleVersion"] ?? "0")
		);
		public string VersionString => (string)NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] ?? "0.0.0";
	}
}
