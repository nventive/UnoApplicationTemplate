#if __ANDROID__
using System;
using Android.Widget;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Uno.Extensions;
using Uno.Logging;
using Uno.UI;

namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// The Android implementation of the <see cref="IToastService"/>.
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

		var toastLength = ToastDurationToToastLength(duration);

		_dispatcherQueue.TryEnqueue(() =>
		{
			Toast.MakeText(ContextHelper.Current, message, toastLength).Show();
		});

		if (this.Log().IsEnabled(LogLevel.Information))
		{
			this.Log().Info($"Showed a notification (message: '{message}', duration: '{duration}').");
		}
	}

	private static ToastLength ToastDurationToToastLength(ToastDuration toastDuration)
	{
		return toastDuration switch
		{
			ToastDuration.Long => ToastLength.Long,
			_ => ToastLength.Short,
		};
	}
}
#endif
