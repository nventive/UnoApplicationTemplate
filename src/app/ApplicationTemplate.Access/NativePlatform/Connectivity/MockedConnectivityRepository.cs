using System;

namespace ApplicationTemplate.DataAccess;

public sealed class MockedConnectivityRepository : IConnectivityRepository
{
	private ConnectivityState _state;

	public MockedConnectivityRepository()
	{
		State = ConnectivityState.Internet;
	}

	public MockedConnectivityRepository(ConnectivityState state)
	{
		State = state;
	}

	public ConnectivityState State
	{
		get => _state;
		set
		{
			_state = value;
			ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs(value));
		}
	}

	public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;
}
