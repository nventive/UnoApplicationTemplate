using System;

namespace ApplicationTemplate;

/// <summary>
/// Checks connectivity state and detect changes.
/// </summary>
public interface IConnectivityProvider
{
	/// <summary>
	/// Occurs when network status changes.
	/// </summary>
	event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

	/// <summary>
	/// Gets current <see cref="ApplicationTemplate.NetworkAccess"/>.
	/// </summary>
	NetworkAccess NetworkAccess { get; }
}
