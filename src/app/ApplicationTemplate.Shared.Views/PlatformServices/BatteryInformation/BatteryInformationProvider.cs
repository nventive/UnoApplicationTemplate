// src/app/ApplicationTemplate.Shared.Views/Configuration/BatteryInformationProvider.cs
using System;
using System.Reactive.Linq;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	/// 
	/// Implementation of IBatteryInformationProvider using Uno Platform.
	/// 
	public class BatteryInformationProvider : IBatteryInformationProvider
	{
		public BatteryStatus BatteryStatus => Windows.System.Power.Battery.Report.Status;

		public EnergySaverStatus EnergySaverStatus => Windows.System.Power.EnergySaverInfo.GetCurrentStatus();

		public PowerSupplyStatus PowerSupplyStatus => Windows.System.Power.Battery.Report.PowerSupplyStatus;

		public int RemainingChargePercent => Windows.System.Power.Battery.Report.RemainingCapacityPercent;

		public TimeSpan RemainingDischargeTime => Windows.System.Power.Battery.Report.RemainingDischargeTime;

		public IObservable GetAndObserveBatteryStatus()
		{
			return Observable.FromEventPattern(
				h => Windows.System.Power.Battery.ReportStatusChanged += h,
				h => Windows.System.Power.Battery.ReportStatusChanged -= h
			).Select(_ => Windows.System.Power.Battery.Report.Status);
		}

		public IObservable GetAndObserveEnergySaverStatus()
		{
			return Observable.FromEventPattern(
				h => Windows.System.Power.EnergySaverInfo.StatusChanged += h,
				h => Windows.System.Power.EnergySaverInfo.StatusChanged -= h
			).Select(_ => Windows.System.Power.EnergySaverInfo.GetCurrentStatus());
		}

		public IObservable GetAndObservePowerSupplyStatus()
		{
			return Observable.FromEventPattern(
				h => Windows.System.Power.Battery.ReportStatusChanged += h,
				h => Windows.System.Power.Battery.ReportStatusChanged -= h
			).Select(_ => Windows.System.Power.Battery.Report.PowerSupplyStatus);
		}

		public IObservable GetAndObserveRemainingChargePercent()
		{
			return Observable.FromEventPattern(
				h => Windows.System.Power.Battery.ReportStatusChanged += h,
				h => Windows.System.Power.Battery.ReportStatusChanged -= h
			).Select(_ => Windows.System.Power.Battery.Report.RemainingCapacityPercent);
		}

		public IObservable GetAndObserveRemainingDischargeTime()
		{
			return Observable.FromEventPattern(
				h => Windows.System.Power.Battery.ReportStatusChanged += h,
				h => Windows.System.Power.Battery.ReportStatusChanged -= h
			).Select(_ => Windows.System.Power.Battery.Report.RemainingDischargeTime);
		}
	}
}
