using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.ApiClients.Agentic;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Interface for AI chat service with Azure AI Foundry integration.
/// </summary>
public interface IAgenticChatService
{
	/// <summary>
	/// Gets the underlying API client for advanced scenarios (e.g., subscribing to navigation events).
	/// </summary>
	IAgenticApiClient ApiClient { get; }

	/// <summary>
	/// Gets the tool executor for registering custom function handlers.
	/// </summary>
	IAgenticToolExecutor ToolExecutor { get; }

	/// <summary>
	/// Initializes the AI agent and creates the assistant if needed.
	/// This should be called when the chat interface is first loaded.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	Task InitializeAsync(CancellationToken ct);

	/// <summary>
	/// Sends a chat message and gets a response from the AI agent.
	/// </summary>
	/// <param name="message">The user message to send.</param>
	/// <param name="conversationHistory">The conversation history.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The AI agent's response message.</returns>
	Task<ChatMessage> SendMessageAsync(string message, Collection<ChatMessage> conversationHistory, CancellationToken ct);

	/// <summary>
	/// Sends a chat message and streams the response from the AI agent.
	/// </summary>
	/// <param name="message">The user message to send.</param>
	/// <param name="conversationHistory">The conversation history.</param>
	/// <param name="onChunkReceived">Callback invoked for each chunk of the response.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The complete AI agent's response message.</returns>
	Task<ChatMessage> SendMessageStreamAsync(
		string message,
		Collection<ChatMessage> conversationHistory,
		System.Action<string> onChunkReceived,
		CancellationToken ct);

	/// <summary>
	/// Transcribes audio to text using speech-to-text.
	/// </summary>
	/// <param name="audioData">The audio data to transcribe.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The transcribed text.</returns>
	Task<string> TranscribeAudioAsync(byte[] audioData, CancellationToken ct);

	/// <summary>
	/// Converts text to speech.
	/// </summary>
	/// <param name="text">The text to convert to speech.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The audio data.</returns>
	Task<byte[]> TextToSpeechAsync(string text, CancellationToken ct);
}
