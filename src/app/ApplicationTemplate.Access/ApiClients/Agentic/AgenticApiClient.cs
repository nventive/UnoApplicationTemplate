using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.Configuration;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

// Use type aliases to avoid naming conflicts
using AzureChatMessage = OpenAI.Chat.ChatMessage;
using AppChatMessage = ApplicationTemplate.DataAccess.ApiClients.Agentic.ChatMessage;

namespace ApplicationTemplate.DataAccess.ApiClients.Agentic;

/// <summary>
/// API client for Azure AI Foundry chat completion with function calling support.
/// </summary>
public class AgenticApiClient : IAgenticApiClient
{
	private readonly HttpClient _httpClient;
	private readonly AgenticConfiguration _configuration;
	private readonly ILogger<AgenticApiClient> _logger;
	private readonly AzureOpenAIClient _openAIClient;
	private readonly ChatClient _chatClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticApiClient"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client for speech services.</param>
	/// <param name="configuration">The AI agent configuration.</param>
	/// <param name="logger">The logger.</param>
	public AgenticApiClient(
		HttpClient httpClient,
		IOptions<AgenticConfiguration> configuration,
		ILogger<AgenticApiClient> logger)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		// Initialize Azure OpenAI client
		var credential = new ApiKeyCredential(_configuration.ApiKey);
		_openAIClient = new AzureOpenAIClient(
			new Uri(_configuration.Endpoint),
			credential);

		_chatClient = _openAIClient.GetChatClient(_configuration.DeploymentName);
	}

	/// <inheritdoc/>
	public async Task<AppChatMessage> SendChatCompletionAsync(Collection<AppChatMessage> messages, CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Sending chat completion request to Azure AI Foundry");

			// Convert messages to Azure SDK format
			var chatMessages = new List<AzureChatMessage>();
			
			// Add system message
			chatMessages.Add(AzureChatMessage.CreateSystemMessage(
				"You are a helpful AI assistant integrated into a mobile application. You can help users navigate the app, access information, and perform tasks using the available tools. Be concise and friendly."));

			// Add conversation messages
			foreach (var msg in messages)
			{
				if (msg.Role == "user")
				{
					chatMessages.Add(AzureChatMessage.CreateUserMessage(msg.Content));
				}
				else if (msg.Role == "assistant")
				{
					chatMessages.Add(AzureChatMessage.CreateAssistantMessage(msg.Content));
				}
				else if (msg.Role == "tool")
				{
					chatMessages.Add(AzureChatMessage.CreateToolMessage(msg.ToolCallId, msg.Content));
				}
			}

			// Create chat completion options with tools
			var options = new ChatCompletionOptions
			{
				Temperature = (float)_configuration.Temperature,
				MaxOutputTokenCount = _configuration.MaxTokens
			};

			// Add function tools
			foreach (var tool in GetAvailableToolDefinitions())
			{
				options.Tools.Add(tool);
			}

			// Send request
			var response = await _chatClient.CompleteChatAsync(chatMessages, options, ct);

			// Convert response to our ChatMessage model
			var assistantMessage = new AppChatMessage
			{
				Role = "assistant",
				Content = response.Value.Content[0].Text ?? string.Empty,
				Timestamp = DateTime.Now
			};

			// Handle tool calls if present
			if (response.Value.ToolCalls != null && response.Value.ToolCalls.Count > 0)
			{
				foreach (var toolCall in response.Value.ToolCalls)
				{
					if (toolCall is ChatToolCall functionToolCall)
					{
						assistantMessage.ToolCalls.Add(new ToolCall
						{
							Id = functionToolCall.Id,
							Type = "function",
							FunctionName = functionToolCall.FunctionName,
							FunctionArguments = functionToolCall.FunctionArguments.ToString()
						});
					}
				}
			}

			return assistantMessage;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending chat completion request");
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<AppChatMessage> SendChatCompletionStreamAsync(
		Collection<AppChatMessage> messages,
		Action<string> onChunkReceived,
		CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Sending streaming chat completion request to Azure AI Foundry");

			// Convert messages to Azure SDK format
			var chatMessages = new List<AzureChatMessage>();
			
			// Add system message
			chatMessages.Add(AzureChatMessage.CreateSystemMessage(
				"You are a helpful AI assistant integrated into a mobile application. You can help users navigate the app, access information, and perform tasks using the available tools. Be concise and friendly."));

			// Add conversation messages
			foreach (var msg in messages)
			{
				if (msg.Role == "user")
				{
					chatMessages.Add(AzureChatMessage.CreateUserMessage(msg.Content));
				}
				else if (msg.Role == "assistant")
				{
					chatMessages.Add(AzureChatMessage.CreateAssistantMessage(msg.Content));
				}
				else if (msg.Role == "tool")
				{
					chatMessages.Add(AzureChatMessage.CreateToolMessage(msg.ToolCallId, msg.Content));
				}
			}

			// Create chat completion options with tools
			var options = new ChatCompletionOptions
			{
				Temperature = (float)_configuration.Temperature,
				MaxOutputTokenCount = _configuration.MaxTokens
			};

			// Add function tools
			foreach (var tool in GetAvailableToolDefinitions())
			{
				options.Tools.Add(tool);
			}

			// Stream the response
			var completeContent = new StringBuilder();
			var toolCalls = new Collection<ToolCall>();

			await foreach (var update in _chatClient.CompleteChatStreamingAsync(chatMessages, options, ct))
			{
				foreach (var contentPart in update.ContentUpdate)
				{
					var chunk = contentPart.Text ?? string.Empty;
					if (!string.IsNullOrEmpty(chunk))
					{
						completeContent.Append(chunk);
						onChunkReceived?.Invoke(chunk);
					}
				}

				// Handle tool calls if present
				if (update.ToolCallUpdates != null && update.ToolCallUpdates.Count > 0)
				{
					_logger.LogDebug("Tool calls detected in stream");
					// Tool calls are accumulated across updates
					// We'll handle them in the final response
				}
			}

			// Return the complete message
			return new AppChatMessage
			{
				Role = "assistant",
				Content = completeContent.ToString(),
				ToolCalls = toolCalls,
				Timestamp = DateTime.Now
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending streaming chat completion request");
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<string> TranscribeAudioAsync(byte[] audioData, CancellationToken ct)
	{
		try
		{
			_logger.LogInformation("Transcribing audio using Azure Speech Services");

			if (string.IsNullOrEmpty(_configuration.SpeechEndpoint) || string.IsNullOrEmpty(_configuration.SpeechApiKey))
			{
				throw new InvalidOperationException("Speech service endpoint or API key not configured");
			}

			// Create multipart form data content
			using var content = new MultipartFormDataContent();
			var audioContent = new ByteArrayContent(audioData);
			audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
			content.Add(audioContent, "file", "audio.wav");

			// Create the HTTP request
			using var request = new HttpRequestMessage(HttpMethod.Post, _configuration.SpeechEndpoint + "/speech-to-text/transcriptions");
			request.Headers.Add("Ocp-Apim-Subscription-Key", _configuration.SpeechApiKey);
			request.Content = content;

			// Send the request
			using var response = await _httpClient.SendAsync(request, ct);
			response.EnsureSuccessStatusCode();

			// Parse the response
			var responseContent = await response.Content.ReadAsStringAsync(ct);
			var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);

			if (responseData.TryGetProperty("text", out var textElement))
			{
				return textElement.GetString() ?? string.Empty;
			}

			return string.Empty;
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
			_logger.LogInformation("Converting text to speech using Azure Speech Services");

			if (string.IsNullOrEmpty(_configuration.SpeechEndpoint) || string.IsNullOrEmpty(_configuration.SpeechApiKey))
			{
				throw new InvalidOperationException("Speech service endpoint or API key not configured");
			}

			// Create the request payload
			var requestPayload = new
			{
				text = text,
				voice = "en-US-JennyNeural" // Default voice
			};

			var jsonPayload = JsonSerializer.Serialize(requestPayload);

			// Create the HTTP request
			using var request = new HttpRequestMessage(HttpMethod.Post, _configuration.SpeechEndpoint + "/text-to-speech/synthesize");
			request.Headers.Add("Ocp-Apim-Subscription-Key", _configuration.SpeechApiKey);
			request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			// Send the request
			using var response = await _httpClient.SendAsync(request, ct);
			response.EnsureSuccessStatusCode();

			// Return the audio data
			return await response.Content.ReadAsByteArrayAsync(ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}

	private IEnumerable<ChatTool> GetAvailableToolDefinitions()
	{
		// Define navigate_to_page function
		yield return ChatTool.CreateFunctionTool(
			functionName: "navigate_to_page",
			functionDescription: "Navigate to a specific page in the application",
			functionParameters: BinaryData.FromString("""
				{
					"type": "object",
					"properties": {
						"page_name": {
							"type": "string",
							"description": "The name of the page to navigate to. Available pages: DadJokes, Posts, Settings, UserProfile"
						},
						"clear_stack": {
							"type": "boolean",
							"description": "Whether to clear the navigation stack before navigating"
						}
					},
					"required": ["page_name"]
				}
				"""));

		// Define get_current_page function
		yield return ChatTool.CreateFunctionTool(
			functionName: "get_current_page",
			functionDescription: "Get information about the current page the user is viewing",
			functionParameters: BinaryData.FromString("""
				{
					"type": "object",
					"properties": {}
				}
				"""));

		// Define go_back function
		yield return ChatTool.CreateFunctionTool(
			functionName: "go_back",
			functionDescription: "Navigate back to the previous page in the navigation stack",
			functionParameters: BinaryData.FromString("""
				{
					"type": "object",
					"properties": {}
				}
				"""));

		// Define open_settings function
		yield return ChatTool.CreateFunctionTool(
			functionName: "open_settings",
			functionDescription: "Open the settings page",
			functionParameters: BinaryData.FromString("""
				{
					"type": "object",
					"properties": {}
				}
				"""));

		// Define logout function
		yield return ChatTool.CreateFunctionTool(
			functionName: "logout",
			functionDescription: "Log out the current user",
			functionParameters: BinaryData.FromString("""
				{
					"type": "object",
					"properties": {}
				}
				"""));
	}
}
