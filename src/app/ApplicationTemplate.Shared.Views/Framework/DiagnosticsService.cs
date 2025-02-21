using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using ApplicationTemplate.Presentation;
using MessageDialogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Windows.Storage;

namespace ApplicationTemplate.Views;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Default behavior is acceptable for diagnostics.")]
public sealed class DiagnosticsService : IDiagnosticsService
{
	private readonly DateTimeOffset _now = DateTimeOffset.Now;
	private readonly DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

	private readonly IMessageDialogService _messageDialogService;
	private readonly IDispatcherScheduler _dispatcherScheduler;
	private readonly IOptions<ReadOnlyConfigurationOptions> _configurationOptions;
	private readonly ILogger _logger;
	private readonly IEmailService _emailRepository;
	private readonly ILogFilesProvider _logFilesProvider;
	private readonly LoggingOutputOptions _loggingOutputOptions;
	private readonly IEnvironmentManager _environmentManager;
	private readonly IVersionProvider _versionProvider;
	private readonly IDeviceInformationProvider _deviceInformationProvider;
	private readonly StartupBase _startupBase;

	public DiagnosticsService(
		IMessageDialogService messageDialogService,
		IDispatcherScheduler dispatcherScheduler,
		IOptions<ReadOnlyConfigurationOptions> configurationOptions,
		ILogger<DiagnosticsService> logger,
		IEmailService emailRepository,
		ILogFilesProvider logFilesProvider,
		IOptionsMonitor<LoggingOutputOptions> optionsMonitor,
		IEnvironmentManager environmentManager,
		IVersionProvider versionProvider,
		IDeviceInformationProvider deviceInformationProvider,
		StartupBase startupBase
		)
	{
		_messageDialogService = messageDialogService;
		_dispatcherScheduler = dispatcherScheduler;
		_configurationOptions = configurationOptions;
		_logger = logger;
		_emailRepository = emailRepository;
		_logFilesProvider = logFilesProvider;
		_loggingOutputOptions = optionsMonitor.CurrentValue;
		_environmentManager = environmentManager;
		_versionProvider = versionProvider;
		_deviceInformationProvider = deviceInformationProvider;
		_startupBase = startupBase;
	}

	public void DeleteConfigurationOverrideFile()
	{
		_logger.LogDebug("Deleting configuration override file.");

		var filePath = Path.Combine(_configurationOptions.Value.ConfigurationOverrideFolderPath, ConfigurationConfiguration.AppSettingsOverrideFileNameWithExtension);
		File.Delete(filePath);

		_logger.LogInformation("Deleted configuration override file.");
	}

	public bool CanOpenSettingsFolder { get; } =
//-:cnd:noEmit
#if __WINDOWS__
		true;
#else
		false;
#endif
//+:cnd:noEmit

	public void OpenSettingsFolder()
	{
		var localFolder = ApplicationData.Current.LocalFolder;

		_dispatcherScheduler.ScheduleTask(async (ct2, s) =>
		{
//-:cnd:noEmit
#if __WINDOWS__
			await Windows.System.Launcher.LaunchFolderAsync(localFolder).AsTask(ct2);
#endif
//+:cnd:noEmit
		});
	}

	public async Task TestExceptionFromMainThread(CancellationToken ct)
	{
//-:cnd:noEmit
// This will not crash on Android as it can be safely handled.
#if !__ANDROID__
		var confirmation = await _messageDialogService.ShowMessage(ct, mb => mb
			.Title("Diagnostics")
			.Content("This should crash your application. Make sure your analytics provider receives a crash log.")
			.CancelCommand()
			.Command(MessageDialogResult.Accept, label: "Crash")
		);

		if (confirmation != MessageDialogResult.Accept)
		{
			return;
		}
#endif
//+:cnd:noEmit

//-:cnd:noEmit
#if __IOS__
//+:cnd:noEmit
		/// This will be handled by <see cref="AppDomain.CurrentDomain.UnhandledException" />
		UIKit.UIApplication.SharedApplication.InvokeOnMainThread(() => throw new InvalidOperationException("This is a test of an exception in the MainThread. Please ignore."));
//-:cnd:noEmit
#elif __ANDROID__
//+:cnd:noEmit
		/// This will be handled by <see cref="Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser" />
		var _ = new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() => throw new InvalidOperationException("This is a test of an exception in the MainLooper. Please ignore."));
		await Task.CompletedTask;
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}

	public async Task SendSummary(CancellationToken ct)
	{
		var summary = GetSummary();

		var message = new Email
		{
			Subject = $"Diagnostics - {GetType().Assembly.GetName().Name} ({_now})",
			Body = summary,
		};

		foreach (var logFilePath in _logFilesProvider.GetLogFilesPaths())
		{
			if (File.Exists(logFilePath))
			{
				message.Attachments.Add(new EmailAttachment(contentType: "text/plain", fullPath: logFilePath));
			}
		}

		await _dispatcherScheduler.Run(_ => _emailRepository.Compose(message), ct);

		_logger.LogInformation("Environment summary sent.");
	}

	public string GetSummary()
	{
		var stringBuilder = new StringBuilder();

		stringBuilder.AppendLine($"Project: {GetType().Assembly.GetName().Name}");

		stringBuilder.AppendLine($"Date on device: {_now}");
		stringBuilder.AppendLine($"Date on device (UTC): {_utcNow}");

		stringBuilder.AppendLine($"Version: {_versionProvider.VersionString}");
		stringBuilder.AppendLine($"Build: {_versionProvider.BuildString}");

		stringBuilder.AppendLine($"OS: {_deviceInformationProvider.OperatingSystem}");
		stringBuilder.AppendLine($"OS version: {DecodeSytemVersion(_deviceInformationProvider.OperatingSystemVersion)}");
		stringBuilder.AppendLine($"Device type: {_deviceInformationProvider.DeviceType}");

		// UserAgent Not available in X.E but we could do it in app, here's the implementation.
		// stringBuilder.AppendLine($"User agent: {environmentService.UserAgent}");
		// Android: UserAgent = $"{applicationName}/{AppVersion.ToString()}({DeviceType}; Android {OSVersionNumber})";
		// iOS :UserAgent = $"{applicationName}/{AppVersion.ToString()}({DeviceType}; iOS {OSVersionNumber})";
		stringBuilder.AppendLine($"Culture: {CultureInfo.CurrentCulture.Name}");

		stringBuilder.AppendLine($"Environment: {_environmentManager.Current}");

		stringBuilder.AppendLine($"Build environment: {_environmentManager.Default}");

		//-:cnd:noEmit
#if DEBUG
		var isDebug = true;
#else
		var isDebug = false;
#endif
		//+:cnd:noEmit

		stringBuilder.AppendLine($"Debug build: {isDebug}");

		stringBuilder.AppendLine($"Console logging enabled: {_loggingOutputOptions.IsConsoleLoggingEnabled}");

		stringBuilder.AppendLine($"File logging enabled: {_loggingOutputOptions.IsFileLoggingEnabled}");

		var hasLogFile = File.Exists(_logFilesProvider.GetLogFilesPaths().First());
		stringBuilder.AppendLine($"Has log file: {hasLogFile}");

		stringBuilder.Append(GetStartupDetails());

		return stringBuilder.ToString();
	}

	private string GetStartupDetails()
	{
		var stringBuilder = new StringBuilder();

		var startupTime = _startupBase.StartActivity.StartTimeUtc + _startupBase.StartActivity.Duration - _startupBase.PreInitializeActivity.StartTimeUtc;

		stringBuilder.AppendLine($"Startup time: {Math.Round(startupTime.TotalMilliseconds)} ms");

		stringBuilder.AppendLine(GetFormattedActivity(_startupBase.PreInitializeActivity));
		stringBuilder.AppendLine(GetFormattedActivity(_startupBase.InitializeActivity, _startupBase.PreInitializeActivity));
		stringBuilder.AppendLine(GetFormattedActivity(_startupBase.CoreStartup.BuildCoreHostActivity, prefix: "  "));
		stringBuilder.AppendLine(GetFormattedActivity(_startupBase.CoreStartup.BuildHostActivity, prefix: "  "));
		stringBuilder.AppendLine(GetFormattedActivity(_startupBase.ShellActivity, _startupBase.InitializeActivity));
		stringBuilder.AppendLine(GetFormattedActivity(_startupBase.StartActivity, _startupBase.ShellActivity));

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

	private Version DecodeSytemVersion(string operatingSystemVersion)
	{
		// The OS Version provided from DeviceFamilyVersion needs to be decoded to show the OS Version that we commonly use.
		var v = ulong.Parse(operatingSystemVersion, CultureInfo.InvariantCulture);
		var major = (v & 0xFFFF000000000000L) >> 48;
		var minor = (v & 0x0000FFFF00000000L) >> 32;
		var build = (v & 0x00000000FFFF0000L) >> 16;
		var revision = v & 0x000000000000FFFFL;
		return new Version((int)major, (int)minor, (int)build, (int)revision);
	}
}
