using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides access to the current ambient light reading and detects changes.
/// </summary>
/// <remarks>
/// The device or emulator that you're using must support an ambient light sensor.
/// </remarks>
public interface IAmbientLightProvider
{
	/// <summary>
	/// Gets the current ambient light (illuminance level) reading in lux.
	/// </summary>
	/// <returns>The current ambient light (illuminance level) reading in lux.</returns>
	float GetCurrentReading();

	/// <summary>
	/// Gets and observes the current ambient light (illuminance level) reading in lux.
	/// </summary>
	/// <returns>An observable sequence yielding the current ambient light (illuminance level) reading in lux.</returns>
	IObservable<float> GetAndObserveCurrentReading();
}
