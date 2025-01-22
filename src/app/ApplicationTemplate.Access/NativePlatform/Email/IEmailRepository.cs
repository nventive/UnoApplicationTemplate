using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to native methods to send emails.
/// </summary>
public interface IEmailRepository
{
	/// <summary>
	/// Launches the email application with a new <see cref="Email"/> displayed.
	/// </summary>
	/// <param name="email"><see cref="Email"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	Task Compose(Email email);
}
