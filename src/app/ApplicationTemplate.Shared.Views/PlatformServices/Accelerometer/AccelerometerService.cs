// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Accelerometer/AccelerometerService.cs
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess.PlatformServices;
using Windows.Devices.Sensors;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Cross-platform implementation of <see cref="IAccelerometerService"/> using Windows API through Uno Platform.
/// </summary>
public sealed class AccelerometerService : IAccelerometerService, IDisposable
{
	private readonly Accelerometer _accelerometer;
	private readonly Subject<AccelerometerReading> _accelerationSubject;
	private readonly Subject<DateTimeOffset> _deviceShakenSubject;
	private bool _disposed;

	public AccelerometerService()
	{
		_accelerometer = Accelerometer.GetDefault();
		_accelerationSubject = new Subject<AccelerometerReading>();
		_deviceShakenSubject = new Subject<DateTimeOffset>();


		if (_accelerometer != null)
		{
			_accelerometer.ReadingChanged += OnReadingChanged;
			_accelerometer.Shaken += OnShaken;
		}
	}

	/// <inheritdoc/>
	public uint ReportInterval
	{
		get => _accelerometer?.ReportInterval ?? 0;
		set
		{
			if (_accelerometer != null)
			{
				_accelerometer.ReportInterval = value;
			}
		}
	}

	/// <inheritdoc/>
	public IObservable<AccelerometerReading> ObserveAcceleration()
	{
		if (_accelerometer == null)
		{
			return Observable.Return<AccelerometerReading>(null);
		}

		return _accelerationSubject.AsObservable();
	}

	/// <inheritdoc/>
	public IObservable<DateTimeOffset> ObserveDeviceShaken()
	{
		if (_accelerometer == null)
		{
			return Observable.Return<DateTimeOffset>(default);
		}

		return _deviceShakenSubject.AsObservable();
	}

	private void OnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
	{
		var reading = args.Reading;
		var accelerometerReading = new AccelerometerReading(
			reading.AccelerationX,
			reading.AccelerationY,
			reading.AccelerationZ,
			null, // Performance count not available in Windows API
			reading.Timestamp
		);

		_accelerationSubject.OnNext(accelerometerReading);
	}

	private void OnShaken(Accelerometer sender, AccelerometerShakenEventArgs args)
	{
		_deviceShakenSubject.OnNext(args.Timestamp);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			if (_accelerometer != null)
			{
				_accelerometer.ReadingChanged -= OnReadingChanged;
				_accelerometer.Shaken -= OnShaken;
			}

			_accelerationSubject?.Dispose();
			_deviceShakenSubject?.Dispose();
			_disposed = true;
		}
	}
}
