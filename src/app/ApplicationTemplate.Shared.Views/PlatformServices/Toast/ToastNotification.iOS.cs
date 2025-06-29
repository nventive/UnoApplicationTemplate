#if __IOS__
namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// This class represent Toast Notification for iOS.
/// </summary>
[Microsoft.UI.Xaml.Data.Bindable]
[Foundation.Preserve(AllMembers = true)]
public sealed class ToastNotification
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ToastNotification"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="duration">The duration.</param>
	public ToastNotification(string message, ToastDuration duration)
	{
		Message = message;
		Duration = duration;
	}

	/// <summary>
	/// Gets the message.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// Gets the duration.
	/// </summary>
	public ToastDuration Duration { get; }
}
#endif
