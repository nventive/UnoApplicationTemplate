//-:cnd:noEmit
#if __MOBILE__
using System.Linq;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate;

public sealed partial class EmailRepository : IEmailRepository
{
	public async Task Compose(Email email)
	{
		var emailMessage = new Microsoft.Maui.ApplicationModel.Communication.EmailMessage()
		{
			Subject = email.Subject,
			Body = email.Body,
			To = email.To.ToList(),
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
