using System;
using System.Reactive.Linq;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The fake implementation of <see cref="IAccelerometerService"/> for testing purposes.
/// </summary>
public sealed class FakeAccelerometerService : IAccelerometerService
{
	/// <inheritdoc/>
	public uint ReportInterval { get; set; }

	/// <inheritdoc/>
	public IObservable<AccelerometerReading> ObserveAcceleration()
	{
		return Observable.Return(new AccelerometerReading(0d, 0d, 0d, 0, DateTimeOffset.Now));
	}

	/// <inheritdoc/>
	public IObservable<DateTimeOffset> ObserveDeviceShaken()
	{
		return Observable.Return(DateTimeOffset.Now);
	}
}
