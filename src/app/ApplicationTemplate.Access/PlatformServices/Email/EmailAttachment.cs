using System;
using System.IO;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class EmailAttachment
{
	public EmailAttachment(string contentType, string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
		{
			throw new ArgumentException("The file path cannot be null or empty.", nameof(fullPath));
		}

		var fileName = Path.GetFileName(fullPath);
		if (string.IsNullOrWhiteSpace(fileName))
		{
			throw new ArgumentException("The file path must be valide.", nameof(fullPath));
		}

		ContentType = contentType;
		FullPath = fullPath;
		FileName = fileName;
	}

	public string ContentType { get; }

	public string FullPath { get; }

	public string FileName { get; }
}
