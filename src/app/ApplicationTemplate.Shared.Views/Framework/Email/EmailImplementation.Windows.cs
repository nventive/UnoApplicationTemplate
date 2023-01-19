#if __WINDOWS__
#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ApplicationTemplate;

public static class EmailImplementation
{
	public static bool IsComposeSupported => true;

	public static Task ComposeAsync(EmailMessage? message)
	{
		if (message != null && message.BodyFormat != EmailBodyFormat.PlainText)
		{
			throw new ArgumentException("Windows can only compose plain text email messages.", nameof(message));
		}

		var platformEmailMessage = new PlatformEmailMessage();

		if (!string.IsNullOrEmpty(message?.Body))
		{
			platformEmailMessage.Body = message.Body;
		}

		if (!string.IsNullOrEmpty(message?.Subject))
		{
			platformEmailMessage.Subject = message.Subject;
		}

		Sync(message?.To, platformEmailMessage.To);
		Sync(message?.Cc, platformEmailMessage.CC);
		Sync(message?.Bcc, platformEmailMessage.Bcc);

		if (message?.Attachments?.Count > 0)
		{
			foreach (var attachment in message.Attachments)
			{
				var path = NormalizePath(attachment.FullPath);

				platformEmailMessage.Attachments.Add(path);
			}
		}

		return EmailHelper.ShowComposeNewEmailAsync(platformEmailMessage);
	}

	private static string NormalizePath(string path) => path.Replace('/', Path.DirectorySeparatorChar);

	private static void Sync(List<string> recipients, IList<PlatformEmailRecipient> nativeRecipients)
	{
		if (recipients is null)
		{
			return;
		}

		foreach (var recipient in recipients)
		{
			if (string.IsNullOrWhiteSpace(recipient))
			{
				continue;
			}
			nativeRecipients.Add(new PlatformEmailRecipient(recipient));
		}
	}

	private static class EmailHelper
	{
		private enum RecipientClass
		{
			MAPI_ORIG = 0,
			MAPI_TO,
			MAPI_CC,
			MAPI_BCC,
		}

		[Flags]
		private enum SendMailFlags
		{
			MAPI_LOGON_UI = 0x00000001,
			MAPI_DIALOG_MODELESS = 0x00000004,
			MAPI_DIALOG = 0x00000008,
		}

		public static Task<bool> ShowComposeNewEmailAsync(PlatformEmailMessage message) =>
			Task.Run(() => SendMail(message) == 0);

		private static int SendMail(PlatformEmailMessage message)
		{
			var flags = SendMailFlags.MAPI_LOGON_UI | SendMailFlags.MAPI_DIALOG_MODELESS | SendMailFlags.MAPI_DIALOG;

			var recipients = GetRecipients(message);
			var attachments = GetAttachments(message);

			var msg = new MapiMessage
			{
				_subject = message.Subject,
				_noteText = message.Body,
				_recipCount = recipients?.Length ?? 0,
				_recips = GetUnmanagedArray(recipients),
				_fileCount = attachments?.Length ?? 0,
				_files = GetUnmanagedArray(attachments),
			};

			try
			{
				return MAPISendMail(IntPtr.Zero, IntPtr.Zero, ref msg, flags, 0);
			}
			finally
			{
				DestroyUnmanagedArray(msg._recips, recipients);
				DestroyUnmanagedArray(msg._files, attachments);
			}
		}

		private static IntPtr GetUnmanagedArray<TStruct>(TStruct[]? array)
		{
			if (array?.Length > 0)
			{
				var size = Marshal.SizeOf(typeof(TStruct));

				var intptr = Marshal.AllocHGlobal(array.Length * size);

				var ptr = intptr;
				foreach (var item in array)
				{
					Marshal.StructureToPtr(item!, ptr, false);
					ptr += size;
				}

				return intptr;
			}

			return IntPtr.Zero;
		}

		private static void DestroyUnmanagedArray<TStruct>(IntPtr intptr, TStruct[]? array)
		{
			var count = array?.Length;
			if (count > 0)
			{
				var size = Marshal.SizeOf(typeof(TStruct));

				var ptr = intptr;
				for (var i = 0; i < count; i++)
				{
					Marshal.DestroyStructure<TStruct>(ptr);
					ptr += size;
				}

				Marshal.FreeHGlobal(intptr);
			}
		}

		private static MapiRecipDesc[]? GetRecipients(PlatformEmailMessage message)
		{
			var recipCount = message.To.Count + message.CC.Count + message.Bcc.Count;

			if (recipCount == 0)
			{
				return null;
			}

			var recipients = new MapiRecipDesc[recipCount];

			var idx = 0;
			foreach (var to in message.To)
			{
				recipients[idx++] = Create(to, RecipientClass.MAPI_TO);
			}

			foreach (var cc in message.CC)
			{
				recipients[idx++] = Create(cc, RecipientClass.MAPI_CC);
			}

			foreach (var bcc in message.Bcc)
			{
				recipients[idx++] = Create(bcc, RecipientClass.MAPI_BCC);
			}

			return recipients;

			static MapiRecipDesc Create(PlatformEmailRecipient recipient, RecipientClass type)
			{
				return new MapiRecipDesc
				{
					_name = recipient.Address,
					_recipClass = type
				};
			}
		}

		private static MapiFileDesc[]? GetAttachments(PlatformEmailMessage message)
		{
			var attachCount = message.Attachments.Count;

			if (attachCount == 0)
			{
				return null;
			}

			var attachments = new MapiFileDesc[attachCount];

			var idx = 0;
			foreach (var file in message.Attachments)
			{
				attachments[idx++] = Create(file);
			}

			return attachments;

			static MapiFileDesc Create(string filename)
			{
				return new MapiFileDesc
				{
					_name = Path.GetFileName(filename),
					_path = filename,
					_position = -1
				};
			}
		}

		[DllImport("MAPI32.DLL")]
		private static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, ref MapiMessage message, SendMailFlags flg, int rsv);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private struct MapiMessage
		{
			public string? _subject;
			public string? _noteText;
			public string? _messageType;
			public string? _dateReceived;
			public string? _conversationID;
			public int _flags;
			public IntPtr _originator;
			public int _recipCount;
			public IntPtr _recips;
			public int _fileCount;
			public IntPtr _files;
			private int _reserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private struct MapiFileDesc
		{
			public int _flags;
			public int _position;
			public string? _path;
			public string? _name;
			public IntPtr _type;
			private int _reserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private struct MapiRecipDesc
		{
			public RecipientClass _recipClass;
			public string? _name;
			public string? _address;
			public int _eIDSize;
			public IntPtr _entryID;
			private int _reserved;
		}
	}

	private class PlatformEmailMessage
	{
		public string? Body { get; set; }

		public string? Subject { get; set; }

		public List<PlatformEmailRecipient> To { get; } = new();

		public List<PlatformEmailRecipient> CC { get; } = new();

		public List<PlatformEmailRecipient> Bcc { get; } = new();

		public List<string> Attachments { get; } = new();
	}

	private class PlatformEmailRecipient
	{
		public PlatformEmailRecipient(string address)
		{
			Address = address;
		}

		public string Address { get; set; }
	}
}
#endif
