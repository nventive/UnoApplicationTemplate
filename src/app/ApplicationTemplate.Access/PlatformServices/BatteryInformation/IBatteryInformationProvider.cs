using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Indicates the status of the battery.
/// </summary>
public enum BatteryStatus
{
	/// <summary>
	/// The battery or battery controller is not present.
	/// </summary>
	NotPresent,

	/// <summary>
	/// The battery is discharging.
	/// </summary>
	Discharging,

	/// <summary>
	/// The battery is idle.
	/// </summary>
	Idle,

	/// <summary>
	/// The battery is charging.
	/// </summary>
	Charging,
}

/// <summary>
/// Specifies the status of battery saver.
/// </summary>
public enum EnergySaverStatus
{
	/// <summary>
	/// Battery saver is off permanently or the device is plugged in.
	/// </summary>
	Disabled,

	/// <summary>
	/// Battery saver is off now, but ready to turn on automatically.
	/// </summary>
	Off,

	/// <summary>
	/// Battery saver is on. Save energy where possible.
	/// </summary>
	On,
}

/// <summary>
/// Represents the device's power supply status.
/// </summary>
public enum PowerSupplyStatus
{
	/// <summary>
	/// The device has no power supply.
	/// </summary>
	NotPresent,

	/// <summary>
	/// The device has an inadequate power supply.
	/// </summary>
	Inadequate,

	/// <summary>
	/// he device has an adequate power supply.
	/// </summary>
	Adequate,
}

/// <summary>
/// Provides access to battery information on the device.
/// </summary>
public interface IBatteryInformationProvider
{
	/// <summary>
	/// Gets the device's battery status.
	/// </summary>
	BatteryStatus BatteryStatus { get; }

	/// <summary>
	/// Gets the devices's battery saver status, indicating when to save energy.
	/// </summary>
	EnergySaverStatus EnergySaverStatus { get; }

	/// <summary>
	/// Gets the device's power supply status.
	/// </summary>
	PowerSupplyStatus PowerSupplyStatus { get; }

	/// <summary>
	/// Gets the total percentage of charge remaining from all batteries connected to the device.
	/// </summary>
	int RemainingChargePercent { get; }

	/// <summary>
	/// Gets the total runtime remaining from all batteries connected to the device.
	/// </summary>
	TimeSpan RemainingDischargeTime { get; }

	/// <summary>
	/// Gets and observes the current battery status.
	/// </summary>
	/// <returns>An observable sequence yielding the curent battery status.</returns>
	IObservable<BatteryStatus> GetAndObserveBatteryStatus();

	/// <summary>
	/// Gets and observes the current energy saver status.
	/// </summary>
	/// <returns>An observable sequence yielding the curent energy saver status.</returns>
	IObservable<EnergySaverStatus> GetAndObserveEnergySaverStatus();

	/// <summary>
	/// Gets and observes the current power supply status.
	/// </summary>
	/// <returns>An observable sequence yielding the curent power supply status.</returns>
	IObservable<PowerSupplyStatus> GetAndObservePowerSupplyStatus();

	/// <summary>
	/// Gets and observes the remaining charge percentage from all batteries connected to the device.
	/// </summary>
	/// <returns>An observable sequence yielding the remaining charge time.</returns>
	IObservable<int> GetAndObserveRemainingChargePercent();

	/// <summary>
	/// Gets and observes the remaining discharge time from all batteries connected to the device.
	/// </summary>
	/// <returns>An observable sequence yielding the remaining discharge time.</returns>
	IObservable<TimeSpan> GetAndObserveRemainingDischargeTime();
}
