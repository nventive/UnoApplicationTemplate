//-:cnd:noEmit
#if __WINDOWS__
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed partial class EmailService : IEmailService
{
	private readonly IDispatcherScheduler _dispatcherScheduler;

	public EmailService(IDispatcherScheduler dispatcherScheduler)
	{
		_dispatcherScheduler = dispatcherScheduler;
	}

	public async Task Compose(CancellationToken ct, Email email)
	{
		throw new NotImplementedException();

//-:cnd:noEmit
		/* Restore this code when Uno implementation is working and supporting attachments.
		 * See https://github.com/unoplatform/uno/issues/2432 for more details. */
#if false
		await _dispatcherScheduler.Run(async () =>
		{
			var emailMessage = new Windows.ApplicationModel.Email.EmailMessage()
			{
				Subject = email.Subject,
				Body = email.Body,
				To = email.To.ToList(),
			};

			foreach (var attachment in email.Attachments)
			{
				var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(attachment.FullPath);
				emailMessage.Attachments.Add(
					new Windows.ApplicationModel.Email.EmailAttachment(attachment.FileName, Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(file)
				));
			}

			await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
			},
		ct);
#endif
//+:cnd:noEmit
	}
}
#endif
//+:cnd:noEmit
