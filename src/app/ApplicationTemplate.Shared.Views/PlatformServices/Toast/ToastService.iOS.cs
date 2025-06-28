// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.iOS.cs
#if __IOS__
using ApplicationTemplate.DataAccess.PlatformServices;

namespace ApplicationTemplate.DataAccess.PlatformServices;


public partial class ToastService : IToastService
{
	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		// Fake implementation matching FakeToastService, does nothing
	}
}
#endif
