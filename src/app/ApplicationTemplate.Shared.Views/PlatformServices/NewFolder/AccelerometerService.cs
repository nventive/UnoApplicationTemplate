// path/to/AccelerometerService.cs
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Devices.Sensors;
using Microsoft.UI.Dispatching;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	/// <summary>
	/// Implementation of IAccelerometerService using Uno Platform's cross-platform accelerometer API.
	/// </summary>
	public sealed class AccelerometerService : IAccelerometerService
	{
		private readonly DispatcherQueue _dispatcherQueue;
		private Accelerometer _accelerometer;
		private BehaviorSubject<AccelerometerReading> _accelerationSubject;
		private bool _isShakenObserving;

		public AccelerometerService(DispatcherQueue dispatcherQueue)
		{
			_dispatcherQueue = dispatcherQueue;
			InitializeAccelerometer();
		}

		private void InitializeAccelerometer()
		{
			_accelerometer = Accelerometer.GetDefault();
			if (_accelerometer != null)
			{
				_accelerometer.ReportInterval = ReportInterval;
				_accelerometer.ReadingChanged += OnReadingChanged;
			}
		}

		private void OnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
		{
			_dispatcherQueue.TryEnqueue(() =>
			{
				var reading = args.Reading;
				var timestamp = DateTimeOffset.FromFileTime(reading.Timestamp);
				var performanceCount = TimeSpan.FromTicks(reading.PerformanceCount.HasValue ? reading.PerformanceCount.Value : 0);
				var accelReading = new AccelerometerReading(
					reading.AccelerationX,
					reading.AccelerationY,
					reading.AccelerationZ,
					performanceCount,
					timestamp
				);
				_accelerationSubject?.OnNext(accelReading);
			});
		}

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

		public IObservable<AccelerometerReading> ObserveAcceleration()
		{
			if (_accelerometer == null)
			{
				return Observable.Return(null);
			}

			_accelerationSubject = new BehaviorSubject<AccelerometerReading>(null);
			return _accelerationSubject.AsObservable();
		}

		public IObservable<DateTimeOffset> ObserveDeviceShaken()
		{
			if (_accelerometer == null)
			{
				return Observable.Return(DateTimeOffset.Now);
			}

			var accelerationObservable = ObserveAcceleration().Where(r => r != null);
			var shakeObservable = accelerationObservable
				.Buffer(TimeSpan.FromSeconds(0.5), 5)  // Buffer last 5 readings in 0.5s
				.Select(buffer => buffer.Any(r =>
					Math.Sqrt(Math.Pow(r.AccelerationX, 2) + Math.Pow(r.AccelerationY, 2) + Math.Pow(r.AccelerationZ, 2)) > 1.5
				))
				.Where(isShake => isShake)
				.Select(_ => DateTimeOffset.Now)
				.Throttle(TimeSpan.FromSeconds(2));  // Debounce to avoid multiple triggers

			return shakeObservable;
		}
	}
}
