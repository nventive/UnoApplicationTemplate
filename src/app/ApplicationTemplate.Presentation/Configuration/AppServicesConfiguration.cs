using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationTemplate;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using ApplicationTemplate.Presentation;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace ApplicationTemplate;

/// <summary>
/// This class is used for application services configuration.
/// - Configures business services.
/// - Configures platform services.
/// </summary>
public static class AppServicesConfiguration
{
	/// <summary>
	/// Adds the application services to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">Service collection.</param>
	/// <returns><see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddAppServices(this IServiceCollection services)
	{
		return services
			.AddXamarinEssentials()
			.AddSingleton<IMessageDialogService, AcceptOrDefaultMessageDialogService>()
			.AddSingleton<IBackgroundScheduler>(s => TaskPoolScheduler.Default.ToBackgroundScheduler())
			.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>()
			.AddSingleton<IPostService, PostService>()
			.AddSingleton<IDadJokesService, DadJokesService>()
			.AddSingleton<IAuthenticationService, AuthenticationService>()
			.AddSingleton<IUserProfileService, UserProfileService>()
			.AddSingleton<DiagnosticsCountersService>();
	}

	private static IServiceCollection AddXamarinEssentials(this IServiceCollection services)
	{
		return services
			.AddSingleton<IDeviceInfo, DeviceInfoImplementation>()
			.AddSingleton<IAppInfo, AppInfoImplementation>()
			.AddSingleton<IConnectivity, ConnectivityImplementation>()
			.AddSingleton<IEmail, EmailImplementation>();
	}
}
