using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate;

public static class LocalizationConfiguration
{
	/// <summary>
	/// Adds the localization services to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">Service collection.</param>
	/// <returns><see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddLocalization(this IServiceCollection services)
	{
		return services
			.AddSingleton<IThreadCultureOverrideService, MockThreadCultureOverrideService>()
			.AddSingleton<IStringLocalizer, MockStringLocalizer>();
	}
}
