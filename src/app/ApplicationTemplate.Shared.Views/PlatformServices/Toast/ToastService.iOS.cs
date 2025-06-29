// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.iOS.cs
#if __IOS__
using Microsoft.UI.Dispatching;
using CPS.DataAccess.PlatformServices;
using UIKit;
using Foundation;
using System;

namespace CPS.DataAccess.PlatformServices
{
	public class ToastService : IToastService
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
				var alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
				var rootViewController = UIApplication.SharedApplication.KeyWindow?.RootViewController;
				if (rootViewController != null)
				{
					rootViewController.PresentViewController(alert, true, () =>
					{
						NSTimer.CreateScheduledTimer(TimeSpan.FromMilliseconds((int)duration), (timer) =>
						{
							alert.DismissViewController(true, null);
							timer.Invalidate();
						});
					});
				}
			});
		}
	}
}
#endif
