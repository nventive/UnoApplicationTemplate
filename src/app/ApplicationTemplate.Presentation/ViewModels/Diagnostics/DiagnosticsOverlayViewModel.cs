using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using ApplicationTemplate.DataAccess.PlatformServices;
using ByteSizeLib;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed class DiagnosticsOverlayViewModel : ViewModel
{
	private readonly IMemoryProvider _memoryProvider;
	private readonly IAmbientLightProvider _ambientLightProvider;

	public DiagnosticsOverlayViewModel()
	{
		ResolveService(out _memoryProvider);
		ResolveService(out _ambientLightProvider);

		Tabs = new TabViewModel[]
		{
			this.AttachChild(new HttpDebuggerViewModel(), "HttpDebugger"),
			this.AttachChild(new NavigationDebuggerViewModel(), "NavigationDebugger"),
			this.AttachChild(new ConfigurationDebuggerViewModel(), "ConfigurationDebugger"),
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

	/// <summary>
	/// Gets a value indicating whether the diagnostic overlay has been enabled by the user.
	/// </summary>
	public bool IsDiagnosticsOverlayEnabled => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.IsDiagnosticsOverlayEnabled);

	/// <summary>
	/// Gets a value indicating whether gets whether the user has closed manually the diagnostic overlay.
	/// </summary>
	public bool IsClosed
	{
		get => this.Get<bool>();
		private set => this.Set(value);
	}

	public IDynamicCommand CloseDiagnostic => this.GetCommand(() =>
	{
		IsClosed = true;
	});

	/// <summary>
	/// Gets a value indicating whether the diagnostic overlay should be displayed.
	/// </summary>
	public bool ShouldDisplayOverlay => this.GetFromObservable(ObserveShouldDisplayOverlay(), initialValue: IsDiagnosticsOverlayEnabled);

	private IObservable<bool> ObserveShouldDisplayOverlay()
	{
		return Observable.CombineLatest(
			this.GetProperty(x => x.IsDiagnosticsOverlayEnabled).GetAndObserve(),
			this.GetProperty(x => x.IsClosed).GetAndObserve(),
			(isEnabled, isClosed) => isEnabled && !isClosed
		);
	}

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

	public bool IsOverlayOnTheLeft
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.IsDiagnosticsOverlayOnTheLeft);
		set => this.Set(value);
	}

	/// <summary>
	/// Gets the VisualState name of the DiagnosticsOverlay.
	/// </summary>
	public string OverlayState => this.GetFromObservable(ObserveOverlayState(), initialValue: "MinimizedRight");

	private IObservable<string> ObserveOverlayState()
	{
		return Observable.CombineLatest(
			this.GetProperty(x => x.IsDiagnosticsExpanded).GetAndObserve(),
			this.GetProperty(x => x.IsOverlayOnTheLeft).GetAndObserve(),
			(isExpanded, isLeft) =>
			{
				if (isExpanded)
				{
					return "Expanded";
				}

				if (isLeft)
				{
					return "MinimizedLeft";
				}

				return "MinimizedRight";
			});
	}

	public IDynamicCommand ToggleMore => this.GetCommand(() =>
	{
		IsDiagnosticsExpanded = !IsDiagnosticsExpanded;
	});

	public IDynamicCommand ToggleSide => this.GetCommand(() =>
	{
		IsOverlayOnTheLeft = !IsOverlayOnTheLeft;
	});

	public CountersData Counters => this.GetFromObservable(ObserveCounters, DiagnosticsCountersService.Counters);

	private IObservable<CountersData> ObserveCounters =>
		Observable.FromEventPattern<EventHandler, EventArgs>(
			h => DiagnosticsCountersService.CountersChanged += h,
			h => DiagnosticsCountersService.CountersChanged -= h
		)
		.Select(_ => DiagnosticsCountersService.Counters);

	public string PrivateMemorySize => this.GetFromObservable(
		source: _memoryProvider.ObservePrivateMemorySize().Select(x => ByteSize.FromBytes(x).ToString("0.#", CultureInfo.InvariantCulture)),
		initialValue: string.Empty
	);

	public string ManagedMemorySize => this.GetFromObservable(
		source: _memoryProvider.ObserveManagedMemorySize().Select(x => ByteSize.FromBytes(x).ToString("0.#", CultureInfo.InvariantCulture)),
		initialValue: string.Empty
	);

	public string AmbientLight => this.GetFromObservable(
		source: _ambientLightProvider.GetAndObserveCurrentReading().Select(x => $"{x:F1} lux"),
		initialValue: "0.0 lux"
	);
}
