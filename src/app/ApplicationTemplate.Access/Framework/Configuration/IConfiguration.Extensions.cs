﻿using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// This class exposes extension methods on <see cref="IConfiguration"/>.
/// </summary>
/// <remarks>
/// Note that the class name starts with "ApplicationTemplate" to avoid potential collisions since we use the original namespace of <see cref="IConfigurationSection"/>.
/// </remarks>
public static class ApplicationTemplateConfigurationExtensions
{
	/// <summary>
	/// Returns the <see cref="IConfigurationSection"/> for the options represented by <typeparamref name="T"/>.
	/// The section name is either the options type name (minus the -Options suffix) or <paramref name="key"/>, if provided.
	/// <see cref="DefaultOptionsName{T}"/> as well.
	/// </summary>
	/// <typeparam name="T">The options type.</typeparam>
	/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
	/// <param name="key">The section name key, if any.</param>
	/// <returns>The <see cref="IConfigurationSection"/>.</returns>
	public static IConfigurationSection GetSectionForOptions<T>(
		this IConfiguration configuration,
		string key = null)
		=> configuration.GetSection(key ?? DefaultOptionsName<T>());

	/// <summary>
	/// Reads the current value for options.
	/// The section name is either the options type name (minus the -Options suffix) or <paramref name="key"/>, if provided.
	/// <see cref="DefaultOptionsName{T}"/> as well.
	/// </summary>
	/// <typeparam name="T">The options type.</typeparam>
	/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
	/// <param name="key">The section name key, if any.</param>
	/// <returns>The current value for <typeparamref name="T"/>.</returns>
	public static T ReadOptions<T>(this IConfiguration configuration, string key = null)
		where T : new()
	{
		var options = configuration.GetSection(key ?? DefaultOptionsName<T>()).Get<T>();
		return options != null ? options : new T();
	}

	/// <summary>
	/// Gets the default name for options of type <paramref name="optionsType"/>.
	/// Removes the -Options suffix if any.
	/// </summary>
	/// <param name="optionsType">The type from which to extract a default options name.</param>
	public static string DefaultOptionsName(Type optionsType) => DefaultOptionsName(optionsType.Name);

	/// <summary>
	/// Gets the default name for options of the specified <paramref name="optionsTypeName"/>.
	/// Removes the -Options suffix if any.
	/// </summary>
	/// <param name="optionsTypeName">The name of the type from which to extract a default options name.</param>
	public static string DefaultOptionsName(string optionsTypeName) => Regex.Replace(optionsTypeName, @"Options$", string.Empty);

	/// <summary>
	/// Gets the default name for options of type <typeparamref name="T"/>.
	/// Removes the -Options suffix if any.
	/// </summary>
	/// <typeparam name="T">The Options type.</typeparam>
	public static string DefaultOptionsName<T>() => DefaultOptionsName(typeof(T));
}
