using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation
{
	public class SettingsDiagnosticsViewModel : ViewModel
	{
		public SettingsDiagnosticsViewModel()
		{
			AddDisposable(this.GetProperty(x => x.IsDiagnosticsOverlayEnabled)
				.Observe()
				.SelectManyDisposePrevious((e, ct) => OnDiagnosticsOverlayChanged(ct, e))
				.Subscribe()
			);
		}

		public bool IsDiagnosticsOverlayEnabled
		{
			get => this.Get(initialValue: this.GetOptionsValue<DiagnosticsOptions>().IsDiagnosticsOverlayEnabled);
			set => this.Set(value);
		}

		public IDynamicCommand OpenSettingsFolder => this.GetCommand(() =>
		{
			throw new NotImplementedException();

			//			var localFolder = ApplicationData.Current.LocalFolder;

			//			this.GetService<IDispatcherScheduler>().ScheduleTask(async (ct2, s) =>
			//			{
			////-:cnd:noEmit
			//#if WINDOWS_UWP
			////+:cnd:noEmit
			//				await Launcher.LaunchFolderAsync(localFolder).AsTask(ct2);
			////-:cnd:noEmit
			//#endif
			////+:cnd:noEmit
			//			});
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

		private async Task OnDiagnosticsOverlayChanged(CancellationToken ct, bool isEnabled)
		{
			var isCurrentlyEnabled = this.GetOptionsValue<DiagnosticsOptions>().IsDiagnosticsOverlayEnabled;

			this.GetService<ILogger<SettingsDiagnosticsViewModel>>().LogInformation("{isEnabled} diagnostics overlay.", isEnabled ? "Enabling" : "Disabling");

			this.GetService<IConfiguration>()["Diagnostics:IsDiagnosticsOverlayEnabled"] = isEnabled.ToString();

			if (isCurrentlyEnabled != isEnabled)
			{
				await this.GetService<IMessageDialogService>().ShowMessage(ct, mb => mb
				   .Title("Diagnostics")
				   .Content("Restart the application to apply your changes.")
				   .OkCommand()
			   );
			}
		}
	}
}
