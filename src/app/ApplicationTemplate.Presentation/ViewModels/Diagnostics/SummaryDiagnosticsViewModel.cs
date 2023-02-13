using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation;

public partial class SummaryDiagnosticsViewModel : ViewModel
{
	private readonly DateTimeOffset _now = DateTimeOffset.Now;
	private readonly DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

	public string Summary => this.Get(GetSummary);

	public IDynamicCommand SendSummary => this.GetCommandFromTask(async ct =>
	{
		var summary = GetSummary();

		var message = new Email
		{
			Subject = $"Diagnostics - {GetType().Assembly.GetName().Name} ({_now})",
			Body = summary,
		};

		foreach (var logFilePath in this.GetService<ILogFilesProvider>().GetLogFilesPaths())
		{
			if (File.Exists(logFilePath))
			{
				message.Attachments.Add(new EmailAttachment(contentType: "text/plain", fullPath: logFilePath));
			}
		}

		await RunOnDispatcher(ct, _ => this.GetService<IEmailService>().Compose(message));

		this.GetService<ILogger<SummaryDiagnosticsViewModel>>().LogInformation("Environment summary sent.");
	});

	private string GetSummary()
	{
		var versionProvider = this.GetService<IVersionProvider>();
		var deviceInformationProvider = this.GetService<IDeviceInformationProvider>();

		var logFilesProvider = this.GetService<ILogFilesProvider>();
		var loggingOptions = this.GetOptionsValue<LoggingOutputOptions>();

		var stringBuilder = new StringBuilder();

		stringBuilder.AppendLine($"Project: {GetType().Assembly.GetName().Name}");

		stringBuilder.AppendLine($"Date on device: {_now}");
		stringBuilder.AppendLine($"Date on device (UTC): {_utcNow}");

		stringBuilder.AppendLine($"Version: {versionProvider.VersionString}");
		stringBuilder.AppendLine($"Build: {versionProvider.BuildString}");

		stringBuilder.AppendLine($"OS: {deviceInformationProvider.OperatingSystem}");
		stringBuilder.AppendLine($"OS version: {deviceInformationProvider.OperatingSystemVersion}");
		stringBuilder.AppendLine($"Device type: {deviceInformationProvider.DeviceType}");

		// UserAgent Not available in X.E but we could do it in app, here's the implementation.
		// stringBuilder.AppendLine($"User agent: {environmentService.UserAgent}");
		// Android: UserAgent = $"{applicationName}/{AppVersion.ToString()}({DeviceType}; Android {OSVersionNumber})";
		// iOS :UserAgent = $"{applicationName}/{AppVersion.ToString()}({DeviceType}; iOS {OSVersionNumber})";
		stringBuilder.AppendLine($"Culture: {CultureInfo.CurrentCulture.Name}");

		stringBuilder.AppendLine($"Environment: {ConfigurationConfiguration.AppEnvironment.GetCurrent(null)}");

		stringBuilder.AppendLine($"Build environment: {ConfigurationConfiguration.DefaultEnvironment}");

//-:cnd:noEmit
#if DEBUG
		var isDebug = true;
#else
		var isDebug = false;
#endif
//+:cnd:noEmit

		stringBuilder.AppendLine($"Debug build: {isDebug}");

		stringBuilder.AppendLine($"Console logging enabled: {loggingOptions.IsConsoleLoggingEnabled}");

		stringBuilder.AppendLine($"File logging enabled: {loggingOptions.IsFileLoggingEnabled}");

		var hasLogFile = File.Exists(logFilesProvider.GetLogFilesPaths().First());
		stringBuilder.AppendLine($"Has log file: {hasLogFile}");

		stringBuilder.Append(GetStartupDetails());

		return stringBuilder.ToString();
	}

	private string GetStartupDetails()
	{
		var stringBuilder = new StringBuilder();

		var startup = this.GetService<StartupBase>();
		var startupTime = startup.StartActivity.StartTimeUtc + startup.StartActivity.Duration - startup.PreInitializeActivity.StartTimeUtc;

		stringBuilder.AppendLine($"Startup time: {Math.Round(startupTime.TotalMilliseconds)} ms");

		stringBuilder.AppendLine(GetFormattedActivity(startup.PreInitializeActivity));
		stringBuilder.AppendLine(GetFormattedActivity(startup.InitializeActivity, startup.PreInitializeActivity));
		stringBuilder.AppendLine(GetFormattedActivity(startup.CoreStartup.BuildCoreHostActivity, prefix: "  "));
		stringBuilder.AppendLine(GetFormattedActivity(startup.CoreStartup.BuildHostActivity, prefix: "  "));
		stringBuilder.AppendLine(GetFormattedActivity(startup.ShellActivity, startup.InitializeActivity));
		stringBuilder.AppendLine(GetFormattedActivity(startup.StartActivity, startup.ShellActivity));

		string GetFormattedActivity(Activity activity, Activity previousActivity = null, string prefix = null)
		{
			var sb = new StringBuilder();

			if (prefix != null)
			{
				sb.Append(prefix);
			}

			sb.Append($"- {activity.OperationName}");
			sb.Append($": {Math.Round(activity.Duration.TotalMilliseconds)} ms");
			sb.Append($" @ {activity.StartTimeUtc.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture)}");

			// This is the time between the start of the current activity and the end of the previous activity.
			// We want this to be as low as possible otherwise the actvity itself might not be taking long
			// but the process (the sum of all the activities) may still be lengthy.
			var blankSpot = previousActivity != null
				? Math.Round((activity.StartTimeUtc - previousActivity.StartTimeUtc - previousActivity.Duration).TotalMilliseconds)
				: default(double?);

			if (blankSpot != null)
			{
				sb.Append($" [...{blankSpot} ms]");
			}

			return sb.ToString();
		}

		return stringBuilder.ToString();
	}
}
