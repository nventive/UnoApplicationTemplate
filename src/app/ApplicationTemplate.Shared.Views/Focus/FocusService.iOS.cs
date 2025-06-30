// src/app/ApplicationTemplate.Shared.Views/Focus/FocusService.iOS.cs
#if __IOS__
using UIKit;
using ApplicationTemplate.DataAccess.PlatformServices;

namespace ApplicationTemplate.Views;

public sealed class FocusService : IFocusService
{
	public void ClearFocus()
	{
		UIApplication.SharedApplication.KeyWindow?.EndEditing(true);
	}
}
#endif
