using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Executes function tool calls from the Azure AI agent.
/// Maps agent function calls to application operations (navigation, data access, etc.).
/// The actual implementations are provided by the Presentation layer through delegates.
/// </summary>
public class AgenticToolExecutor : IAgenticToolExecutor
{
	private readonly ILogger<AgenticToolExecutor> _logger;
	private readonly Dictionary<string, Func<JsonDocument, CancellationToken, Task<string>>> _functionHandlers;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticToolExecutor"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public AgenticToolExecutor(ILogger<AgenticToolExecutor> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_functionHandlers = new Dictionary<string, Func<JsonDocument, CancellationToken, Task<string>>>();
	}

	/// <summary>
	/// Registers a function handler that will be called when the AI agent invokes the function.
	/// </summary>
	/// <param name="functionName">The name of the function.</param>
	/// <param name="handler">The handler that implements the function logic.</param>
	public void RegisterFunctionHandler(string functionName, Func<JsonDocument, CancellationToken, Task<string>> handler)
	{
		if (string.IsNullOrEmpty(functionName))
		{
			throw new ArgumentNullException(nameof(functionName));
		}

		if (handler == null)
		{
			throw new ArgumentNullException(nameof(handler));
		}

		_functionHandlers[functionName] = handler;
		_logger.LogInformation("Registered function handler: {FunctionName}", functionName);
	}

	/// <summary>
	/// Execute a function tool call from the agent.
	/// </summary>
	/// <param name="functionName">Name of the function to execute.</param>
	/// <param name="argumentsJson">JSON string of function arguments.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>Result as JSON string.</returns>
	public async Task<string> ExecuteFunctionAsync(string functionName, string argumentsJson, CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("AgenticToolExecutor: Executing function '{FunctionName}' with args: {ArgumentsJson}", functionName, argumentsJson);

			// Check if a handler is registered for this function
			if (_functionHandlers.TryGetValue(functionName, out var handler))
			{
				var args = JsonDocument.Parse(argumentsJson);
				return await handler(args, ct);
			}

			// No handler registered
			_logger.LogWarning("No handler registered for function: {FunctionName}", functionName);
			return JsonSerializer.Serialize(new
			{
				success = false,
				error = $"Unknown function: {functionName}. No handler registered."
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "AgenticToolExecutor: Error executing {FunctionName}: {ErrorMessage}", functionName, ex.Message);
			return JsonSerializer.Serialize(new
			{
				success = false,
				error = $"Error executing {functionName}: {ex.Message}"
			});
		}
	}

	/// <summary>
	/// Gets helper methods for creating common JSON responses.
	/// </summary>
	public static class ResponseHelpers
	{
		/// <summary>
		/// Creates a success response.
		/// </summary>
		public static string Success(string message, object? data = null)
		{
			var response = new Dictionary<string, object>
			{
				["success"] = true,
				["message"] = message,
				["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
			};

			if (data != null)
			{
				foreach (var prop in data.GetType().GetProperties())
				{
					response[prop.Name] = prop.GetValue(data) ?? string.Empty;
				}
			}

			return JsonSerializer.Serialize(response);
		}

		/// <summary>
		/// Creates an error response.
		/// </summary>
		public static string Error(string error)
		{
			return JsonSerializer.Serialize(new
			{
				success = false,
				error = error,
				timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
			});
		}

		/// <summary>
		/// Extracts a string parameter from JSON arguments.
		/// </summary>
		public static bool TryGetStringParameter(JsonDocument args, string parameterName, out string value)
		{
			value = string.Empty;

			// Try camelCase version
			if (args.RootElement.TryGetProperty(parameterName, out var prop))
			{
				value = prop.GetString() ?? string.Empty;
				return !string.IsNullOrEmpty(value);
			}

			// Try snake_case version
			var snakeCaseName = ToSnakeCase(parameterName);
			if (args.RootElement.TryGetProperty(snakeCaseName, out prop))
			{
				value = prop.GetString() ?? string.Empty;
				return !string.IsNullOrEmpty(value);
			}

			return false;
		}

		/// <summary>
		/// Extracts a boolean parameter from JSON arguments.
		/// </summary>
		public static bool TryGetBoolParameter(JsonDocument args, string parameterName, out bool value, bool defaultValue = false)
		{
			value = defaultValue;

			// Try camelCase version
			if (args.RootElement.TryGetProperty(parameterName, out var prop))
			{
				value = prop.GetBoolean();
				return true;
			}

			// Try snake_case version
			var snakeCaseName = ToSnakeCase(parameterName);
			if (args.RootElement.TryGetProperty(snakeCaseName, out prop))
			{
				value = prop.GetBoolean();
				return true;
			}

			return false;
		}

		private static string ToSnakeCase(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var result = new System.Text.StringBuilder();
			result.Append(char.ToLowerInvariant(text[0]));

			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
				{
					result.Append('_');
					result.Append(char.ToLowerInvariant(text[i]));
				}
				else
				{
					result.Append(text[i]);
				}
			}

			return result.ToString();
		}
	}
}
