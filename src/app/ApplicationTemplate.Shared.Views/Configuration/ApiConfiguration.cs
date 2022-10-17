using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MallardMessageHandlers;
using Microsoft.Extensions.Configuration;
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
			new Xamarin.Android.Net.AndroidClientHandler()
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
//-:cnd:noEmit
#if WINDOWS || __ANDROID__ || __IOS__
		// TODO #172362: Not implemented in Uno.
		return Task.FromResult(NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
		//return Task.FromResult(Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet);
#else
		return Task.FromResult(true);
#endif
//+:cnd:noEmit
	}
}
