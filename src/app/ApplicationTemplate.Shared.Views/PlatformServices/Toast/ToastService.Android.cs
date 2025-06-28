// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.Android.cs
#if __ANDROID__
using Android.Widget;
using AndroidX.Core.App;
using Microsoft.UI.Dispatching;

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
			var toastLength = duration == ToastDuration.Short ? ToastLength.Short : ToastLength.Long;
			Toast.MakeText(Platform.CurrentActivity ?? Android.App.Application.Context, message, toastLength)?.Show();
		});
	}
}
#endif
