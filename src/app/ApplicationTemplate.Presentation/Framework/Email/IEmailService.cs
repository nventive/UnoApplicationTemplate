using System.Threading.Tasks;

namespace ApplicationTemplate;

public interface IEmailService
{
	/// <summary>
	/// Launches the email application with a new <see cref="Email"/> displayed.
	/// </summary>
	/// <param name="email"><see cref="Email"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	Task Compose(Email email);
}
