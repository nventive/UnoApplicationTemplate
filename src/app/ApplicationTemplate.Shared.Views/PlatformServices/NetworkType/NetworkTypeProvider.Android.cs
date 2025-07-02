// src/app/ApplicationTemplate.Shared.Views/PlatformServices/NetworkType/NetworkTypeProvider.Android.cs
#if __ANDROID__
using Android.Content;
using Android.Net;
using AndroidX.Core.Content;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The Android implementation of <see cref="INetworkTypeProvider"/>.
/// </summary>
public sealed class NetworkTypeProvider : INetworkTypeProvider
{
	/// <inheritdoc/>
	public NetworkType NetworkType => GetNetworkType();

	private static NetworkType GetNetworkType()
	{
		var context = Platform.CurrentActivity?.ApplicationContext ?? Android.App.Application.Context;
		var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

		if (connectivityManager == null)
		{
			return NetworkType.None;
		}

		var activeNetwork = connectivityManager.ActiveNetwork;
		if (activeNetwork == null)
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

		if (networkCapabilities.HasTransport(TransportType.Cellular))
		{
			return NetworkType.Cellular;
		}

		if (networkCapabilities.HasTransport(TransportType.Ethernet))
		{
			return NetworkType.Ethernet;
		}

		return NetworkType.Unknown;
	}
}
#endif
