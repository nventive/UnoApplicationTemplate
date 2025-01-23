using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using Uno.Extensions;
using Uno.Logging;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate;

public sealed class ConnectivityRepository : IConnectivityRepository, IDisposable
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
#if __WINDOWS__
			var joinableTaskFactory = new Microsoft.VisualStudio.Threading.JoinableTaskFactory(new Microsoft.VisualStudio.Threading.JoinableTaskContext());
			var networkConnectivityLevel = joinableTaskFactory.Run(static async () =>
			{
				// Needs to run on the UI thread on Windows.
				return await DefaultScheduler.Instance.Run(
					func: () => NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel(),
					cancellationToken: CancellationToken.None
				);
			});
#else
			var networkConnectivityLevel = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel();
#endif
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
