// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.Windows.cs
#if __WINDOWS__
using Microsoft.UI.Dispatching;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CPS.DataAccess.PlatformServices;

public sealed class ToastService : IToastService
{
	private readonly DispatcherQueue _dispatcherQueue;


	public ToastService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue;
	}

	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		_dispatcherQueue.TryEnqueue(() =>
		{
			var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
			var textElements = toastXml.GetElementsByTagName("text");
			textElements[0].AppendChild(toastXml.CreateTextNode(message));


			var toast = new ToastNotification(toastXml);
			ToastNotificationManager.CreateToastNotifier().Show(toast);
		});
	}
}
#endif
