using System.Reactive.Concurrency;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess;
using ApplicationTemplate.Presentation;
using ApplicationTemplate.Presentation.Framework.Connectivity;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;

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
			.AddSingleton<IMessageDialogService, AcceptOrDefaultMessageDialogService>()
			.AddSingleton<IConnectivityProvider, MockedConnectivityProvider>()
			.AddSingleton<IBackgroundScheduler>(s => TaskPoolScheduler.Default.ToBackgroundScheduler())
			.AddSingleton<IApplicationSettingsRepository, ApplicationSettingsRepository>()
			.AddSingleton<IPostService, PostService>()
			.AddSingleton<IDadJokesService, DadJokesService>()
			.AddSingleton<IAuthenticationService, AuthenticationService>()
			.AddSingleton<IUserProfileService, UserProfileService>()
			.AddSingleton<IUpdateRequiredService, UpdateRequiredService>()
			.AddSingleton<IKillSwitchService, KillSwitchService>()
			.AddSingleton<DiagnosticsCountersService>();
	}
}
