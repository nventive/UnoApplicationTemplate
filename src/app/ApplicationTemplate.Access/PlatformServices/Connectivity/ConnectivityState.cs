namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Various states of the connection to the internet.
/// </summary>
public enum ConnectivityState
{
	/// <summary>
	/// The state of the connectivity is not known.
	/// </summary>
	Unknown,

	/// <summary>
	/// No connectivity.
	/// </summary>
	None,

	/// <summary>
	/// Local network access only.
	/// </summary>
	Local,

	/// <summary>
	/// Limited internet access.
	/// </summary>
	ConstrainedInternet,

	/// <summary>
	/// Local and Internet access.
	/// </summary>
	Internet,
}
