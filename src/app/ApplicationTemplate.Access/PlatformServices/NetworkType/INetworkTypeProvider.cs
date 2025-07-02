namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides the current network type.
/// </summary>
public interface INetworkTypeProvider
{
	/// <summary>
	/// Gets the current network type.
	/// </summary>
	NetworkType NetworkType { get; }
}
