// src/app/ApplicationTemplate.Shared.Views/PlatformServices/NetworkType/NetworkTypeProvider.iOS.cs
#if __IOS__
using SystemConfiguration;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// 
/// The iOS implementation of .
/// 
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	/// 
	public NetworkType NetworkType
	{
		get
		{
			var reachability = new SCNetworkReachability(new System.Net.IPAddress(0));
			SCNetworkReachabilityFlags flags;
			if (!reachability.TryGetFlags(out flags))
			{
				return NetworkType.None;
			}
			if (!flags.HasFlag(SCNetworkReachabilityFlags.Reachable))
			{
				return NetworkType.None;
			}
			var isWWAN = flags.HasFlag(SCNetworkReachabilityFlags.IsWWAN);
			if (isWWAN)
			{
				return NetworkType.Cellular;
			}
			// Assuming WiFi if not WWAN and reachable, as Ethernet is rare on iOS.
			return NetworkType.Wifi;
		}
	}
}
#endif
