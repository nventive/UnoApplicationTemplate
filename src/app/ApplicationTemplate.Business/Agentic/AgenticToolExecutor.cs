using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Executes function tool calls from the Azure AI agent.
/// This is a dispatcher that allows dynamic registration of function handlers.
/// The actual implementations are provided by the Presentation layer or other services.
/// </summary>
public class AgenticToolExecutor : IAgenticToolExecutor, IAgenticToolRegistry
{
	private readonly ILogger<AgenticToolExecutor> _logger;
	private readonly Dictionary<string, Func<JsonDocument, CancellationToken, Task<string>>> _functionHandlers;
	private readonly List<AgenticToolDefinition> _toolDefinitions;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticToolExecutor"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public AgenticToolExecutor(ILogger<AgenticToolExecutor> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_functionHandlers = new Dictionary<string, Func<JsonDocument, CancellationToken, Task<string>>>();
		_toolDefinitions = new List<AgenticToolDefinition>();
	}

	/// <inheritdoc/>
	public void RegisterFunctionHandler(string functionName, Func<JsonDocument, CancellationToken, Task<string>> handler)
	{
		if (string.IsNullOrWhiteSpace(functionName))
		{
			throw new ArgumentException("Function name cannot be null or empty", nameof(functionName));
		}

		if (handler == null)
		{
			throw new ArgumentNullException(nameof(handler));
		}

		_functionHandlers[functionName] = handler;
		_logger.LogInformation("Registered function handler: {FunctionName}", functionName);
	}

	/// <inheritdoc/>
	public async Task<string> ExecuteFunctionAsync(string functionName, string argumentsJson, CancellationToken ct)
	{
		_logger.LogInformation("Executing function: {FunctionName} with args: {Arguments}", functionName, argumentsJson);

		if (!_functionHandlers.TryGetValue(functionName, out var handler))
		{
			var error = $"Function '{functionName}' is not registered. Available functions: {string.Join(", ", _functionHandlers.Keys)}";
			_logger.LogWarning(error);
			return JsonSerializer.Serialize(new { success = false, error });
		}

		try
		{
			var args = JsonDocument.Parse(argumentsJson);
			var result = await handler(args, ct);
			_logger.LogInformation("Function {FunctionName} executed successfully", functionName);
			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing function {FunctionName}", functionName);
			return JsonSerializer.Serialize(new { success = false, error = ex.Message });
		}
	}

	/// <inheritdoc/>
	public IEnumerable<string> GetRegisteredFunctions()
	{
		return _functionHandlers.Keys;
	}

	/// <inheritdoc/>
	public void RegisterToolDefinition(AgenticToolDefinition toolDefinition)
	{
		if (toolDefinition == null)
		{
			throw new ArgumentNullException(nameof(toolDefinition));
		}

		_toolDefinitions.Add(toolDefinition);
		_logger.LogInformation("Registered tool definition: {ToolName}", toolDefinition.Name);
	}

	/// <inheritdoc/>
	public IEnumerable<AgenticToolDefinition> GetToolDefinitions()
	{
		return _toolDefinitions;
	}
}
