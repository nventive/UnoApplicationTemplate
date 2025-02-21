using System;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the current connectivity state and detects changes.
/// </summary>
public interface IConnectivityProvider
{
	/// <summary>
	/// Occurs when connectivity state changes.
	/// </summary>
	event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

	/// <summary>
	/// Gets the current connectivity state.
	/// </summary>
	ConnectivityState State { get; }
}
