using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// This class is used for native platform configuration.
/// - Configures native platform repositories.
/// </summary>
public static class NativePlatformConfiguration
{
	/// <summary>
	/// Adds the native platform repositories to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">Service collection.</param>
	/// <returns><see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddNativePlaformRepositories(this IServiceCollection services)
	{
		return services
			.AddSingleton<IConnectivityProvider, MockedConnectivityProvider>()
			.AddSingleton<IEmailService, MockedEmailService>();
	}
}
