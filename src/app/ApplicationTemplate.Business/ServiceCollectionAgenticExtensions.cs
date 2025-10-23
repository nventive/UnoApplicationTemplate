using ApplicationTemplate.Business.Agentic;
using ApplicationTemplate.DataAccess.ApiClients.Agentic;
using ApplicationTemplate.DataAccess.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Business;

/// <summary>
/// Extension methods for registering Agentic services.
/// </summary>
public static class ServiceCollectionAgenticExtensions
{
	/// <summary>
	/// Adds Agentic services to the service collection.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <param name="configuration">The configuration.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddAgentic(this IServiceCollection services, IConfiguration configuration)
	{
		// Register configuration
		services.Configure<AgenticConfiguration>(configuration.GetSection("Agentic"));

		// Register services - note: AgenticToolExecutor doesn't depend on navigation
		// Navigation functions are registered separately at the Presentation layer
		services.AddSingleton<IAgenticToolExecutor, AgenticToolExecutor>();
		services.AddSingleton<IAgenticChatService, AgenticChatService>();

		// Register API client with HttpClient
		services.AddHttpClient<IAgenticApiClient, AgenticApiClient>();

		return services;
	}
}
