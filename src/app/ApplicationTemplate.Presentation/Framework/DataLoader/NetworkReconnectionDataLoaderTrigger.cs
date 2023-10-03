using System;
using Chinook.DataLoader;
using MallardMessageHandlers;

namespace ApplicationTemplate;

/// <summary>
/// A <see cref="IDataLoaderTrigger"/> that will request a load
/// when the <see cref="IDataLoader"/> is in a no network state
/// and that the network connectivity is regained.
/// </summary>
public sealed class NetworkReconnectionDataLoaderTrigger : DataLoaderTriggerBase
{
	private readonly IDataLoader _dataLoader;
	private readonly IConnectivityProvider _connectivity;

	public NetworkReconnectionDataLoaderTrigger(IDataLoader dataLoader, IConnectivityProvider connectivity)
		: base("NetworkReconnection")
	{
		_dataLoader = dataLoader ?? throw new ArgumentNullException(nameof(dataLoader));
		_connectivity = connectivity;
		_connectivity.ConnectivityChanged += OnConnectivityChanged;
	}

	/// <remarks>
	/// We should only refresh when <see cref="IDataLoader" /> state is <see cref="NoNetworkException"/> AND network access is <see cref="NetworkAccess.Internet"/>.
	/// </remarks>
	private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
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
	public static TBuilder TriggerOnNetworkReconnection<TBuilder>(this TBuilder dataLoaderBuilder, IConnectivityProvider connectivity)
		where TBuilder : IDataLoaderBuilder
		=> (TBuilder)dataLoaderBuilder.WithTrigger(dataLoader => new NetworkReconnectionDataLoaderTrigger(dataLoader, connectivity));
}
