using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MallardMessageHandlers;
using Microsoft.Extensions.DependencyInjection;
using Windows.Networking.Connectivity;

namespace ApplicationTemplate.Views;

public static class ApiConfiguration
{
	public static IServiceCollection AddApi(this IServiceCollection services)
	{
		return services
			.AddMainHandler()
			.AddSingleton<INetworkAvailabilityChecker>(new NetworkAvailabilityChecker(GetIsNetworkAvailable));
	}

	private static IServiceCollection AddMainHandler(this IServiceCollection services)
	{
		return services.AddTransient<HttpMessageHandler>(s =>
//-:cnd:noEmit
#if __IOS__
//+:cnd:noEmit
			new NSUrlSessionHandler()
//-:cnd:noEmit
#elif __ANDROID__
//+:cnd:noEmit
			new Xamarin.Android.Net.AndroidMessageHandler()
//-:cnd:noEmit
#else
//+:cnd:noEmit
			new HttpClientHandler()
//-:cnd:noEmit
#endif
//+:cnd:noEmit
		);
	}

	private static Task<bool> GetIsNetworkAvailable(CancellationToken ct)
	{
		return Task.FromResult(NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
	}
}
