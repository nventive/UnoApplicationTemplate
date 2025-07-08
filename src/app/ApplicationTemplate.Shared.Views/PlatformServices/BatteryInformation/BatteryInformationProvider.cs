using System;
using System.Reactive.Linq;
using Windows.System.Power;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The <see cref="IBatteryInformationProvider"/> implementation using Uno.
/// </summary>
public sealed class BatteryInformationProvider : IBatteryInformationProvider
{
	/// <inheritdoc/>
	public BatteryStatus BatteryStatus => PowerManager.BatteryStatus.ToInternalBatteryStatus();

	/// <inheritdoc/>
	public EnergySaverStatus EnergySaverStatus => PowerManager.EnergySaverStatus.ToInternalEnergySaverStatus();

	/// <inheritdoc/>
	public PowerSupplyStatus PowerSupplyStatus => PowerManager.PowerSupplyStatus.ToInternalPowerSupplyStatus();

	/// <inheritdoc/>
	public int RemainingChargePercent => PowerManager.RemainingChargePercent;

	/// <inheritdoc/>
	public TimeSpan RemainingDischargeTime
#if __WINDOWS__
		=> PowerManager.RemainingDischargeTime;
#else
		=> TimeSpan.Zero;
#endif

	/// <inheritdoc/>
	public IObservable<BatteryStatus> GetAndObserveBatteryStatus()
	{
		return Observable.FromEventPattern<EventHandler<object>, object>(
			h => PowerManager.BatteryStatusChanged += h,
			h => PowerManager.BatteryStatusChanged -= h
		)
		.Select(_ => BatteryStatus)
		.StartWith(BatteryStatus)
		.DistinctUntilChanged();
	}

	/// <inheritdoc/>
	public IObservable<EnergySaverStatus> GetAndObserveEnergySaverStatus()
	{
		return Observable.FromEventPattern<EventHandler<object>, object>(
			h => PowerManager.EnergySaverStatusChanged += h,
			h => PowerManager.EnergySaverStatusChanged -= h
		)
		.Select(_ => EnergySaverStatus)
		.StartWith(EnergySaverStatus)
		.DistinctUntilChanged();
	}

	/// <inheritdoc/>
	public IObservable<PowerSupplyStatus> GetAndObservePowerSupplyStatus()
	{
		return Observable.FromEventPattern<EventHandler<object>, object>(
			h => PowerManager.PowerSupplyStatusChanged += h,
			h => PowerManager.PowerSupplyStatusChanged -= h
		)
		.Select(_ => PowerSupplyStatus)
		.StartWith(PowerSupplyStatus)
		.DistinctUntilChanged();
	}

	/// <inheritdoc/>
	public IObservable<int> GetAndObserveRemainingChargePercent()
	{
		return Observable.FromEventPattern<EventHandler<object>, object>(
			h => PowerManager.RemainingChargePercentChanged += h,
			h => PowerManager.RemainingChargePercentChanged -= h
		)
		.Select(_ => RemainingChargePercent)
		.StartWith(RemainingChargePercent)
		.DistinctUntilChanged();
	}

	/// <inheritdoc/>
	public IObservable<TimeSpan> GetAndObserveRemainingDischargeTime()
	{
#if __WINDOWS__
		return Observable.FromEventPattern<EventHandler<object>, object>(
			h => PowerManager.RemainingDischargeTimeChanged += h,
			h => PowerManager.RemainingDischargeTimeChanged -= h
		)
		.Select(_ => RemainingDischargeTime)
		.StartWith(RemainingDischargeTime)
		.DistinctUntilChanged();
#else
		return Observable.Return(RemainingDischargeTime);
#endif
	}
}
