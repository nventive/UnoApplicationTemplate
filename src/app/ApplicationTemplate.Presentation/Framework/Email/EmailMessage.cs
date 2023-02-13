using System.Collections.Generic;
using System.Linq;

namespace ApplicationTemplate;

public sealed class EmailMessage
{
	public EmailMessage()
	{
	}

	public EmailMessage(string subject, string body, params string[] to)
	{
		Subject = subject;
		Body = body;
		To = to?.ToList() ?? new List<string>();
	}

	public string Subject { get; set; }

	public string Body { get; set; }

	public EmailBodyFormat BodyFormat { get; set; }

	public List<string> To { get; set; } = new List<string>();

	public List<string> Cc { get; set; } = new List<string>();

	public List<string> Bcc { get; set; } = new List<string>();

	public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
}
