// src/app/ApplicationTemplate.Shared.Views/PlatformServices/BatteryInformation/WindowsBatteryInformationProvider.cs
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess.PlatformServices;
using Windows.System.Power;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Windows API implementation of <see cref="IBatteryInformationProvider"/> using Uno Platform.
/// </summary>
public sealed class WindowsBatteryInformationProvider : IBatteryInformationProvider, IDisposable
{
	private readonly BehaviorSubject<BatteryStatus> _batteryStatusSubject;
	private readonly BehaviorSubject<EnergySaverStatus> _energySaverStatusSubject;
	private readonly BehaviorSubject<PowerSupplyStatus> _powerSupplyStatusSubject;
	private readonly BehaviorSubject<int> _remainingChargePercentSubject;
	private readonly BehaviorSubject<TimeSpan> _remainingDischargeTimeSubject;

	public WindowsBatteryInformationProvider()
	{
		_batteryStatusSubject = new BehaviorSubject<BatteryStatus>(GetCurrentBatteryStatus());
		_energySaverStatusSubject = new BehaviorSubject<EnergySaverStatus>(GetCurrentEnergySaverStatus());
		_powerSupplyStatusSubject = new BehaviorSubject<PowerSupplyStatus>(GetCurrentPowerSupplyStatus());
		_remainingChargePercentSubject = new BehaviorSubject<int>(GetCurrentRemainingChargePercent());
		_remainingDischargeTimeSubject = new BehaviorSubject<TimeSpan>(GetCurrentRemainingDischargeTime());

		// Subscribe to Windows power events
		PowerManager.BatteryStatusChanged += OnBatteryStatusChanged;
		PowerManager.EnergySaverStatusChanged += OnEnergySaverStatusChanged;
		PowerManager.PowerSupplyStatusChanged += OnPowerSupplyStatusChanged;
		PowerManager.RemainingChargePercentChanged += OnRemainingChargePercentChanged;
		PowerManager.RemainingDischargeTimeChanged += OnRemainingDischargeTimeChanged;
	}

	/// <inheritdoc/>
	public BatteryStatus BatteryStatus => _batteryStatusSubject.Value;

	/// <inheritdoc/>
	public EnergySaverStatus EnergySaverStatus => _energySaverStatusSubject.Value;

	/// <inheritdoc/>
	public PowerSupplyStatus PowerSupplyStatus => _powerSupplyStatusSubject.Value;

	/// <inheritdoc/>
	public int RemainingChargePercent => _remainingChargePercentSubject.Value;

	/// <inheritdoc/>
	public TimeSpan RemainingDischargeTime => _remainingDischargeTimeSubject.Value;

	/// <inheritdoc/>
	public IObservable<BatteryStatus> GetAndObserveBatteryStatus()
	{
		return _batteryStatusSubject.AsObservable();
	}

	/// <inheritdoc/>
	public IObservable<EnergySaverStatus> GetAndObserveEnergySaverStatus()
	{
		return _energySaverStatusSubject.AsObservable();
	}

	/// <inheritdoc/>
	public IObservable<PowerSupplyStatus> GetAndObservePowerSupplyStatus()
	{
		return _powerSupplyStatusSubject.AsObservable();
	}

	/// <inheritdoc/>
	public IObservable<int> GetAndObserveRemainingChargePercent()
	{
		return _remainingChargePercentSubject.AsObservable();
	}

	/// <inheritdoc/>
	public IObservable<TimeSpan> GetAndObserveRemainingDischargeTime()
	{
		return _remainingDischargeTimeSubject.AsObservable();
	}

	private BatteryStatus GetCurrentBatteryStatus()
	{
		return PowerManager.BatteryStatus switch
		{
			Windows.System.Power.BatteryStatus.NotPresent => BatteryStatus.NotPresent,
			Windows.System.Power.BatteryStatus.Discharging => BatteryStatus.Discharging,
			Windows.System.Power.BatteryStatus.Idle => BatteryStatus.Idle,
			Windows.System.Power.BatteryStatus.Charging => BatteryStatus.Charging,
			_ => BatteryStatus.NotPresent
		};
	}

	private EnergySaverStatus GetCurrentEnergySaverStatus()
	{
		return PowerManager.EnergySaverStatus switch
		{
			Windows.System.Power.EnergySaverStatus.Disabled => EnergySaverStatus.Disabled,
			Windows.System.Power.EnergySaverStatus.Off => EnergySaverStatus.Off,
			Windows.System.Power.EnergySaverStatus.On => EnergySaverStatus.On,
			_ => EnergySaverStatus.Disabled
		};
	}

	private PowerSupplyStatus GetCurrentPowerSupplyStatus()
	{
		return PowerManager.PowerSupplyStatus switch
		{
			Windows.System.Power.PowerSupplyStatus.NotPresent => PowerSupplyStatus.NotPresent,
			Windows.System.Power.PowerSupplyStatus.Inadequate => PowerSupplyStatus.Inadequate,
			Windows.System.Power.PowerSupplyStatus.Adequate => PowerSupplyStatus.Adequate,
			_ => PowerSupplyStatus.NotPresent
		};
	}

	private int GetCurrentRemainingChargePercent()
	{
		return PowerManager.RemainingChargePercent;
	}

	private TimeSpan GetCurrentRemainingDischargeTime()
	{
		return PowerManager.RemainingDischargeTime;
	}

	private void OnBatteryStatusChanged(object sender, object e)
	{
		_batteryStatusSubject.OnNext(GetCurrentBatteryStatus());
	}

	private void OnEnergySaverStatusChanged(object sender, object e)
	{
		_energySaverStatusSubject.OnNext(GetCurrentEnergySaverStatus());
	}

	private void OnPowerSupplyStatusChanged(object sender, object e)
	{
		_powerSupplyStatusSubject.OnNext(GetCurrentPowerSupplyStatus());
	}

	private void OnRemainingChargePercentChanged(object sender, object e)
	{
		_remainingChargePercentSubject.OnNext(GetCurrentRemainingChargePercent());
	}

	private void OnRemainingDischargeTimeChanged(object sender, object e)
	{
		_remainingDischargeTimeSubject.OnNext(GetCurrentRemainingDischargeTime());
	}

	public void Dispose()
	{
		PowerManager.BatteryStatusChanged -= OnBatteryStatusChanged;
		PowerManager.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;
		PowerManager.PowerSupplyStatusChanged -= OnPowerSupplyStatusChanged;
		PowerManager.RemainingChargePercentChanged -= OnRemainingChargePercentChanged;
		PowerManager.RemainingDischargeTimeChanged -= OnRemainingDischargeTimeChanged;

		_batteryStatusSubject?.Dispose();
		_energySaverStatusSubject?.Dispose();
		_powerSupplyStatusSubject?.Dispose();
		_remainingChargePercentSubject?.Dispose();
		_remainingDischargeTimeSubject?.Dispose();
	}
}
