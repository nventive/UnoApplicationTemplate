using System;
using System.Reactive.Concurrency;
using System.Threading;
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

	public NetworkAccess NetworkAccess
	{
		get
		{
#if  WINDOWS10_0_18362_0_OR_GREATER
			// This is null if we access it from the UI thread.
			var networkConnectivityLevel = DefaultScheduler.Instance.Run(
				func: () => NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel(),
				cancellationToken: CancellationToken.None
			).Result;
#else
			var networkConnectivityLevel = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel();
#endif
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
		InnerConnectivityChanged.Invoke(this, new ConnectivityChangedEventArgs(NetworkAccess));
	}
}
