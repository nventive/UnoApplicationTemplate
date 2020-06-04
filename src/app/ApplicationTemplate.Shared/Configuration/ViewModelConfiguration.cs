using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApplicationTemplate
{
	public static class ViewModelConfiguration
	{
		/// <summary>
		/// Adds the mvvm services to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">Service collection.</param>
		/// <returns><see cref="IServiceCollection"/>.</returns>
		public static IServiceCollection AddMvvm(this IServiceCollection services)
		{
			return services
				.AddDynamicProperties()
				.AddDynamicCommands()
				.AddDataLoaders()
				.AddValidatorsFromAssemblyContaining(typeof(ViewModelConfiguration), ServiceLifetime.Singleton);
		}

		private static IServiceCollection AddDynamicCommands(this IServiceCollection services)
		{
			return services
				.AddSingleton<IDynamicCommandStrategyDecorator>(s =>
					new DynamicCommandStrategyDecorator(c => c
						.WithLogs(s.GetRequiredService<ILogger<IDynamicCommand>>())
						.CatchErrors(s.GetRequiredService<IDynamicCommandErrorHandler>())
						.CancelPrevious()
						.OnBackgroundThread()
						.DisableWhileExecuting()
					)
				)
				.AddSingleton<IDynamicCommandFactory, DynamicCommandFactory>();
		}

		private static IServiceCollection AddDynamicProperties(this IServiceCollection services)
		{
			return services.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>();
		}

		private static IServiceCollection AddDataLoaders(this IServiceCollection services)
		{
			return services.AddSingleton<IDataLoaderBuilderFactory>(s =>
			{
				return new DataLoaderBuilderFactory(b => b
					.OnBackgroundThread()
					.WithEmptySelector(GetIsEmpty)
					.WithAnalytics(
						onSuccess: async (ct, request, value) => { /* Some analytics */ },
						onError: async (ct, request, error) => { /* Somme analytics */ }
					)
				);

				bool GetIsEmpty(IDataLoaderState state)
				{
					return state.Data == null || (state.Data is IEnumerable enumerable && !enumerable.Cast<object>().Any());
				}
			});
		}
	}
}
