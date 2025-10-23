using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Interface for executing AI agent tool calls.
/// </summary>
public interface IAgenticToolExecutor
{
	/// <summary>
	/// Registers a function handler that will be called when the AI agent invokes the function.
	/// </summary>
	/// <param name="functionName">The name of the function.</param>
	/// <param name="handler">The handler that implements the function logic.</param>
	void RegisterFunctionHandler(string functionName, Func<JsonDocument, CancellationToken, Task<string>> handler);

	/// <summary>
	/// Execute a function tool call from the agent.
	/// </summary>
	/// <param name="functionName">Name of the function to execute.</param>
	/// <param name="argumentsJson">JSON string of function arguments.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>Result as JSON string.</returns>
	Task<string> ExecuteFunctionAsync(string functionName, string argumentsJson, CancellationToken ct);
}
