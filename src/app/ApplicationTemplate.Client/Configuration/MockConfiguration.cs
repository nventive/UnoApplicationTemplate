using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate;

/// <summary>
/// This class configures data mocking.
/// </summary>
public static class MockConfiguration
{
	/// <summary>
	/// Adds everything related to data mocking.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <param name="configuration">The configuration.</param>
	public static IServiceCollection AddMock(this IServiceCollection services, IConfiguration configuration)
	{
		return services.BindOptionsToConfiguration<MockOptions>(configuration);
	}
}
