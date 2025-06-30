// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Focus/FocusService.iOS.cs
#if __IOS__
using ApplicationTemplate.DataAccess.PlatformServices;
using UIKit;

namespace ApplicationTemplate.Views.PlatformServices;

public sealed class FocusService : IFocusService
{
	public void ClearFocus()
	{
		UIApplication.SharedApplication.KeyWindow?.EndEditing(true);
	}
}
#endif
