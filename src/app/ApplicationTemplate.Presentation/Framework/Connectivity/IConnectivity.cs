using System;

namespace ApplicationTemplate;

/// <summary>
/// Checks connectivity state and detect changes.
/// </summary>
public interface IConnectivity
{
	event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

	NetworkAccess NetworkAccess { get; }
}
