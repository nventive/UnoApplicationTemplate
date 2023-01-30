//-:cnd:noEmit
#if __ANDROID__ || __IOS__
using System.Threading.Tasks;

namespace ApplicationTemplate;

public sealed partial class EmailService : IEmailService
{
	public async Task Compose(Email email)
	{
		var emailMessage = new Microsoft.Maui.ApplicationModel.Communication.EmailMessage()
		{
			Subject = email.Subject,
			Body = email.Body,
		};

		foreach (var attachment in email.Attachments)
		{
			emailMessage.Attachments.Add(
				new Microsoft.Maui.ApplicationModel.Communication.EmailAttachment(attachment.FullPath, attachment.ContentType)
			);
		}

		await Microsoft.Maui.ApplicationModel.Communication.Email.ComposeAsync(emailMessage);
	}
}
#endif
//+:cnd:noEmit
