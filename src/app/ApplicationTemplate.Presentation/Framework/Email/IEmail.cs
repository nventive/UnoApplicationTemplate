#nullable enable

using System.Threading.Tasks;

namespace ApplicationTemplate;

/// <summary>
/// Provides an easy way to allow the user to send emails.
/// </summary>
public interface IEmail
{
	/// <summary>
	/// Gets a value indicating whether composing an email is supported on this device.
	/// </summary>
	bool IsComposeSupported { get; }

	/// <summary>
	/// Opens the default email client to allow the user to send the message.
	/// </summary>
	/// <param name="message">Instance of <see cref="EmailMessage"/> containing details of the email message to compose.</param>
	/// <returns>A <see cref="Task"/> object with the current status of the asynchronous operation.</returns>
	Task ComposeAsync(EmailMessage? message);
}
