using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.ApiClients.Agentic;

/// <summary>
/// Interface for AI agent API client to communicate with Azure AI Foundry.
/// </summary>
public interface IAgenticApiClient
{
	/// <summary>
	/// Event raised when the AI agent requests navigation to a different page.
	/// </summary>
#nullable enable
	event EventHandler<NavigationRequestedEventArgs>? NavigationRequested;
#nullable restore

	/// <summary>
	/// Initializes the AI agent client and ensures the assistant is created.
	/// Should be called before any other operations.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	Task InitializeAsync(CancellationToken ct);

	/// <summary>
	/// Sends a chat completion request to Azure AI Foundry.
	/// </summary>
	/// <param name="messages">The conversation messages.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The AI agent's response message.</returns>
	Task<ChatMessage> SendChatCompletionAsync(Collection<ChatMessage> messages, CancellationToken ct);

	/// <summary>
	/// Sends a streaming chat completion request to Azure AI Foundry.
	/// </summary>
	/// <param name="messages">The conversation messages.</param>
	/// <param name="onChunkReceived">Callback invoked for each chunk of the response.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The complete AI agent's response message.</returns>
	Task<ChatMessage> SendChatCompletionStreamAsync(
		Collection<ChatMessage> messages,
		Action<string> onChunkReceived,
		CancellationToken ct);

	/// <summary>
	/// Sends a message to the Azure AI Agent (Assistants API) and receives response.
	/// </summary>
	/// <param name="userMessage">The user's message.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The assistant's response message.</returns>
	Task<ChatMessage> SendMessageToAssistantAsync(string userMessage, CancellationToken ct);

	/// <summary>
	/// Resets the conversation thread.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	Task ResetConversationAsync(CancellationToken ct);

	/// <summary>
	/// Deletes the assistant (cleanup).
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	Task DeleteAssistantAsync(CancellationToken ct);

	/// <summary>
	/// Transcribes audio to text using Azure Speech Services.
	/// </summary>
	/// <param name="audioData">The audio data to transcribe.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The transcribed text.</returns>
	Task<string> TranscribeAudioAsync(byte[] audioData, CancellationToken ct);

	/// <summary>
	/// Converts text to speech using Azure Speech Services.
	/// </summary>
	/// <param name="text">The text to convert to speech.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The audio data.</returns>
	Task<byte[]> TextToSpeechAsync(string text, CancellationToken ct);
}
