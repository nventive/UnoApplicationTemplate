using System;

namespace ApplicationTemplate.DataAccess;

public sealed class ConnectivityChangedEventArgs : EventArgs
{
	public ConnectivityChangedEventArgs(ConnectivityState state)
	{
		State = state;
	}

	public ConnectivityState State { get; }
}
