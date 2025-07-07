using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides a way for keeping the device's display on.
/// </summary>
public interface IScreenWakeLockService
{
	/// <summary>
	/// Enables the device's keep screen on feature.
	/// </summary>
	void Enable();

	/// <summary>
	/// Disables the device's keep screen on feature.
	/// </summary>
	/// <remarks>
	/// Throws <see cref="Exception"/> if no request has been made.
	/// </remarks>
	void Disable();
}
