// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Connectivity/ConnectivityProvider.cs
using System;
using Uno.Platform.Networking;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	public class ConnectivityProvider : IConnectivityProvider, IDisposable
	{
		private readonly object _lock = new();
		private ConnectivityState _state;

		public ConnectivityProvider()
		{
			NetworkInformation.ConnectionProfilesChanged += OnConnectionProfilesChanged;
			UpdateState();
		}

		public void Dispose()
		{
			NetworkInformation.ConnectionProfilesChanged -= OnConnectionProfilesChanged;
		}

		public event EventHandler ConnectivityChanged;

		public ConnectivityState State => _state;

		private void OnConnectionProfilesChanged(object sender, object e)
		{
			UpdateState();
			ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs(_state));
		}

		private void UpdateState()
		{
			lock (_lock)
			{
				var profiles = NetworkInformation.GetConnectionProfiles();
				if (profiles == null || profiles.Length == 0)
				{
					_state = ConnectivityState.None;
					return;
				}

				var bestLevel = NetworkConnectivityLevel.None;
				foreach (var profile in profiles)
				{
					if (profile.NetworkConnectivityLevel > bestLevel)
					{
						bestLevel = profile.NetworkConnectivityLevel;
					}
				}

				switch (bestLevel)
				{
					case NetworkConnectivityLevel.None:
						_state = ConnectivityState.None;
						break;
					case NetworkConnectivityLevel.LocalAccess:
						_state = ConnectivityState.Local;
						break;
					case NetworkConnectivityLevel.ConstrainedInternetAccess:
						_state = ConnectivityState.ConstrainedInternet;
						break;
					case NetworkConnectivityLevel.InternetAccess:
						_state = ConnectivityState.Internet;
						break;
					default:
						_state = ConnectivityState.Unknown;
						break;
				}
			}
		}
	}
}
