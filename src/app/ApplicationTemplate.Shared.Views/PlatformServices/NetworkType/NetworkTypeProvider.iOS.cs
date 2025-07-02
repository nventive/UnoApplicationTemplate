#if __IOS__
using System;
using SystemConfiguration;
using Uno.Extensions;
using Uno.Logging;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The iOS implementation of <see cref="INetworkTypeProvider"/>.
/// </summary>
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	/// <inheritdoc/>
	public NetworkType NetworkType => GetNetworkType();

	private NetworkType GetNetworkType()
	{
		try
		{
			var reachability = new NetworkReachability("www.google.com");

			if (!reachability.TryGetFlags(out var flags) || !flags.HasFlag(NetworkReachabilityFlags.Reachable))
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
		catch (Exception e)
		{
			this.Log().Error("Unable to get the Network Type.", e);
			return NetworkType.None;
		}
	}
}
#endif
