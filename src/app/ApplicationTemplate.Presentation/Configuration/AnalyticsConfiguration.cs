using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Presentation;

public static class AnalyticsConfiguration
{
	/// <summary>
	/// Adds the analytics services to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">The service collection.</param>
	public static IServiceCollection AddAnalytics(this IServiceCollection services)
	{
		return services.AddSingleton<IAnalyticsSink, AnalyticsSink>();
	}
}
