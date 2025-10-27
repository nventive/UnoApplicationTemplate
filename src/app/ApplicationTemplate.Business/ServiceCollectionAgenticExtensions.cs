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

		// Register services
		// Tool executor allows dynamic registration of function handlers AND tool definitions from the app
		services.AddSingleton<AgenticToolExecutor>();
		services.AddSingleton<IAgenticToolExecutor>(sp => sp.GetRequiredService<AgenticToolExecutor>());
		services.AddSingleton<IAgenticToolRegistry>(sp => sp.GetRequiredService<AgenticToolExecutor>());
		
		services.AddSingleton<IAgenticChatService, AgenticChatService>();

		// Register API client with HttpClient
		services.AddHttpClient<IAgenticApiClient, AgenticApiClient>();

		return services;
	}
}
