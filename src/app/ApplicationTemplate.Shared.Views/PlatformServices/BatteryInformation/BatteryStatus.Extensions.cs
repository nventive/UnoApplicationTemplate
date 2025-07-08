using System;

namespace Windows.System.Power;

internal static class BatteryStatusExtensions
{
	/// <summary>
	/// Converts the <see cref="BatteryStatus"/> to <see cref="ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus"/>.
	/// </summary>
	/// <param name="status">The battery status.</param>
	/// <returns>The mapped <see cref="ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus"/>.</returns>
	public static ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus ToInternalBatteryStatus(this BatteryStatus status)
	{
		return status switch
		{
			BatteryStatus.NotPresent => ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus.NotPresent,
			BatteryStatus.Discharging => ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus.Discharging,
			BatteryStatus.Idle => ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus.Idle,
			BatteryStatus.Charging => ApplicationTemplate.DataAccess.PlatformServices.BatteryStatus.Charging,
			_ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown battery status."),
		};
	}
}
