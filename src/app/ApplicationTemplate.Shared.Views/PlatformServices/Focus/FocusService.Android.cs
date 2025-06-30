// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Focus/FocusService.Android.cs
#if __ANDROID__
using Android.Views.InputMethods;
using AndroidX.Core.Content;
using ApplicationTemplate.DataAccess.PlatformServices;

namespace ApplicationTemplate.Views.PlatformServices;

public sealed class FocusService : IFocusService
{
	public void ClearFocus()
	{
		var activity = Platform.CurrentActivity ?? Microsoft.UI.Xaml.Window.Current?.Content?.XamlRoot?.ContentIslandEnvironment?.AppWindowId;
		if (activity is Android.App.Activity androidActivity)
		{
			var inputMethodManager = ContextCompat.getSystemService(androidActivity, Java.Lang.Class.FromType(typeof(InputMethodManager))) as InputMethodManager;
			var currentFocus = androidActivity.CurrentFocus;
			if (currentFocus != null)
			{
				inputMethodManager?.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
				currentFocus.ClearFocus();
			}
		}
	}
}
#endif
