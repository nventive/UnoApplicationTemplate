using System;

namespace Windows.System.Power;

internal static class EnergySaverStatusExtensions
{
	/// <summary>
	/// Converts the <see cref="EnergySaverStatus"/> to <see cref="ApplicationTemplate.DataAccess.PlatformServices.EnergySaverStatus"/>.
	/// </summary>
	/// <param name="status">The energy saver status.</param>
	/// <returns>The mapped <see cref="ApplicationTemplate.DataAccess.PlatformServices.EnergySaverStatus"/>.</returns>
	public static ApplicationTemplate.DataAccess.PlatformServices.EnergySaverStatus ToInternalEnergySaverStatus(this EnergySaverStatus status)
	{
		return status switch
		{
			EnergySaverStatus.Disabled => ApplicationTemplate.DataAccess.PlatformServices.EnergySaverStatus.Disabled,
			EnergySaverStatus.Off => ApplicationTemplate.DataAccess.PlatformServices.EnergySaverStatus.Off,
			EnergySaverStatus.On => ApplicationTemplate.DataAccess.PlatformServices.EnergySaverStatus.On,
			_ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown energy saver status."),
		};
	}
}
