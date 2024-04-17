using System.Collections.Generic;
using System.Linq;

namespace ApplicationTemplate;

public sealed class Email
{
	public Email()
	{
	}

	public Email(string subject, string body, params string[] to)
	{
		Subject = subject;
		Body = body;
		To = to?.ToList() ?? new List<string>();
	}

	public string Subject { get; set; }

	public string Body { get; set; }

	public IList<string> To { get; set; } = new List<string>();

	public IList<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
}
