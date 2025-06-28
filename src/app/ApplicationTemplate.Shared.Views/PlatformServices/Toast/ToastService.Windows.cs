#if __WINDOWS__
using System;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI.Notifications;

namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// The Windows implementation of the <see cref="IToastService"/>.
/// </summary>
public sealed class ToastService : IToastService
{
	private readonly DispatcherQueue _dispatcherQueue;

	/// <summary>
	/// Initializes a new instance of the <see cref="ToastService"/> class.
	/// </summary>
	/// <param name="dispatcherQueue">Dispatcher queue.</param>
	public ToastService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
	}

	/// <inheritdoc/>
	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		if (this.Log().IsEnabled(LogLevel.Debug))
		{
			this.Log().Debug($"Showing a notification (message: '{message}', duration: '{duration}').");
		}

		_dispatcherQueue.TryEnqueue(() =>
		{
			var toastNotifier = ToastNotificationManager.CreateToastNotifier();

			if (toastNotifier.Setting == NotificationSetting.Enabled)
			{
				var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

				var toastText = toastXml.GetElementsByTagName("text");
				var toastExpiration = DateTimeOffset.Now.AddMilliseconds((int)duration);

				toastText[0].AppendChild(toastXml.CreateTextNode(message));

				var toast = new ToastNotification(toastXml)
				{
					ExpirationTime = toastExpiration,
				};

				toastNotifier.Show(toast);
			}
		});

		if (this.Log().IsEnabled(LogLevel.Information))
		{
			this.Log().Info($"Showed a notification (message: '{message}', duration: '{duration}').");
		}
	}
}
#endif
