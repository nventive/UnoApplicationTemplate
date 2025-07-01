using System;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate.DataAccess.PlatformServices;

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
				this.Log().Debug("Subscribed to NetworkInformation.NetworkStatusChanged.");
			}
			InnerConnectivityChanged += value;
		}

		remove
		{
			InnerConnectivityChanged -= value;
			UnsubscribeLocalEvent();
		}
	}

	private event EventHandler<ConnectivityChangedEventArgs> InnerConnectivityChanged;

	public ConnectivityState State
	{
		get
		{
			var networkConnectivityLevel = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel();
			switch (networkConnectivityLevel)
			{
				case NetworkConnectivityLevel.None:
					return ConnectivityState.None;
				case NetworkConnectivityLevel.LocalAccess:
					return ConnectivityState.Local;
				case NetworkConnectivityLevel.ConstrainedInternetAccess:
					return ConnectivityState.ConstrainedInternet;
				case NetworkConnectivityLevel.InternetAccess:
					return ConnectivityState.Internet;
				default:
					this.Log().Error($"Unsupported Network Connectivity Level: {networkConnectivityLevel}");
					return ConnectivityState.Unknown;
			}
		}
	}

	public void Dispose()
	{
		InnerConnectivityChanged = null;
		UnsubscribeLocalEvent();
		GC.SuppressFinalize(this);
	}

	private void UnsubscribeLocalEvent()
	{
		if (_subscribed && InnerConnectivityChanged is null)
		{
			_subscribed = false;
			NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
			this.Log().Debug("Unsubscribed to NetworkInformation.NetworkStatusChanged because no subscriptions were left on the ConnectivityProvider.ConnectivityChanged event.");
		}
	}

	private void OnNetworkStatusChanged(object sender)
	{
		InnerConnectivityChanged.Invoke(this, new ConnectivityChangedEventArgs(State));
	}
}
