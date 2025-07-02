// src/app/ApplicationTemplate.Shared.Views/PlatformServices/NetworkType/NetworkTypeProvider.Android.cs
#if __ANDROID__
using Android.Net;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// 
/// The Android implementation of .
/// 
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	/// 
	public NetworkType NetworkType => GetNetworkType();

	private static NetworkType GetNetworkType()
	{
		var context = Android.App.Application.Context;
		var cm = (ConnectivityManager)context.GetSystemService(Android.Content.Context.ConnectivityService);
		var activeNetwork = cm?.ActiveNetwork;
		if (activeNetwork == null)
		{
			return NetworkType.None;
		}
		var capabilities = cm.GetNetworkCapabilities(activeNetwork);
		if (capabilities == null)
		{
			return NetworkType.None;
		}
		if (capabilities.HasTransport(Transport.Wifi))
		{
			return NetworkType.Wifi;
		}
		if (capabilities.HasTransport(Transport.Ethernet))
		{
			return NetworkType.Ethernet;
		}
		if (capabilities.HasTransport(Transport.Cellular))
		{
			return NetworkType.Cellular;
		}
		return NetworkType.Unknown;
	}
}
#endif
