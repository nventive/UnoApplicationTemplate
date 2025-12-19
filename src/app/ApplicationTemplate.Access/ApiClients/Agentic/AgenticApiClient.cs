using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApplicationTemplate.DataAccess.ApiClients.Agentic;

/// <summary>
/// API client for Azure AI Foundry Agents API.
/// Uses HTTP REST API for full control and mobile compatibility.
/// </summary>
public partial class AgenticApiClient : IAgenticApiClient
{
	private readonly AgenticConfiguration _configuration;
	private readonly ILogger<AgenticApiClient> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticApiClient"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client (not used in current implementation but kept for DI compatibility).</param>
	/// <param name="configuration">The AI agent configuration.</param>
	/// <param name="logger">The logger.</param>
	public AgenticApiClient(
		System.Net.Http.HttpClient httpClient,
		IOptions<AgenticConfiguration> configuration,
		ILogger<AgenticApiClient> logger)
	{
		_configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <inheritdoc/>
	public Task<ChatMessage> SendChatCompletionAsync(System.Collections.ObjectModel.Collection<ChatMessage> messages, CancellationToken ct)
	{
		// This method is not used with the Agents API.
		// Use SendMessageToAssistantAsync instead, which handles agent threads, runs, and tool calling.
		throw new NotSupportedException(
			"SendChatCompletionAsync is not supported when using Azure AI Foundry Agents API. " +
			"Use SendMessageToAssistantAsync instead for agent-based conversations with tool calling support.");
	}

	/// <inheritdoc/>
	public Task<ChatMessage> SendChatCompletionStreamAsync(
		System.Collections.ObjectModel.Collection<ChatMessage> messages,
		Action<string> onChunkReceived,
		CancellationToken ct)
	{
		// Streaming is not supported with the Agents API in the current implementation.
		// The Agents API uses a run-based model with polling.
		throw new NotSupportedException(
			"SendChatCompletionStreamAsync is not supported when using Azure AI Foundry Agents API. " +
			"Use SendMessageToAssistantAsync instead. The Agents API uses a run-based polling model.");
	}

	/// <inheritdoc/>
	public Task<string> TranscribeAudioAsync(byte[] audioData, CancellationToken ct)
	{
		// Audio transcription is not implemented in the current Agents API implementation.
		// Future: Could use Azure Speech Services or agent's multimodal audio support.
		throw new NotSupportedException(
			"TranscribeAudioAsync is not implemented. " +
			"Consider using Azure Speech Services or the agent's multimodal audio capabilities.");
	}

	/// <inheritdoc/>
	public Task<byte[]> TextToSpeechAsync(string text, CancellationToken ct)
	{
		// Text-to-speech is not implemented in the current Agents API implementation.
		// Future: Could use Azure Speech Services or agent's multimodal audio output.
		throw new NotSupportedException(
			"TextToSpeechAsync is not implemented. " +
			"Consider using Azure Speech Services or the agent's multimodal audio capabilities.");
	}
}
