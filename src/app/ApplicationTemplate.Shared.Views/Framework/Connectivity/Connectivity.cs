using System;
using Uno.Extensions;
using Uno.Logging;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate;

public sealed class Connectivity : IConnectivity, IDisposable
{
	public Connectivity()
	{
		NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
	}

	public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

	public NetworkAccess NetworkAccess
	{
		get
		{
			var networkConnectivityLevel = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel();
			switch (networkConnectivityLevel)
			{
				case NetworkConnectivityLevel.None:
					return NetworkAccess.None;
				case NetworkConnectivityLevel.LocalAccess:
					return NetworkAccess.Local;
				case NetworkConnectivityLevel.ConstrainedInternetAccess:
					return NetworkAccess.ConstrainedInternet;
				case NetworkConnectivityLevel.InternetAccess:
					return NetworkAccess.Internet;
				default:
					this.Log().Error($"Unsupported Network Connectivity Level: {networkConnectivityLevel}");
					return NetworkAccess.Unknown;
			}
		}
	}

	public void Dispose()
	{
		NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
		GC.SuppressFinalize(this);
	}

	private void OnNetworkStatusChanged(object sender)
	{
		ConnectivityChanged.Invoke(sender, new ConnectivityChangedEventArgs(NetworkAccess));
	}
}
