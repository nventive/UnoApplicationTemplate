using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Represents an accelerometer reading.
/// </summary>
public sealed class AccelerometerReading
{
	public AccelerometerReading(double accelerationX, double accelerationY, double accelerationZ, TimeSpan? performanceCount, DateTimeOffset timestamp)
	{
		AccelerationX = accelerationX;
		AccelerationY = accelerationY;
		AccelerationZ = accelerationZ;
		PerformanceCount = performanceCount;
		Timestamp = timestamp;
	}

	/// <summary>
	/// Gets the g-force acceleration along the x-axis.
	/// </summary>
	public double AccelerationX { get; }

	/// <summary>
	/// Gets the g-force acceleration along the y-axis.
	/// </summary>
	public double AccelerationY { get; }

	/// <summary>
	/// Gets the g-force acceleration along the z-axis.
	/// </summary>
	public double AccelerationZ { get; }

	/// <summary>
	/// Gets the performance count associated with the reading.
	/// This allows the reading to be synchronized with other devices and processes on the system.
	/// </summary>
	public TimeSpan? PerformanceCount { get; }

	/// <summary>
	/// Gets the time at which the sensor reported the reading.
	/// </summary>
	public DateTimeOffset Timestamp { get; }
}
