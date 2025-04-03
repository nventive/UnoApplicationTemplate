//-:cnd:noEmit
#if __MOBILE__
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
		await _dispatcherScheduler.Run(async () =>
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
		},
		ct);
	}
}
#endif
//+:cnd:noEmit
