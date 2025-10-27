using System;
using System.Collections.ObjectModel;

namespace ApplicationTemplate.DataAccess.ApiClients.Agentic;

/// <summary>
/// Represents a chat message in the AI agent conversation.
/// </summary>
public class ChatMessage
{
	/// <summary>
	/// Gets or sets the unique identifier for the message.
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	/// Gets or sets the role of the message sender (user, assistant, system, tool).
	/// </summary>
	public string Role { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the content of the message.
	/// </summary>
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the timestamp when the message was created.
	/// </summary>
	public DateTime Timestamp { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the tool calls if this is an assistant message with function calls.
	/// </summary>
	public Collection<ToolCall> ToolCalls { get; set; } = new Collection<ToolCall>();

	/// <summary>
	/// Gets or sets the tool call ID if this is a tool response message.
	/// </summary>
	public string ToolCallId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether this message is being streamed (partial response).
	/// </summary>
	public bool IsStreaming { get; set; }

	/// <summary>
	/// Gets a value indicating whether this message is from the user.
	/// </summary>
	public bool IsFromUser => Role == "user";
}

/// <summary>
/// Represents a tool/function call made by the AI agent.
/// </summary>
public class ToolCall
{
	/// <summary>
	/// Gets or sets the unique identifier for the tool call.
	/// </summary>
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the type of the tool call (usually "function").
	/// </summary>
	public string Type { get; set; } = "function";

	/// <summary>
	/// Gets or sets the function name.
	/// </summary>
	public string FunctionName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the function arguments as a JSON string.
	/// </summary>
	public string FunctionArguments { get; set; } = string.Empty;
}
