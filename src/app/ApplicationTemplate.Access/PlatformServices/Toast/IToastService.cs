namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// A service that shows toasts notifications.
/// </summary>
public interface IToastService
{
	/// <summary>
	/// Displays a toast notification with the specified message and duration.
	/// </summary>
	/// <param name="message">The message to display in the notification.</param>
	/// <param name="duration">The duration for which the notification is displayed. Defaults to <see cref="ToastDuration.Short"/>.</param>
	void ShowNotification(string message, ToastDuration duration = ToastDuration.Short);
}
