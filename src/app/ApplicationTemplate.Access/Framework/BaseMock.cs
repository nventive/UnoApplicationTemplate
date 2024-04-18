using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationTemplate;

public class BaseMock
{
	private readonly JsonSerializerOptions _serializerOptions;

	public BaseMock(JsonSerializerOptions serializerOptions)
	{
		_serializerOptions = serializerOptions;
	}

	/// <summary>
	/// Gets the deserialized value of the specified embedded resource.
	/// </summary>
	/// <typeparam name="T">The type of value.</typeparam>
	/// <param name="resourceName">The name of the resource.</param>
	/// <param name="callerMemberName">The caller member name.</param>
	/// <returns>The deserialized value.</returns>
	/// <remarks>
	/// If left empty, the <paramref name="resourceName" /> will implicitly be treated as "{callerTypeName}.{callerMemberName}.json".
	/// Note that this will deserialize the first embedded resource whose name ends with the specified <paramref name="resourceName" />.
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not available for Desktop")]
	protected T GetFromEmbeddedResource<T>(
		string resourceName = null,
		[CallerMemberName] string callerMemberName = null)
	{
		var assembly = GetType().GetTypeInfo().Assembly;

		var desiredResourceName = resourceName != null
			? resourceName.Replace("/", ".")
			: $"{GetType().Name}.{callerMemberName}.json";

		var actualResourceName = assembly
			.GetManifestResourceNames()
			.FirstOrDefault(name => name.EndsWith(desiredResourceName, StringComparison.OrdinalIgnoreCase));

		if (actualResourceName == null)
		{
			throw new FileNotFoundException($"Couldn't find an embedded resource ending with '{desiredResourceName}'.", desiredResourceName);
		}

		using (var stream = assembly.GetManifestResourceStream(actualResourceName))
		{
			return JsonSerializer.Deserialize<T>(stream, _serializerOptions);
		}
	}

	/// <summary>
	/// Creates a task that's completed successfully with the deserialized value of the specified embedded resource.
	/// </summary>
	/// <remarks>
	/// If left empty, the <paramref name="resourceName" /> will implicitly be treated as "{callerTypeName}.{callerMemberName}.json".
	/// Note that this will deserialize the first embedded resource whose name ends with the specified <paramref name="resourceName" />.
	/// </remarks>
	/// <typeparam name="T">The type of object.</typeparam>
	/// <param name="resourceName">The name of the resource.</param>
	/// <param name="callerMemberName">The name of the caller (used if no resource name provided).</param>
	/// <returns>The deserialized object.</returns>
	protected Task<T> GetTaskFromEmbeddedResource<T>(
		string resourceName = null,
		[CallerMemberName] string callerMemberName = null
	) => Task.FromResult(GetFromEmbeddedResource<T>(resourceName, callerMemberName));
}
