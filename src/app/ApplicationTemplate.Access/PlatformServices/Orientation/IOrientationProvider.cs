using System;

namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// Provides the current device orientation.
/// </summary>
public interface IOrientationProvider
{
	/// <summary>
	/// Gets whether the device is in landscape orientation.
	/// </summary>
	/// <returns>Whether the device is in landscape orientation.</returns>
	bool GetIsLandscape();

	/// <summary>
	/// Gets and observe the device orientation.
	/// </summary>
	/// <returns>An observable sequence yielding whether the device is in landscape.</returns>
	IObservable<bool> GetAndObserveIsLandscape();
}
