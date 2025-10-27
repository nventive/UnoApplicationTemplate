using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Interface for executing AI agent function tool calls.
/// Allows registration of custom function handlers that can be called by the AI agent.
/// </summary>
public interface IAgenticToolExecutor
{
	/// <summary>
	/// Registers a function handler that can be called by the AI agent.
	/// </summary>
	/// <param name="functionName">The name of the function (must match the tool definition sent to Azure).</param>
	/// <param name="handler">The handler function that executes the tool logic.</param>
	void RegisterFunctionHandler(string functionName, Func<JsonDocument, CancellationToken, Task<string>> handler);

	/// <summary>
	/// Executes a registered function by name with the provided arguments.
	/// </summary>
	/// <param name="functionName">The name of the function to execute.</param>
	/// <param name="argumentsJson">The JSON string containing the function arguments.</param>
	/// <param name="ct">Cancellation token.</param>
	/// <returns>The JSON result of the function execution.</returns>
	Task<string> ExecuteFunctionAsync(string functionName, string argumentsJson, CancellationToken ct);

	/// <summary>
	/// Gets all registered function names.
	/// </summary>
	/// <returns>Collection of registered function names.</returns>
	IEnumerable<string> GetRegisteredFunctions();
}
