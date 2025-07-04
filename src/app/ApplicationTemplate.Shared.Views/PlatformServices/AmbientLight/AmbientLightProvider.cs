using System;
using System.Reactive.Linq;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The <see cref="IAmbientLightProvider"/> implementation.
/// </summary>
public sealed class AmbientLightProvider : IAmbientLightProvider
{
	private readonly LightSensor _lightSensor;

	public AmbientLightProvider()
	{
		_lightSensor = LightSensor.GetDefault();
	}

	/// <inheritdoc />
	public float GetCurrentReading()
	{
		return _lightSensor is null ? -1f : _lightSensor.GetCurrentReading().IlluminanceInLux;
	}

	/// <inheritdoc />
	public IObservable<float> GetAndObserveCurrentReading()
	{
		if (_lightSensor is null)
		{
			return Observable.Return(-1f);
		}

		return Observable.FromEventPattern<TypedEventHandler<LightSensor, LightSensorReadingChangedEventArgs>, LightSensorReadingChangedEventArgs>(
			h => _lightSensor.ReadingChanged += h,
			h => _lightSensor.ReadingChanged -= h
		)
		.Select(eventPattern => eventPattern.EventArgs.Reading.IlluminanceInLux)
		.StartWith(GetCurrentReading());
	}
}
