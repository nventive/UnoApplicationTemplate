using System;

namespace ApplicationTemplate;

public sealed class ConnectivityChangedEventArgs : EventArgs
{
	public ConnectivityChangedEventArgs(NetworkAccess networkAccess)
	{
		NetworkAccess = networkAccess;
	}

	public NetworkAccess NetworkAccess { get; }
}
