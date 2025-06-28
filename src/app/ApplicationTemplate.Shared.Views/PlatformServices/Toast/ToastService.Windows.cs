// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.Windows.cs
#if WINDOWS
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public partial class ToastService : IToastService
{
	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		var content = new ToastContentBuilder()
			.AddText(message)
			.GetToastContent();


		var toast = new ToastNotification(content.GetXml());
		toast.ExpirationTime = DateTime.Now.AddMilliseconds((int)duration);
		ToastNotificationManager.CreateToastNotifier().Show(toast);
	}
}
#endif
