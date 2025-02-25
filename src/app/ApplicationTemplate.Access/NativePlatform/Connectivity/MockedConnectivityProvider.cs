using System;

namespace ApplicationTemplate.DataAccess;

public sealed class MockedConnectivityProvider : IConnectivityProvider
{
	private ConnectivityState _state;

	public MockedConnectivityProvider()
	{
		State = ConnectivityState.Internet;
	}

	public MockedConnectivityProvider(ConnectivityState state)
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
