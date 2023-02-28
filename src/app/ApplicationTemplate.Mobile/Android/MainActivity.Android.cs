using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace ApplicationTemplate;

[Activity(
	MainLauncher = true,
	ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
	WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateHidden,
	ScreenOrientation = ScreenOrientation.Portrait,
	ResizeableActivity = false,
	LaunchMode = LaunchMode.SingleTask
)]
public sealed class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
	protected override void OnCreate(Bundle bundle)
	{
		// Support the ExtendedSplashScreen on Android 12+.
		Nventive.ExtendedSplashScreen.ExtendedSplashScreen.AndroidSplashScreen = AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

		base.OnCreate(bundle);
	}
}
