using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DataLoader;
using MallardMessageHandlers;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace ApplicationTemplate;

/// <summary>
/// A <see cref="IDataLoaderTrigger"/> that will request a load
/// when the <see cref="IDataLoader"/> is in a no network state
/// and that the network connectivity is regained.
/// </summary>
public sealed class NetworkReconnectionDataLoaderTrigger : DataLoaderTriggerBase
{
	private readonly IDataLoader _dataLoader;
	private readonly IConnectivity _connectivity;

	public NetworkReconnectionDataLoaderTrigger(IDataLoader dataLoader, IConnectivity connectivity)
		: base("NetworkReconnection")
	{
		_dataLoader = dataLoader ?? throw new ArgumentNullException(nameof(dataLoader));
		_connectivity = connectivity;
		// TODO Fix Xamarin.Essentials for Windows
		//connectivity.ConnectivityChanged += OnConnectivityChanged;
	}

	private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		// Should only refresh when loader is in NoNetwork AND network is now active
		if (_dataLoader.State.Error is NoNetworkException &&
			e.NetworkAccess == NetworkAccess.Internet)
		{
			RaiseLoadRequested();
		}
	}

	public override void Dispose()
	{
		base.Dispose();

		_connectivity.ConnectivityChanged -= OnConnectivityChanged;
	}
}

public static class NetworkReconnectionDataLoaderExtensions
{
	public static TBuilder TriggerOnNetworkReconnection<TBuilder>(this TBuilder dataLoaderBuilder, IConnectivity connectivity)
		where TBuilder : IDataLoaderBuilder
		=> (TBuilder)dataLoaderBuilder.WithTrigger(d => new NetworkReconnectionDataLoaderTrigger(d, connectivity));
}
