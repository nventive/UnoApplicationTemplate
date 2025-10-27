using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.ApiClients.Agentic;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Service for AI chat with Azure AI Foundry integration.
/// Handles conversation flow with Azure AI Foundry Agents API (server-side tool execution).
/// </summary>
public class AgenticChatService : IAgenticChatService
{
	private readonly IAgenticApiClient _apiClient;
	private readonly IAgenticToolExecutor _toolExecutor;
	private readonly ILogger<AgenticChatService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticChatService"/> class.
	/// </summary>
	/// <param name="apiClient">The AI agent API client.</param>
	/// <param name="toolExecutor">The tool executor for client-side function registration.</param>
	/// <param name="logger">The logger.</param>
	public AgenticChatService(
		IAgenticApiClient apiClient,
		IAgenticToolExecutor toolExecutor,
		ILogger<AgenticChatService> logger)
	{
		_apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
		_toolExecutor = toolExecutor ?? throw new ArgumentNullException(nameof(toolExecutor));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <inheritdoc/>
	public IAgenticApiClient ApiClient => _apiClient;

	/// <inheritdoc/>
	public IAgenticToolExecutor ToolExecutor => _toolExecutor;

	/// <inheritdoc/>
	public async Task InitializeAsync(CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Initializing AI Assistant");

			// This will ensure the HTTP client is created and the assistant exists
			// (or creates a new one if AssistantId is empty in config)
			await _apiClient.InitializeAsync(ct);

			_logger.LogInformation("AI Assistant initialized successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error initializing AI Assistant");
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<ChatMessage> SendMessageAsync(string message, Collection<ChatMessage> conversationHistory, CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Sending message to AI Assistant: {Message}", message);

			// Use the Assistants API (with automatic tool handling on server-side)
			var response = await _apiClient.SendMessageToAssistantAsync(message, ct);

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending message to AI assistant");
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<ChatMessage> SendMessageStreamAsync(
		string message,
		Collection<ChatMessage> conversationHistory,
		Action<string> onChunkReceived,
		CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Sending streaming message to AI agent: {Message}", message);

			// Convert conversation history to API format
			var apiMessages = ConvertToApiMessages(conversationHistory);

			// Stream the response
			var response = await _apiClient.SendChatCompletionStreamAsync(apiMessages, onChunkReceived, ct);

			// Check if the response contains tool calls
			if (response.ToolCalls != null && response.ToolCalls.Count > 0)
			{
				_logger.LogInformation("AI agent requested {Count} tool call(s) in stream", response.ToolCalls.Count);

				// Execute tool calls
				var toolResults = new Collection<ChatMessage>();
				foreach (var toolCall in response.ToolCalls)
				{
					var toolResult = await _toolExecutor.ExecuteFunctionAsync(
						toolCall.FunctionName,
						toolCall.FunctionArguments,
						ct);

					toolResults.Add(new ChatMessage
					{
						Role = "tool",
						Content = toolResult,
						ToolCallId = toolCall.Id,
						Timestamp = DateTime.Now
					});
				}

				// Add the assistant's tool call message to history
				conversationHistory.Add(response);

				// Add tool results to history
				foreach (var result in toolResults)
				{
					conversationHistory.Add(result);
				}

				// Get the final response from the AI agent after tool execution
				var finalApiMessages = ConvertToApiMessages(conversationHistory);
				var finalResponse = await _apiClient.SendChatCompletionStreamAsync(finalApiMessages, onChunkReceived, ct);

				return finalResponse;
			}

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending streaming message to AI agent");
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<string> TranscribeAudioAsync(byte[] audioData, CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Transcribing audio, size: {Size} bytes", audioData.Length);
			return await _apiClient.TranscribeAudioAsync(audioData, ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error transcribing audio");
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<byte[]> TextToSpeechAsync(string text, CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Converting text to speech: {Text}", text);
			return await _apiClient.TextToSpeechAsync(text, ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}

	private Collection<ChatMessage> ConvertToApiMessages(Collection<ChatMessage> messages)
	{
		// In this implementation, we're using the same ChatMessage model
		// In a real implementation, you might need to convert between different models
		return messages;
	}
}
