namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class FakeNetworkTypeProvider : INetworkTypeProvider
{
	public NetworkType NetworkType => NetworkType.Wifi;
}
