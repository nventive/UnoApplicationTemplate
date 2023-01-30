//-:cnd:noEmit
#if __WINDOWS__
using System;
using System.Threading.Tasks;

namespace ApplicationTemplate;

public sealed partial class EmailService : IEmailService
{
	public Task Compose(Email email)
	{
		throw new NotImplementedException();

//-:cnd:noEmit
/* TODO: Use when Uno implementation is working and supporting attachments.
 * See https://github.com/unoplatform/uno/issues/2432 for more details. */
#if false
		var emailMessage = new Windows.ApplicationModel.Email.EmailMessage()
		{
			Subject = email.Subject,
			Body = email.Body,
		};

		foreach (var attachment in email.Attachments)
		{
			var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(attachment.FullPath);
			emailMessage.Attachments.Add(
				new Windows.ApplicationModel.Email.EmailAttachment(attachment.FileName, Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(file)
			));
		}

		await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
#endif
//+:cnd:noEmit
	}
}
#endif
//+:cnd:noEmit
