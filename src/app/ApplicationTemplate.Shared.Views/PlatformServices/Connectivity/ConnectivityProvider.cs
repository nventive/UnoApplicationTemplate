// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Connectivity/ConnectivityProvider.cs
using System;
using Microsoft.Extensions.Logging;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides access to the current connectivity state and detects changes.
/// </summary>
public class ConnectivityProvider : IConnectivityProvider
{
	private readonly ILogger<ConnectivityProvider> _logger;

	public ConnectivityProvider(ILogger<ConnectivityProvider> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
	}

	/// <inheritdoc/>
	public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

	/// <inheritdoc/>
	public ConnectivityState State => GetCurrentConnectivityState();

	private void OnNetworkStatusChanged(object sender)
	{
		var newState = GetCurrentConnectivityState();
		_logger.LogDebug("Network status changed to: {ConnectivityState}", newState);

		ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs(newState));
	}

	private ConnectivityState GetCurrentConnectivityState()
	{
		try
		{
			var profile = NetworkInformation.GetInternetConnectionProfile();

			if (profile == null)
			{
				return ConnectivityState.None;
			}

			var level = profile.GetNetworkConnectivityLevel();

			return level switch
			{
				NetworkConnectivityLevel.None => ConnectivityState.None,
				NetworkConnectivityLevel.LocalAccess => ConnectivityState.Local,
				NetworkConnectivityLevel.ConstrainedInternetAccess => ConnectivityState.ConstrainedInternet,
				NetworkConnectivityLevel.InternetAccess => ConnectivityState.Internet,
				_ => ConnectivityState.Unknown
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to get connectivity state");
			return ConnectivityState.Unknown;
		}
	}
}
