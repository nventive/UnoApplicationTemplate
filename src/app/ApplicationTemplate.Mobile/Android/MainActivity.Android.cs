using Android.App;
using Android.Content.PM;
using Android.Views;

namespace ApplicationTemplate;

[Activity(
	MainLauncher = true,
	ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
	WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden,
	ScreenOrientation = ScreenOrientation.Portrait,
	ResizeableActivity = false,
	LaunchMode = LaunchMode.SingleTask
)]
public sealed class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
}
