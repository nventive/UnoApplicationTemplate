using System.Net.Http;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using MallardMessageHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Views;

public static class ApiConfiguration
{
	public static IServiceCollection AddApi(this IServiceCollection services)
	{
		return services
			.AddMainHandler()
			.AddSingleton<INetworkAvailabilityChecker>(serviceProvider =>
			{
				var connectivityProvider = serviceProvider.GetRequiredService<IConnectivityProvider>();
				return new NetworkAvailabilityChecker(_ => Task.FromResult(connectivityProvider.State is ConnectivityState.Internet));
			});
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
}
