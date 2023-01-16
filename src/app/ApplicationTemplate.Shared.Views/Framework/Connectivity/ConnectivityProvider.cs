using System;
using Uno.Extensions;
using Uno.Logging;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate;

public sealed class ConnectivityProvider : IConnectivityProvider, IDisposable
{
	private bool _subscribed = false;

	public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged
	{
		add
		{
			if (!_subscribed)
			{
				_subscribed = true;
				NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
			}
			InnerConnectivityChanged += value;
		}

		remove
		{
			UnsubscribeLocalEvent();
			InnerConnectivityChanged -= value;
		}
	}

	private event EventHandler<ConnectivityChangedEventArgs> InnerConnectivityChanged;

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
		UnsubscribeLocalEvent();
		InnerConnectivityChanged = null;
		GC.SuppressFinalize(this);
	}

	private void UnsubscribeLocalEvent()
	{
		if (_subscribed)
		{
			_subscribed = false;
			NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
		}
	}

	private void OnNetworkStatusChanged(object sender)
	{
		InnerConnectivityChanged.Invoke(this, new ConnectivityChangedEventArgs(NetworkAccess));
	}
}
