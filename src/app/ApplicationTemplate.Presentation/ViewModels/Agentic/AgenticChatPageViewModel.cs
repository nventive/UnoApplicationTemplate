using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationTemplate.Business.Agentic;
using ApplicationTemplate.DataAccess.ApiClients.Agentic;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
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
	{
		ResolveService(out _chatService);
		ResolveService(out _logger);

		SendMessage = this.GetCommandFromTask(OnSendMessage);
		StartVoiceInput = this.GetCommandFromTask(OnStartVoiceInput);
		StopVoiceInput = this.GetCommandFromTask(OnStopVoiceInput);
		PlayVoiceOutput = this.GetCommandFromTask<ChatMessage>(OnPlayVoiceOutput);
		ToggleVoiceOutputCommand = this.GetCommand(OnToggleVoiceOutput);

		InitializeConversation();
		
		// Initialize the AI agent when the ViewModel is created
		_ = InitializeAgentAsync();
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
	/// Gets or sets a value indicating whether the UI is busy (sending message or processing).
	/// </summary>
	public bool IsBusy
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
	/// Gets or sets a value indicating whether voice output is enabled.
	/// </summary>
	public bool IsVoiceOutputEnabled
	{
		get => this.Get<bool>(initialValue: true);
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
	/// Gets the command to send a message (alias for XAML binding).
	/// </summary>
	public ICommand SendMessageCommand => SendMessage;

	/// <summary>
	/// Gets the command to start voice input.
	/// </summary>
	public ICommand StartVoiceInput { get; }

	/// <summary>
	/// Gets the command to start voice input (alias for XAML binding).
	/// </summary>
	public ICommand StartVoiceInputCommand => StartVoiceInput;

	/// <summary>
	/// Gets the command to stop voice input.
	/// </summary>
	public ICommand StopVoiceInput { get; }

	/// <summary>
	/// Gets the command to play voice output for a message.
	/// </summary>
	public ICommand PlayVoiceOutput { get; }

	/// <summary>
	/// Gets the command to toggle voice output.
	/// </summary>
	public ICommand ToggleVoiceOutputCommand { get; }

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

	private async Task InitializeAgentAsync()
	{
		try
		{
			_logger.LogInformation("Initializing AI Agent on page load");
			IsBusy = true;

			// Initialize the agent (creates HTTP client and assistant if needed)
			await _chatService.InitializeAsync(CancellationToken.None);

			// Subscribe to navigation events from the AI agent
			_chatService.ApiClient.NavigationRequested += OnNavigationRequested;

			_logger.LogInformation("AI Agent initialized successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error initializing AI Agent");

			// Add error message to chat
			await RunOnDispatcher(CancellationToken.None, async ct =>
			{
				Messages.Add(new ChatMessage
				{
					Role = "assistant",
					Content = "I'm sorry, I encountered an error during initialization. Please try reloading the page.",
					Timestamp = DateTime.Now
				});
				await Task.CompletedTask;
			});
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task OnSendMessage(CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(CurrentMessage) || IsSending)
		{
			return;
		}

		var userMessage = CurrentMessage;
		
		try
		{
			IsSending = true;
			IsBusy = true;
			
			// Clear input field immediately
			CurrentMessage = string.Empty;

			// Create user message
			var userChatMessage = new ChatMessage
			{
				Role = "user",
				Content = userMessage,
				Timestamp = DateTime.Now
			};
			
			// Add user message to collection on UI thread
			await RunOnDispatcher(ct, async ct2 =>
			{
				Messages.Add(userChatMessage);
				await Task.CompletedTask;
			});

			// Send message to the assistant (uses Persistent Agents API with threads)
			var response = await _chatService.SendMessageAsync(
				userMessage,
				new ObservableCollection<ChatMessage>(Messages),
				ct);

			// Add assistant's response on UI thread
			await RunOnDispatcher(ct, async ct2 =>
			{
				Messages.Add(response);
				await Task.CompletedTask;
			});

			_logger.LogInformation("Message sent and response received");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending message");

			// Add error message on UI thread
			await RunOnDispatcher(ct, async ct2 =>
			{
				Messages.Add(new ChatMessage
				{
					Role = "assistant",
					Content = "I'm sorry, I encountered an error processing your request. Please try again.",
					Timestamp = DateTime.Now
				});
				await Task.CompletedTask;
			});
		}
		finally
		{
			IsSending = false;
			IsBusy = false;
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

	private void OnToggleVoiceOutput()
	{
		IsVoiceOutputEnabled = !IsVoiceOutputEnabled;
		_logger.LogInformation("Voice output {Status}", IsVoiceOutputEnabled ? "enabled" : "disabled");
	}

	private void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
	{
		// Handle navigation on the UI thread
		_ = RunOnDispatcher(CancellationToken.None, async ct =>
		{
			try
			{
				_logger.LogInformation("Navigation requested: {Type}, Page: {Page}", e.NavigationType, e.PageName);

				var sectionsNavigator = this.GetService<ISectionsNavigator>();

				switch (e.NavigationType)
				{
					case NavigationType.Page:
						// Navigate to the appropriate section with proper ViewModel factory
						switch (e.PageName)
						{
							case "Posts":
								await sectionsNavigator.SetActiveSection(ct, nameof(MenuViewModel.Section.Posts), () => new PostsPageViewModel());
								break;
							
							case "Home" or "DadJokes" or "Jokes":
								await sectionsNavigator.SetActiveSection(ct, nameof(MenuViewModel.Section.Home), () => new DadJokesPageViewModel());
								break;
							
							case "Settings" or "UserProfile" or "Profiles" or "Profile" or "EditProfile":
								await sectionsNavigator.SetActiveSection(ct, nameof(MenuViewModel.Section.Settings), () => new SettingsPageViewModel());
								break;
							
							case "Agentic" or "AgenticChat" or "Chat":
								await sectionsNavigator.SetActiveSection(ct, nameof(MenuViewModel.Section.Agentic), () => new AgenticChatPageViewModel());
								break;
							
							default:
								_logger.LogWarning("Unknown page name: {PageName}", e.PageName);
								break;
						}
						break;

					case NavigationType.Back:
						// Navigate to Home as a fallback for back navigation
						await sectionsNavigator.SetActiveSection(ct, nameof(MenuViewModel.Section.Home), () => new DadJokesPageViewModel());
						break;

					case NavigationType.Settings:
						await sectionsNavigator.SetActiveSection(ct, nameof(MenuViewModel.Section.Settings), () => new SettingsPageViewModel());
						break;

					case NavigationType.Logout:
						// TODO: Implement logout logic
						_logger.LogWarning("Logout requested but not implemented yet");
						break;

					case NavigationType.DrawContent:
						var drawingService = this.GetService<Business.Agentic.IDrawingModalService>();
						await drawingService.ShowDrawingAsync(e.SvgContent, e.Title, e.Description, ct);
						break;
				}

				_logger.LogInformation("Navigation completed successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error handling navigation request");
			}
		});
	}
}
