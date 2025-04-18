﻿using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides access to native methods to send emails.
/// </summary>
public interface IEmailService
{
	/// <summary>
	/// Launches the device's email application with a pre-composed <see cref="Email"/>.
	/// </summary>
	/// /// <param name="ct">The cancellation token.</param>
	/// <param name="email">The email to compose.</param>
	Task Compose(CancellationToken ct, Email email);
}
