// path/to/file.cs
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Devices.Sensors;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides access to the current ambient light reading and detects changes.
/// </summary>
/// <remarks>
/// The device or emulator that you're using must support an ambient light sensor.
/// </remarks>
public class AmbientLightProvider : IAmbientLightProvider
{
	private Luxometer _luxometer;
	private Subject<float> _readingSubject;

	public AmbientLightProvider()
	{
		_luxometer = Luxometer.GetDefault();
		if (_luxometer != null)
		{
			_readingSubject = new Subject<float>();
			_luxometer.ReadingChanged += OnReadingChanged;
		}
	}

	public float GetCurrentReading()
	{
		if (_luxometer == null)
		{
			return 0f;
		}

		return _luxometer.CurrentReading.IlluminanceInLux;
	}

	public IObservable<float> GetAndObserveCurrentReading()
	{
		if (_luxometer == null)
		{
			return Observable.Empty<float>();
		}

		return _readingSubject.AsObservable();
	}

	private void OnReadingChanged(Luxometer sender, LuxometerReadingChangedEventArgs args)
	{
		_readingSubject?.OnNext(args.Reading.IlluminanceInLux);
	}
}
