using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate;

/// <summary>
/// Offers methods to diagnose the application.
/// </summary>
public interface IDiagnosticsService
{
	/// <summary>
	/// Deletes the configuration override file.
	/// </summary>
	void DeleteConfigurationOverrideFile();

	/// <summary>
	/// Throws an exception from the main thread.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	Task TestExceptionFromMainThread(CancellationToken ct);

	/// <summary>
	/// Opens the settings folder where the configuration override file is stored.
	/// </summary>
	void OpenSettingsFolder();

	/// <summary>
	/// Gets a value indicating whether the settings folder can be open.
	/// This value is based on the platform.
	/// </summary>
	bool CanOpenSettingsFolder { get; }

	/// <summary>
	/// Sends the diagnostics summary by email.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	Task SendSummary(CancellationToken ct);

	/// <summary>
	/// Gets the diagnostics summary.
	/// </summary>
	/// <returns>The diagnostics summary.</returns>
	string GetSummary();
}
