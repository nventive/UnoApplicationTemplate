using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation;

public class SettingsDiagnosticsViewModel : ViewModel
{
	public SettingsDiagnosticsViewModel()
	{
		CanOpenSettingsFolder = this.GetService<IDiagnosticsService>().CanOpenSettingsFolder;
	}

	public bool IsDiagnosticsOverlayEnabled
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.IsDiagnosticsOverlayEnabled);
		set => this.Set(value);
	}

	public IDynamicCommand OpenSettingsFolder => this.GetCommand(() =>
	{
		this.GetService<IDiagnosticsService>().OpenSettingsFolder();
	});

	public bool CanOpenSettingsFolder { get; }

	public bool IsMockEnabled
	{
		get => this.GetFromOptionsMonitor<MockOptions, bool>(o => o.IsMockEnabled);
		set => this.Set(value);
	}

	public IDynamicCommand NotifyNeedsRestart => this.GetNotifyNeedsRestartCommand();
}
