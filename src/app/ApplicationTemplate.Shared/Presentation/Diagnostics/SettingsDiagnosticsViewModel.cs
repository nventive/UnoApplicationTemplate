using System;
using System.Reactive.Concurrency;
using Chinook.DynamicMvvm;
using Windows.Storage;
using Windows.System;

namespace ApplicationTemplate
{
	public class SettingsDiagnosticsViewModel : ViewModel
	{
		public IDynamicCommand OpenSettingsFolder => this.GetCommand(() =>
		{
			var localFolder = ApplicationData.Current.LocalFolder;

			this.GetService<IDispatcherScheduler>().ScheduleTask(async (ct2, s) =>
			{
				await Launcher.LaunchFolderAsync(localFolder).AsTask(ct2);
			});
		});

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
	}
}
