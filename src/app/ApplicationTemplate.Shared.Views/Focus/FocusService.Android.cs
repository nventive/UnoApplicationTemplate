// src/app/ApplicationTemplate.Shared.Views/Focus/FocusService.Android.cs
#if __ANDROID__
using Android.Views.InputMethods;
using Android.App;
using ApplicationTemplate.DataAccess.PlatformServices;

namespace ApplicationTemplate.Views;

public sealed class FocusService : IFocusService
{
	public void ClearFocus()
	{
		var context = Android.App.Application.Context;
		var inputMethodManager = (InputMethodManager)context.GetSystemService(Android.Content.Context.InputMethodService);
		if (Activity.Current?.CurrentFocus != null)
		{
			inputMethodManager.HideSoftInputFromWindow(Activity.Current.CurrentFocus.WindowToken, HideSoftInputFlags.None);
			Activity.Current.CurrentFocus.ClearFocus();
		}
	}
}
#endif
