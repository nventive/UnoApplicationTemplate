using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Uno.Extensions;
using Uno.Logging;

namespace ApplicationTemplate.Presentation
{
	public partial class DiagnosticsOverlayViewModel : ViewModel
	{
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

		public IDynamicCommand NavigateToHttpTracingDiagnosticsPage => this.GetCommandFromTask(async ct =>
		{
			if (this.GetService<ISectionsNavigator>().GetActiveViewModel() is HttpTracingDiagnosticsPageViewModel vm)
			{
				await vm.NavigateBack.Execute(ct);
			}
			else
			{
				await this.GetService<ISectionsNavigator>().OpenModal(ct, () => new HttpTracingDiagnosticsPageViewModel());
			}
		});

		public bool IsDiagnosticsOverlayEnabled => DiagnosticsConfiguration.DiagnosticsOverlay.GetIsEnabled();

		public bool IsAlignmentGridEnabled
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public IDynamicCommand ToggleAlignmentGrid => this.GetCommand(() =>
		{
			IsAlignmentGridEnabled = !IsAlignmentGridEnabled;
		});

		public bool IsDiagnosticsExpanded
		{
			get => this.Get<bool>();
			set => this.Set(value);
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
}
