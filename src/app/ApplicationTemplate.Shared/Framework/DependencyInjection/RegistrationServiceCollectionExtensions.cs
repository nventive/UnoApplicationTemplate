using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class RegistrationServiceCollectionExtensions
	{
		/// <summary>
		/// Registers all services in <paramref name="assembly"/> that are marked with <see cref="RegisterServiceAttribute"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		/// <param name="assembly">The <see cref="Assembly"/> to scan.</param>
		/// <returns>The <see cref="IServiceCollection"/> with services registered.</returns>
		public static IServiceCollection AutoRegisterServicesFromAssembly(this IServiceCollection services, Assembly assembly = null)
		{
			assembly = assembly ?? Assembly.GetCallingAssembly();
			services.Scan(scan =>
			{
				scan
					.FromAssemblies(assembly)
					.AddClasses(classes => classes
						.WithAttribute<RegisterServiceAttribute>(a =>
							a.Modes.HasFlag(RegistrationModes.Interface) &&
							a.Lifetime == ServiceLifetime.Singleton))
					.AsImplementedInterfaces()
					.WithSingletonLifetime();

				scan
					.FromAssemblies(assembly)
					.AddClasses(classes => classes
						.WithAttribute<RegisterServiceAttribute>(a =>
							a.Modes.HasFlag(RegistrationModes.ConcreteClass) &&
							a.Lifetime == ServiceLifetime.Singleton))
					.AsSelf()
					.WithSingletonLifetime();

				scan
					.FromAssemblies(assembly)
					.AddClasses(classes => classes
						.WithAttribute<RegisterServiceAttribute>(a =>
							a.Modes.HasFlag(RegistrationModes.Interface) &&
							a.Lifetime == ServiceLifetime.Scoped))
					.AsImplementedInterfaces()
					.WithScopedLifetime();

				scan
					.FromAssemblies(assembly)
					.AddClasses(classes => classes
						.WithAttribute<RegisterServiceAttribute>(a =>
							a.Modes.HasFlag(RegistrationModes.ConcreteClass) &&
							a.Lifetime == ServiceLifetime.Scoped))
					.AsSelf()
					.WithScopedLifetime();

				scan
					.FromAssemblies(assembly)
					.AddClasses(classes => classes
						.WithAttribute<RegisterServiceAttribute>(a =>
							a.Modes.HasFlag(RegistrationModes.Interface) &&
							a.Lifetime == ServiceLifetime.Transient))
					.AsImplementedInterfaces()
					.WithTransientLifetime();

				scan
					.FromAssemblies(assembly)
					.AddClasses(classes => classes
						.WithAttribute<RegisterServiceAttribute>(a =>
							a.Modes.HasFlag(RegistrationModes.ConcreteClass) &&
							a.Lifetime == ServiceLifetime.Transient))
					.AsSelf()
					.WithTransientLifetime();
			});

			return services;
		}

		/// <summary>
		/// Registers all services in the assembly containing <typeparamref name="T"/> that are marked with
		/// <see cref="RegisterSingletonServiceAttribute"/>, <see cref="[RegisterService(ServiceLifetime.Scoped)]Attribute"/> or
		/// <see cref="RegisterTransientServiceAttribute"/>.
		/// </summary>
		/// <typeparam name="T">The type contained in the assembly to scan.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		/// <returns>The <see cref="IServiceCollection"/> with services registered.</returns>
		public static IServiceCollection AutoRegisterServicesFromAssemblyContaining<T>(this IServiceCollection services)
			=> services.AutoRegisterServicesFromAssembly(typeof(T).Assembly);

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
}
