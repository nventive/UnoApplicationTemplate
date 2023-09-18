using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Presentation.Framework.Connectivity;

public sealed class MockedConnectivityProvider : IConnectivityProvider
{
	private NetworkAccess _networkAccess;

	public MockedConnectivityProvider()
	{
		NetworkAccess = NetworkAccess.Internet;
	}

	public MockedConnectivityProvider(NetworkAccess networkAccess)
	{
		NetworkAccess = networkAccess;
	}

	public NetworkAccess NetworkAccess
	{
		get => _networkAccess;
		set
		{
			_networkAccess = value;
			ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs(value));
		}
	}
	
	public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;
}
