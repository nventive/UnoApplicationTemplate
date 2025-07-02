#if __ANDROID__
using System;
using Android.Content;
using Android.Net;
using Uno.Extensions;
using Uno.Logging;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The Android implementation of <see cref="INetworkTypeProvider"/>.
/// </summary>
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	/// <inheritdoc/>
	public NetworkType NetworkType => GetNetworkType();

	private NetworkType GetNetworkType()
	{
		try
		{
			var context = Android.App.Application.Context;
			var connectivityManager = context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;

			var activeNetwork = connectivityManager?.ActiveNetwork;
			if (activeNetwork is null)
			{
				return NetworkType.None;
			}

			var networkCapabilities = connectivityManager.GetNetworkCapabilities(activeNetwork);
			if (networkCapabilities == null)
			{
				return NetworkType.None;
			}

			if (networkCapabilities.HasTransport(TransportType.Wifi))
			{
				return NetworkType.Wifi;
			}
			else if (networkCapabilities.HasTransport(TransportType.Cellular))
			{
				return NetworkType.Cellular;
			}
			else if (networkCapabilities.HasTransport(TransportType.Ethernet))
			{
				return NetworkType.Ethernet;
			}
			else
			{
				return networkCapabilities.HasTransport(TransportType.Vpn) ? NetworkType.Unknown : NetworkType.None;
			}
		}
		catch (Exception e)
		{
			this.Log().Error("Unable to get the Network Type.", e);
			return NetworkType.None;
		}
	}
}
#endif
