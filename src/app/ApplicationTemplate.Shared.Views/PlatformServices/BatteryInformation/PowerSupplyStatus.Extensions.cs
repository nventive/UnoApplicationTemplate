using System;

namespace Windows.System.Power;

internal static class PowerSupplyStatusExtensions
{
	/// <summary>
	/// Converts the <see cref="PowerSupplyStatus"/> to <see cref="ApplicationTemplate.DataAccess.PlatformServices.PowerSupplyStatus"/>.
	/// </summary>
	/// <param name="status">The power supply status.</param>
	/// <returns>The mapped <see cref="ApplicationTemplate.DataAccess.PlatformServices.PowerSupplyStatus"/>.</returns>
	public static ApplicationTemplate.DataAccess.PlatformServices.PowerSupplyStatus ToInternalPowerSupplyStatus(this PowerSupplyStatus status)
	{
		return status switch
		{
			PowerSupplyStatus.NotPresent => ApplicationTemplate.DataAccess.PlatformServices.PowerSupplyStatus.NotPresent,
			PowerSupplyStatus.Inadequate => ApplicationTemplate.DataAccess.PlatformServices.PowerSupplyStatus.Inadequate,
			PowerSupplyStatus.Adequate => ApplicationTemplate.DataAccess.PlatformServices.PowerSupplyStatus.Adequate,
			_ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown power supply status."),
		};
	}
}
