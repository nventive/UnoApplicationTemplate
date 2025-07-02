namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Represents the type of a network connection.
/// </summary>
public enum NetworkType
{
	/// <summary>
	/// No network connection.
	/// </summary>
	None,

	/// <summary>
	/// Network connection is available, but the type is not known.
	/// </summary>
	Unknown,

	/// <summary>
	/// Wi-Fi network connection.
	/// </summary>
	Wifi,

	/// <summary>
	/// Cellular network connection.
	/// </summary>
	Cellular,

	/// <summary>
	/// Ethernet network connection.
	/// </summary>
	Ethernet,
}
