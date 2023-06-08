using System.Reactive.Concurrency;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using ApplicationTemplate.Presentation;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using ReviewService.Abstractions;

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
			.AddSingleton<IBackgroundScheduler>(s => TaskPoolScheduler.Default.ToBackgroundScheduler())
			.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>()
			.AddSingleton<IReviewSettingsSource<ReviewSettings>, ReviewSettingsSource>()
			.AddSingleton<IPostService, PostService>()
			.AddSingleton<IDadJokesService, DadJokesService>()
			.AddSingleton<IAuthenticationService, AuthenticationService>()
			.AddSingleton<IUserProfileService, UserProfileService>()
			.AddSingleton<DiagnosticsCountersService>();
	}
}
