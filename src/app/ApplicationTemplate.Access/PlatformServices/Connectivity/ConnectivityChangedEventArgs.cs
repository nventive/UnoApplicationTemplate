using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class ConnectivityChangedEventArgs : EventArgs
{
	public ConnectivityChangedEventArgs(ConnectivityState state)
	{
		State = state;
	}

	public ConnectivityState State { get; }
}
