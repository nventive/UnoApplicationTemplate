using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegistrationServiceCollectionExtensions
{
	/// <summary>
	/// Registers <typeparamref name="T"/> as an option bound to the <paramref name="configuration"/>
	/// using the typename as key (minus the -Options prefix).
	/// The validation, based on Data Annotations, happens when options are retrieved from DI,
	/// not at the time of registration.
	/// </summary>
	/// <typeparam name="T">The type of options to register.</typeparam>
	/// <param name="services">The <see cref="IServiceCollection"/>.</param>
	/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
	/// <param name="key">
	/// The configuration section key name to use.
	/// If not provided, it will be the <typeparamref name="T"/> type name without the -Options prefix.
	/// (see <see cref="ConfigurationExtensions.DefaultOptionsName(Type)"/>.
	/// </param>
	/// <returns>The <see cref="IServiceCollection"/> with services registered.</returns>
	public static IServiceCollection BindOptionsToConfiguration<T>(
		this IServiceCollection services,
		IConfiguration configuration,
		string key = null)
		where T : class
	{
		services
			.AddOptions<T>()
			.Bind(configuration.GetSectionForOptions<T>(key));

		return services;
	}
}
