#if __IOS__
using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UIKit;
using Uno;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;

namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// The iOS implementation of the <see cref="IToastService"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Uno doesn't want us to use Dispose on a Panel.")]
public sealed class ToastService : IToastService, IDisposable
{
	private static readonly TimeSpan _fadeInDuration = TimeSpan.FromMilliseconds(100);
	private static readonly TimeSpan _fadeOutDuration = TimeSpan.FromMilliseconds(300);

	private readonly ConcurrentQueue<ToastNotification> _toastNotificationsQueue = new();
	private readonly CompositeDisposable _subscriptions = [];

	private readonly IDispatcherScheduler _dispatcherScheduler;
	private readonly DataTemplate _toastDataTemplate;
	private readonly Panel _toastContainer;

	/// <summary>
	/// Initializes a new instance of the <see cref="ToastService"/> class.
	/// Default entrance and exit animations are fade-in / fade-out.
	/// </summary>
	/// <param name="dispatcherScheduler">Dispatcher scheduler.</param>
	/// <param name="toastContainer">Toast container in your view.</param>
	/// <param name="toastDataTemplate">Toast data template.</param>
	public ToastService(
		IDispatcherScheduler dispatcherScheduler,
		Panel toastContainer,
		DataTemplate toastDataTemplate
	)
	{
		_dispatcherScheduler = dispatcherScheduler.Validation().NotNull("dispatcherScheduler");
		_toastContainer = toastContainer.Validation().NotNull("toastContainer");
		_toastDataTemplate = toastDataTemplate.Validation().NotNull("toastDataTemplate");
	}

	/// <inheritdoc/>
	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		if (this.Log().IsEnabled(LogLevel.Debug))
		{
			this.Log().Debug($"Showing a notification (message: '{message}', duration: '{duration}').");
		}

		var toastNotification = new ToastNotification(message, duration);

		_toastNotificationsQueue.Enqueue(toastNotification);

		// If it's the only item in queue, show it immediately.
		if (_toastNotificationsQueue.Count == 1)
		{
			TryShowFollowingToast();
		}

		if (this.Log().IsEnabled(LogLevel.Information))
		{
			this.Log().Info($"Showed a notification (message: '{message}', duration: '{duration}').");
		}
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_subscriptions.Dispose();
	}

	/// <summary>
	/// Adds the toast to the visual tree and displays it.
	/// Toasts are queued while there is a toast currently displayed.
	/// Once the currently displayed toast goes hidden, we dequeue it and show the following toast (if any).
	/// </summary>
	/// <param name="ct">Cancellation Token.</param>
	/// <param name="toastNotification">Toast Notification.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	private async Task ShowToast(CancellationToken ct, ToastNotification toastNotification)
	{
		var toastView = GetToastView();

		(toastView as IDataContextProvider).Maybe(t => t.DataContext = toastNotification);
		_toastContainer.Children.Add(toastView);

		await FadeInToast(ct, toastView);

		// We need to wait the toast duration before hiding it.
		var timeBeforeHidingToast = TimeSpan.FromMilliseconds((int)toastNotification.Duration);

		_dispatcherScheduler.ScheduleTask(timeBeforeHidingToast, async (ct2, s) =>
		{
			// Remove the notification from the queue now that it is complete.
			_toastNotificationsQueue.TryDequeue(out var notification);

			// Start hiding the toast now.
			var hideToast = HideToast(ct2, toastView);

			// As soon as the toast is hiding itself, we display the following toast.
			TryShowFollowingToast();

			await hideToast;
		}).DisposeWith(_subscriptions);
	}

	/// <summary>
	/// Hides and removes the toast from the visual tree.
	/// </summary>
	/// <param name="ct">Cancellation Token</param>
	/// <param name="toastView">Toast View</param>
	private async Task HideToast(CancellationToken ct, UIView toastView)
	{
		await FadeOutToast(ct, toastView);

		_toastContainer.Children.Remove((UIElement)toastView);
	}

	/// <summary>
	/// Tries to show the following toast (if any).
	/// </summary>
	private void TryShowFollowingToast()
	{
		_toastNotificationsQueue.TryPeek(out var notification);

		if (notification != null)
		{
			_dispatcherScheduler.ScheduleTask((ct, s) => ShowToast(ct, notification)).DisposeWith(_subscriptions);
		}
	}

	private UIView GetToastView()
	{
		return _toastDataTemplate.LoadContent();
	}

	private async Task FadeInToast(CancellationToken ct, UIView toastView)
	{
		var taskCompletionSource = new TaskCompletionSource<Unit>();

		using (ct.Register(() => taskCompletionSource.TrySetCanceled()))
		{
			toastView.Alpha = 0;

			UIView.Animate(
				duration: _fadeInDuration.TotalSeconds,
				delay: 0,
				options: UIViewAnimationOptions.CurveEaseIn,
				animation: () => toastView.Alpha = 1,
				completion: () => taskCompletionSource.TrySetResult(Unit.Default)
			);

			await taskCompletionSource.Task;
		}
	}

	private async Task FadeOutToast(CancellationToken ct, UIView toastView)
	{
		var taskCompletionSource = new TaskCompletionSource<Unit>();

		using (ct.Register(() => taskCompletionSource.TrySetCanceled()))
		{
			UIView.Animate(
				duration: _fadeOutDuration.TotalSeconds,
				delay: 0,
				options: UIViewAnimationOptions.CurveEaseIn,
				animation: () => toastView.Alpha = 0,
				completion: () => taskCompletionSource.TrySetResult(Unit.Default)
			);

			await taskCompletionSource.Task;
		}
	}
}
#endif
