using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation;

public sealed partial class DiagnosticsOverlayViewModel : ViewModel
{
	public DiagnosticsOverlayViewModel()
	{
		Tabs = new TabViewModel[]
		{
			this.AttachChild(new HttpDebuggerViewModel(), "HttpDebugger"),
			this.AttachChild(new NavigationDebuggerViewModel(), "NavigationDebugger"),
		};
		Tabs.SetActiveTabIndex(-1);
	}

	public TabViewModel[] Tabs { get; }

	public int ActiveTabIndex
	{
		get => this.Get(initialValue: -1);
		set
		{
			this.Set(value);
			Tabs.SetActiveTabIndex(value);
		}
	}

	public TabViewModel ActiveTabViewModel => this.GetFromDynamicProperty(this.GetProperty(that => that.ActiveTabIndex), i => i < 0 ? null : Tabs[i]);

	private DiagnosticsCountersService DiagnosticsCountersService => this.GetService<DiagnosticsCountersService>();

	public IDynamicCommand CollectMemory => this.GetCommand(() =>
	{
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
	});

	public IDynamicCommand NavigateToDiagnosticsPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().OpenModal(ct, () => new DiagnosticsPageViewModel());
	});

	public bool IsDiagnosticsOverlayEnabled => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.IsDiagnosticsOverlayEnabled);

	public bool IsAlignmentGridEnabled
	{
		get => this.Get<bool>();
		set => this.Set(value);
	}

	public IDynamicCommand ToggleAlignmentGrid => this.GetCommand(() =>
	{
		IsAlignmentGridEnabled = !IsAlignmentGridEnabled;
	});

	public IDynamicCommand ToggleHttpDebugger => this.GetCommand(() =>
	{
		// The HttpDebugger is currently the only thing in the expanded view.
		// This method will change when we add more things to the expanded view.
		IsDiagnosticsExpanded = !IsDiagnosticsExpanded;
	});

	public bool IsDiagnosticsExpanded
	{
		get => this.Get<bool>();
		set
		{
			this.Set(value);
			if (value)
			{
				if (ActiveTabIndex < 0)
				{
					// Select the first tab when opening the Overlay the first time.
					ActiveTabIndex = 0;
				}

				// Reactivates the active tab when we expand the Overlay.
				Tabs.SetActiveTabIndex(ActiveTabIndex);
			}
			else
			{
				// Deactivates all tabs when we collapsed the Overlay.
				Tabs.SetActiveTabIndex(-1);
			}
		}
	}

	public IDynamicCommand ToggleMore => this.GetCommand(() =>
	{
		IsDiagnosticsExpanded = !IsDiagnosticsExpanded;
	});

	public CountersData Counters => this.GetFromObservable(ObserveCounters, DiagnosticsCountersService.Counters);

	private IObservable<CountersData> ObserveCounters =>
		Observable.FromEventPattern<EventHandler, EventArgs>(
			h => DiagnosticsCountersService.CountersChanged += h,
			h => DiagnosticsCountersService.CountersChanged -= h
		)
		.Select(_ => DiagnosticsCountersService.Counters);
}
