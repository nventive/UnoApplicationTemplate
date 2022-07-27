using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApplicationTemplate.Client;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;

namespace ApplicationTemplate
{
	[JsonSerializable(typeof(AuthenticationToken))]
	[JsonSerializable(typeof(AuthenticationData))]
	[JsonSerializable(typeof(ApplicationSettings))]
	[JsonSerializable(typeof(Refit.ProblemDetails))]
	[JsonSerializable(typeof(PostData))]
	[JsonSerializable(typeof(PostData[]))]
	[JsonSerializable(typeof(DadJokesResponse))]
	[JsonSerializable(typeof(DadJokesErrorResponse))]
	[JsonSerializable(typeof(UserProfileData))]
	[JsonSerializable(typeof(DadJokesData))]
	[JsonSerializable(typeof(DadJokeChildData))]
	[JsonSerializable(typeof(DadJokeContentData))]
	[JsonSerializable(typeof(Dictionary<string, string>))]
	public partial class JsonContext : JsonSerializerContext
	{
	}

	/// <summary>
	/// This class is used for serialization configuration.
	/// - Configures the serializers.
	/// </summary>
	public static class SerializationConfiguration
	{
		public static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = GetDefaultOptions();

		private static JsonSerializerOptions GetDefaultOptions()
		{
			// These options allow some more cases than just the default.
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				NumberHandling = JsonNumberHandling.AllowReadingFromString,
				PropertyNameCaseInsensitive = true,
			};
			options.AddContext<JsonContext>();

			return options;
		}

		/// <summary>
		/// Adds the serialization services to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">Service collection.</param>
		/// <returns><see cref="IServiceCollection"/>.</returns>
		public static IServiceCollection AddSerialization(this IServiceCollection services)
		{
			services
				.AddSingleton(DefaultJsonSerializerOptions)
				.AddSingleton<ISettingsSerializer>(c => new JsonSerializerToSettingsSerializerAdapter(DefaultJsonSerializerOptions));

			return services;
		}
	}
}
