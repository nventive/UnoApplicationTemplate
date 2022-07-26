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
			CanOpenSettingsFolder = this.GetService<IDiagnosticsService>().CanOpenSettingsFolder;

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
			this.GetService<IDiagnosticsService>().OpenSettingsFolder();
		});

		public bool CanOpenSettingsFolder { get; }

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
