using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;

namespace ApplicationTemplate;

// TODO: Add the types you want to use in JSON source-generator-based deserialization here.

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
[JsonSerializable(typeof(IDictionary<string, string>))]
[ExcludeFromCodeCoverage]
public partial class JsonContext : JsonSerializerContext
{
}

/// <summary>
/// This class is used for serialization configuration.
/// - Configures the serializers.
/// </summary>
public static class SerializationConfiguration
{
	public static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = GetOptionsWithSourceGeneration();

	public static JsonSerializerOptions NoSourceGenerationJsonSerializerOptions { get; } = GetBaseOptions();

	private static JsonSerializerOptions GetBaseOptions()
	{
		// These options allow some more cases than just the default.
		var options = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString,
			PropertyNameCaseInsensitive = true,
		};

		return options;
	}

	private static JsonSerializerOptions GetOptionsWithSourceGeneration()
	{
		var options = GetBaseOptions();
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
