namespace ApplicationTemplate;

/// <summary>
/// Provides information about the device.
/// </summary>
public interface IDeviceInformationProvider
{
	/// <summary>
	/// Gets the device type (Mobile, Tablet, Television, Car, Watch, VirtualReality, Desktop, Unknown).
	/// </summary>
	string DeviceType { get; }

	/// <summary>
	/// Gets the operating system (Android, iOS, Browser).
	/// </summary>
	string OperatingSystem { get; }

	/// <summary>
	/// Gets the operating system version.
	/// </summary>
	string OperatingSystemVersion { get; }
}
