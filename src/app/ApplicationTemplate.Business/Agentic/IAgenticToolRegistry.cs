using System.Collections.Generic;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Represents a tool definition for Azure AI Foundry Agents.
/// </summary>
public class AgenticToolDefinition
{
	/// <summary>
	/// Gets or sets the tool name.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the tool description.
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Gets or sets the parameter schema (JSON schema format).
	/// </summary>
	public object Parameters { get; set; }
}

/// <summary>
/// Interface for registering tool definitions with the AI agent.
/// Allows the app to dynamically define what tools are available to the AI.
/// </summary>
public interface IAgenticToolRegistry
{
	/// <summary>
	/// Registers a tool definition that will be sent to Azure AI Foundry.
	/// </summary>
	/// <param name="toolDefinition">The tool definition.</param>
	void RegisterToolDefinition(AgenticToolDefinition toolDefinition);

	/// <summary>
	/// Gets all registered tool definitions.
	/// </summary>
	/// <returns>Collection of tool definitions.</returns>
	IEnumerable<AgenticToolDefinition> GetToolDefinitions();
}
