using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageDialogService;
using Windows.Storage;
using Windows.System;

namespace ApplicationTemplate.Views;

public class DiagnosticsService : IDiagnosticsService
{
	private readonly IMessageDialogService _messageDialogService;
	private readonly IDispatcherScheduler _dispatcherScheduler;

	public DiagnosticsService(IMessageDialogService messageDialogService, IDispatcherScheduler dispatcherScheduler)
	{
		_messageDialogService = messageDialogService;
		_dispatcherScheduler = dispatcherScheduler;
	}

	public bool CanOpenSettingsFolder { get; } =
//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
		true;
//-:cnd:noEmit
#else
//+:cnd:noEmit
		false;
//-:cnd:noEmit
#endif
//+:cnd:noEmit

	public void OpenSettingsFolder()
	{
		var localFolder = ApplicationData.Current.LocalFolder;

		_dispatcherScheduler.ScheduleTask(async (ct2, s) =>
		{
//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
			await Launcher.LaunchFolderAsync(localFolder).AsTask(ct2);
//-:cnd:noEmit
#endif
//+:cnd:noEmit
		});
	}

	public async Task TestExceptionFromMainThread(CancellationToken ct)
	{
//-:cnd:noEmit
		// This will not crash on Android as it can be safely handled.
#if !__ANDROID__
//+:cnd:noEmit
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
//-:cnd:noEmit
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
