using NUnit.Framework;
using Uno.UITest;
using Uno.UITest.Helpers.Queries;
using Uno.UITests.Helpers;

namespace ApplicationTemplate.UITests
{
	public class TestBase
	{
		private static IApp _app;

		static TestBase()
		{
			AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
			AppInitializer.TestEnvironment.WebAssemblyDefaultUri = Constants.WebAssemblyDefaultUri;
			AppInitializer.TestEnvironment.iOSAppName = Constants.IOSAppName;
			AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
			AppInitializer.TestEnvironment.iOSDeviceNameOrId = Constants.IOSDeviceNameOrId;
			AppInitializer.TestEnvironment.CurrentPlatform = Constants.PlatformUnderTest;

#if DEBUG
			AppInitializer.TestEnvironment.WebAssemblyHeadless = false;
#endif

			// Start the app only once, so the tests runs don't restart it
			// and gain some time for the tests.
			AppInitializer.ColdStartApp();
		}

		public static IApp App
		{
			get => _app;
			set => Helpers.App = _app = value;
		}

		[SetUp]
		public void StartApp()
		{
			App = AppInitializer.AttachToApp();
		}

		[TearDown]
		public void StopApp()
		{
		}
	}
}
