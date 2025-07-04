// src/app/ApplicationTemplate.Shared.Views/PlatformServices/AmbientLight/AmbientLightProvider.cs
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess.PlatformServices;
using Windows.Devices.Sensors;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class AmbientLightProvider : IAmbientLightProvider, IDisposable
{
	private readonly LightSensor _lightSensor;
	private readonly BehaviorSubject<float> _currentReadingSubject;
	private bool _disposed;

	public AmbientLightProvider()
	{
		_lightSensor = LightSensor.GetDefault();
		_currentReadingSubject = new BehaviorSubject<float>(0f);

		if (_lightSensor != null)
		{
			_lightSensor.ReadingChanged += OnReadingChanged;

			// Get initial reading if available
			var initialReading = _lightSensor.GetCurrentReading();
			if (initialReading != null)
			{
				_currentReadingSubject.OnNext(initialReading.IlluminanceInLux);
			}
		}
	}

	public float GetCurrentReading()
	{
		if (_lightSensor == null)
			return 0f;

		var reading = _lightSensor.GetCurrentReading();
		return reading?.IlluminanceInLux ?? 0f;
	}

	public IObservable<float> GetAndObserveCurrentReading()
	{
		return _currentReadingSubject.AsObservable();
	}

	private void OnReadingChanged(LightSensor sender, LightSensorReadingChangedEventArgs args)
	{
		_currentReadingSubject.OnNext(args.Reading.IlluminanceInLux);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			if (_lightSensor != null)
			{
				_lightSensor.ReadingChanged -= OnReadingChanged;
			}

			_currentReadingSubject?.Dispose();
			_disposed = true;
		}
	}
}
