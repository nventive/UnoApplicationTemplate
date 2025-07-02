#if __WINDOWS__
using System;
using System.Linq;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The Windows implementation of <see cref="INetworkTypeProvider"/>.
/// </summary>
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	private const uint IANAWIFI = 71;
	private const uint IANAETHERNET = 6;
	private const uint IANAMOBILE = 243;
	private const uint IANAMOBILE2 = 244;

	/// <inheritdoc/>
	public NetworkType NetworkType => GetNetworkType();

	private static NetworkType GetNetworkType()
	{
		var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

		if (connectionProfile is null || connectionProfile.NetworkAdapter is null
		)
		{
			return NetworkType.None;
		}

		// "Iana" (uint) interface allow us to identify the protocol.
		var interfaceType = connectionProfile.NetworkAdapter.IanaInterfaceType;

		if (interfaceType == IANAWIFI)
		{
			return NetworkType.Wifi;
		}

		if (interfaceType == IANAETHERNET)
		{
			return NetworkType.Ethernet;
		}

		// If IANA interface corresponds to mobile and that WwanConnectionProfileDetails is not null.
		// We dig deeper in order to determine the mobile connexion type.
		return (interfaceType == IANAMOBILE || interfaceType == IANAMOBILE2) && connectionProfile.WwanConnectionProfileDetails is not null
			? GetMobileNetworkType(connectionProfile)
			: NetworkType.None;
	}

	private static NetworkType GetMobileNetworkType(ConnectionProfile cp)
	{
		// Get the actual WwanDataClass.
		var wwanProfiles = cp.WwanConnectionProfileDetails.GetCurrentDataClass();

		if (wwanProfiles.Equals(WwanDataClass.None))
		{
			return NetworkType.None;
		}

		var wwanList = Enum
			.GetValues<WwanDataClass>()
			.Cast<WwanDataClass>()
			.ToList();

		// We exclude the None in order to have an accurate research.
		wwanList.Remove(WwanDataClass.None);

		WwanDataClass? wwanClass = null;
		foreach (var wwan in wwanList)
		{
			if (wwanProfiles.HasFlag(wwan))
			{
				// We pick the first match.
				wwanClass = wwan;
				break;
			}
		}

		return GetMobileNetworkType(wwanClass);
	}

	private static NetworkType GetMobileNetworkType(WwanDataClass? wwanClass)
	{
		return wwanClass switch
		{
			WwanDataClass.Gprs
				or WwanDataClass.Cdma1xEvdo
				or WwanDataClass.Cdma1xEvdoRevA
				or WwanDataClass.Cdma1xEvdv
				or WwanDataClass.Cdma1xEvdoRevB
				or WwanDataClass.Cdma3xRtt
				or WwanDataClass.Cdma1xRtt
				or WwanDataClass.Umts
				or WwanDataClass.Hsupa
				or WwanDataClass.Hsdpa
				or WwanDataClass.LteAdvanced
				or WwanDataClass.CdmaUmb
					=> NetworkType.Cellular,

			WwanDataClass.Edge
				or WwanDataClass.Custom
					=> NetworkType.Unknown,

			_ => NetworkType.None,
		};
	}
}
#endif
