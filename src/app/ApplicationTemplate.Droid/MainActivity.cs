using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Uno.UI;
using Windows.UI.Xaml;

namespace ApplicationTemplate
{
	[Activity(
		// We need to set Name, otherwise a random one is generated at every build, which causes the app to be removed from the home screen on app-updates.
		// The name must be the fully qualified name of the class, with the namespace part in lowercase.
		// https://developer.xamarin.com/releases/android/xamarin.android_5/xamarin.android_5.1/#Android_Callable_Wrapper_Naming
		Name = "applicationtemplate.MainActivity",
		MainLauncher = true,
		ConfigurationChanges = ActivityHelper.AllConfigChanges,
		WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden,
		ScreenOrientation = ScreenOrientation.Portrait,
		ResizeableActivity = false,
		LaunchMode = LaunchMode.SingleTask
	)]
	public class MainActivity : ApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

#if (IncludeFirebaseAnalytics)
			/// Do not call Crashlytics.Crashlytics.HandleManagedExceptions();
			/// We are already handling managed exceptions in <see cref="HandleUnhandledExceptions"/>
			/// WARNING: This method will fail if you didn't provide a GoogleServicesJson file.
#endif
		}
	}
}
