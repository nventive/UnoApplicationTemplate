using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationTemplate.Business.Agentic;
using ApplicationTemplate.DataAccess.ApiClients.Agentic;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// ViewModel for the Agentic Chat page with voice integration.
/// </summary>
public class AgenticChatPageViewModel : ViewModel
{
	private readonly IAgenticChatService _chatService;
	private readonly ILogger<AgenticChatPageViewModel> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticChatPageViewModel"/> class.
	/// </summary>
	public AgenticChatPageViewModel()
		: base()
	{
		_chatService = this.GetService<IAgenticChatService>();
		_logger = this.GetService<ILogger<AgenticChatPageViewModel>>();

		SendMessage = this.GetCommandFromTask(OnSendMessage);
		StartVoiceInput = this.GetCommandFromTask(OnStartVoiceInput);
		StopVoiceInput = this.GetCommandFromTask(OnStopVoiceInput);
		PlayVoiceOutput = this.GetCommandFromTask<ChatMessage>(OnPlayVoiceOutput);

		InitializeConversation();
	}

	/// <summary>
	/// Gets the collection of chat messages.
	/// </summary>
	public ObservableCollection<ChatMessage> Messages { get; } = new ObservableCollection<ChatMessage>();

	/// <summary>
	/// Gets or sets the current message being typed by the user.
	/// </summary>
	public string CurrentMessage
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether a message is being sent.
	/// </summary>
	public bool IsSending
	{
		get => this.Get<bool>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether voice input is active.
	/// </summary>
	public bool IsVoiceInputActive
	{
		get => this.Get<bool>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the assistant is speaking.
	/// </summary>
	public bool IsSpeaking
	{
		get => this.Get<bool>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets or sets the current streaming message from the assistant.
	/// </summary>
	public string StreamingMessage
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets the command to send a message.
	/// </summary>
	public ICommand SendMessage { get; }

	/// <summary>
	/// Gets the command to start voice input.
	/// </summary>
	public ICommand StartVoiceInput { get; }

	/// <summary>
	/// Gets the command to stop voice input.
	/// </summary>
	public ICommand StopVoiceInput { get; }

	/// <summary>
	/// Gets the command to play voice output for a message.
	/// </summary>
	public ICommand PlayVoiceOutput { get; }

	private void InitializeConversation()
	{
		// Add a welcome message from the assistant
		Messages.Add(new ChatMessage
		{
			Role = "assistant",
			Content = "Hello! I'm your AI assistant. I can help you navigate the app and answer your questions. How can I help you today?",
			Timestamp = DateTime.Now
		});
	}

	private async Task OnSendMessage(CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(CurrentMessage) || IsSending)
		{
			return;
		}

		try
		{
			IsSending = true;
			var userMessage = CurrentMessage;
			CurrentMessage = string.Empty;

			// Add user message to the conversation
			var userChatMessage = new ChatMessage
			{
				Role = "user",
				Content = userMessage,
				Timestamp = DateTime.Now
			};
			Messages.Add(userChatMessage);

			// Clear streaming message
			StreamingMessage = string.Empty;

			// Add a placeholder for the assistant's response
			var assistantMessage = new ChatMessage
			{
				Role = "assistant",
				Content = string.Empty,
				Timestamp = DateTime.Now,
				IsStreaming = true
			};
			Messages.Add(assistantMessage);

			// Send message and stream the response
			var response = await _chatService.SendMessageStreamAsync(
				userMessage,
				new ObservableCollection<ChatMessage>(Messages.Where(m => !m.IsStreaming)),
				chunk =>
				{
					// Update the streaming message
					StreamingMessage += chunk;
					assistantMessage.Content = StreamingMessage;
				},
				ct);

			// Update the assistant's message with the complete response
			assistantMessage.Content = response.Content;
			assistantMessage.IsStreaming = false;
			assistantMessage.ToolCalls = response.ToolCalls;

			// Clear streaming message
			StreamingMessage = string.Empty;

			_logger.LogInformation("Message sent and response received");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending message");

			Messages.Add(new ChatMessage
			{
				Role = "assistant",
				Content = "I'm sorry, I encountered an error processing your request. Please try again.",
				Timestamp = DateTime.Now
			});
		}
		finally
		{
			IsSending = false;
		}
	}

	private async Task OnStartVoiceInput(CancellationToken ct)
	{
		try
		{
			IsVoiceInputActive = true;
			_logger.LogInformation("Voice input started");

			// In a real implementation, you would:
			// 1. Request microphone permissions
			// 2. Start recording audio
			// 3. Store the audio data
			// This is a placeholder implementation

			// TODO: Implement platform-specific audio recording
			await Task.Delay(100, ct); // Placeholder
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error starting voice input");
			IsVoiceInputActive = false;
		}
	}

	private async Task OnStopVoiceInput(CancellationToken ct)
	{
		try
		{
			IsVoiceInputActive = false;
			_logger.LogInformation("Voice input stopped");

			// In a real implementation, you would:
			// 1. Stop recording audio
			// 2. Send the audio to the transcription service
			// 3. Display the transcribed text
			// 4. Optionally send it automatically

			// Placeholder implementation
			IsSending = true;

			// TODO: Get the recorded audio data
			byte[] audioData = Array.Empty<byte>();

			if (audioData.Length > 0)
			{
				// Transcribe the audio
				var transcribedText = await _chatService.TranscribeAudioAsync(audioData, ct);

				if (!string.IsNullOrWhiteSpace(transcribedText))
				{
					CurrentMessage = transcribedText;
					// Optionally send automatically
					// OnSendMessage(ct);
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error stopping voice input");
		}
		finally
		{
			IsSending = false;
		}
	}

	private async Task OnPlayVoiceOutput(CancellationToken ct, ChatMessage message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message.Content))
			{
				return;
			}

			IsSpeaking = true;
			_logger.LogInformation("Playing voice output");

			// Convert text to speech
			var audioData = await _chatService.TextToSpeechAsync(message.Content, ct);

			// In a real implementation, you would:
			// 1. Play the audio using platform-specific audio player
			// This is a placeholder implementation

			// TODO: Implement platform-specific audio playback
			await Task.Delay(1000, ct); // Placeholder

			_logger.LogInformation("Voice output played");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error playing voice output");
		}
		finally
		{
			IsSpeaking = false;
		}
	}
}
