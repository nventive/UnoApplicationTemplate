using System;
using System.Reactive.Linq;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The Windows implementation of <see cref="IOrientationProvider"/>.
/// </summary>
public sealed class OrientationProvider : IOrientationProvider
{
	private readonly SimpleOrientationSensor _simpleOrientationSensor;

	public OrientationProvider()
	{
		_simpleOrientationSensor = SimpleOrientationSensor.GetDefault();
	}

	/// <inheritdoc />
	public IObservable<bool> GetAndObserveIsLandscape()
	{
		if (_simpleOrientationSensor is null)
		{
			return Observable.Return(GetIsLandscape());
		}

		return Observable.FromEventPattern<TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>, SimpleOrientationSensorOrientationChangedEventArgs>(
			h => _simpleOrientationSensor.OrientationChanged += h,
			h => _simpleOrientationSensor.OrientationChanged -= h
		)
		.Select(args => GetIsLandscape(args.EventArgs.Orientation))
		.StartWith(GetIsLandscape());
	}

	/// <inheritdoc />
	public bool GetIsLandscape()
	{
		var currentOrientation = _simpleOrientationSensor?.GetCurrentOrientation();

		return GetIsLandscape(currentOrientation);
	}

	/// <summary>
	/// Gets whether the given <see cref="SimpleOrientation"/> is landscape.
	/// </summary>
	/// <param name="simpleOrientation">The simple orientation.</param>
	/// <returns>Whether the given orientation is landscape.</returns>
	private static bool GetIsLandscape(SimpleOrientation? simpleOrientation)
	{
		return simpleOrientation is SimpleOrientation.Rotated90DegreesCounterclockwise or SimpleOrientation.Rotated270DegreesCounterclockwise;
	}
}
