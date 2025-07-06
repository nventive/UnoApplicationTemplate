using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides functionality to observe accelerometer readings and detect device shake events.
/// </summary>
/// <remarks>
/// On Android, when both ReadingChanged and Shaken events are attached and the user sets the ReportInterval to a high value, the ReadingChanged event may be raised more often than requested.
/// This is because for multiple subscribers to the same sensor, the system may raise the sensor events with the frequency of the one with the lowest requested report delay.
/// This is, however, in line with the behavior of the WinUI Accelerometer, and you can filter the events as necessary for your use case.
/// </remarks>
public interface IAccelerometerService
{
	/// <summary>
	/// Gets or sets the current report interval for the accelerometer.
	/// </summary>
	uint ReportInterval { get; set; }

	/// <summary>
	/// Observes the acceleration data from the device's accelerometer.
	/// </summary>
	/// <remarks>
	/// If the device does not support an accelerometer sensor, this method will return an observable sequence that yields null.
	/// </remarks>
	IObservable<AccelerometerReading> ObserveAcceleration();

	/// <summary>
	/// Observes when the device is shaken.
	/// </summary>
	/// <remarks>
	/// If the device does not support an accelerometer sensor, this method will return an observable sequence that yields null.
	/// </remarks>
	/// <returns>An observable sequence yielding a timestamp when the device has been shaken.</returns>
	IObservable<DateTimeOffset> ObserveDeviceShaken();
}
