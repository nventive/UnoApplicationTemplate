// src/app/ApplicationTemplate.Shared.Views/PlatformServices/NetworkType/NetworkTypeProvider.iOS.cs
#if __IOS__
using Network;
using SystemConfiguration;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The iOS implementation of <see cref="INetworkTypeProvider"/>.
/// </summary>
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	/// <inheritdoc/>
	public NetworkType NetworkType => GetNetworkType();

	private static NetworkType GetNetworkType()
	{
		var reachability = new NetworkReachability("www.google.com");

		if (!reachability.TryGetFlags(out var flags))
		{
			return NetworkType.None;
		}

		if (!flags.HasFlag(NetworkReachabilityFlags.Reachable))
		{
			return NetworkType.None;
		}

		if (flags.HasFlag(NetworkReachabilityFlags.IsWWAN))
		{
			return NetworkType.Cellular;
		}

		if (!flags.HasFlag(NetworkReachabilityFlags.ConnectionRequired))
		{
			// Direct connection (likely WiFi or Ethernet)
			return NetworkType.Wifi;
		}

		return NetworkType.Unknown;
	}
}
#endif
