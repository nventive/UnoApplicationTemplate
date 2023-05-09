using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using MessageDialogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Windows.Storage;

namespace ApplicationTemplate.Views;

public class DiagnosticsService : IDiagnosticsService
{
	private readonly IMessageDialogService _messageDialogService;
	private readonly IDispatcherScheduler _dispatcherScheduler;
	private readonly IOptions<ReadOnlyConfigurationOptions> _configurationOptions;
	private readonly ILogger _logger;

	public DiagnosticsService(IMessageDialogService messageDialogService, IDispatcherScheduler dispatcherScheduler, IOptions<ReadOnlyConfigurationOptions> configurationOptions, ILogger<DiagnosticsService> logger)
	{
		_messageDialogService = messageDialogService;
		_dispatcherScheduler = dispatcherScheduler;
		_configurationOptions = configurationOptions;
		_logger = logger;
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
		UIKit.UIApplication.SharedApplication.InvokeOnMainThread(() => throw new Exception("This is a test of an exception in the MainThread. Please ignore."));
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
}
