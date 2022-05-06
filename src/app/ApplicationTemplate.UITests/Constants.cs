using Uno.UITest.Helpers.Queries;

namespace ApplicationTemplate.UITests
{
	public static class Constants
	{
		public const string IOSAppName = "com.nventive.internal.applicationtemplate";
		public const string AndroidAppName = "com.nventive.internal.applicationtemplate";
		public const string IOSDeviceNameOrId = "00008030-000865912E90802E";

		public static Platform PlatformUnderTest { get; } = Platform.Android;
	}
}
