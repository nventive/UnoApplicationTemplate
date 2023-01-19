#if __WINDOWS__
#nullable enable
using System.Threading.Tasks;

namespace ApplicationTemplate;

/// <summary>
/// Static class with extension methods for the <see cref="IEmail"/> APIs.
/// </summary>
public static class EmailImplementationExtensions
{
	/// <summary>
	/// Opens the default email client to allow the user to send the message.
	/// </summary>
	/// <param name="email">The object this method is invoked on.</param>
	/// <returns>A <see cref="Task"/> object with the current status of the asynchronous operation.</returns>
	public static Task ComposeAsync(this IEmail email) => email.ComposeAsync(null);

	/// <summary>
	/// Opens the default email client to allow the user to send the message with the provided subject, body, and recipients.
	/// </summary>
	/// <param name="email">The object this method is invoked on.</param>
	/// <param name="subject">The email subject.</param>
	/// <param name="body">The email body.</param>
	/// <param name="to">The email recipients.</param>
	/// <returns>A <see cref="Task"/> object with the current status of the asynchronous operation.</returns>
	public static Task ComposeAsync(this IEmail email, string subject, string body, params string[] to)
		=> email.ComposeAsync(new EmailMessage(subject, body, to));
}
#endif
