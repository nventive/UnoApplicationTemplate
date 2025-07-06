using System;
using System.Reactive.Linq;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The <see cref="IAccelerometerService"/> implementation using Uno.
/// </summary>
public sealed class AccelerometerService : IAccelerometerService
{
	private readonly Accelerometer _accelerometer;

	public AccelerometerService()
	{
		_accelerometer = Accelerometer.GetDefault();
	}

	/// <inheritdoc/>
	public uint ReportInterval
	{
		get => _accelerometer?.ReportInterval ?? 0;
		set
		{
			if (_accelerometer is null)
			{
				return;
			}

			_accelerometer.ReportInterval = value;
		}
	}

	/// <inheritdoc/>
	public IObservable<AccelerometerReading> ObserveAcceleration()
	{
		if (_accelerometer is null)
		{
			return Observable.Return<AccelerometerReading>(null);
		}

		return Observable.FromEventPattern<TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>, AccelerometerReadingChangedEventArgs>(
			h => _accelerometer.ReadingChanged += h,
			h => _accelerometer.ReadingChanged -= h
		)
		.Select(eventPattern =>
		{
			var reading = eventPattern.EventArgs.Reading;

			return new AccelerometerReading(
				reading.AccelerationX,
				reading.AccelerationY,
				reading.AccelerationZ,
				reading.PerformanceCount,
				reading.Timestamp
			);
		});
	}

	/// <inheritdoc/>
	public IObservable<DateTimeOffset> ObserveDeviceShaken()
	{
		if (_accelerometer is null)
		{
			return Observable.Return(DateTimeOffset.MinValue);
		}

		return Observable.FromEventPattern<TypedEventHandler<Accelerometer, AccelerometerShakenEventArgs>, AccelerometerShakenEventArgs>(
			h => _accelerometer.Shaken += h,
			h => _accelerometer.Shaken -= h
		)
		.Select(eventPattern => eventPattern.EventArgs.Timestamp);
	}
}
