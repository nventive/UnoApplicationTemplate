using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.Configuration;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.DataAccess.ApiClients.Agentic;

/// <summary>
/// Creates or retrieves a persistent agent with custom function tool definitions for app navigation.
/// </summary>
public partial class AgenticApiClient
{
	/// <summary>
	/// Event raised when the AI agent requests navigation to a different page.
	/// </summary>
	public event EventHandler<NavigationRequestedEventArgs> NavigationRequested;

	private HttpClient _agentsHttpClient;
	private string _currentAgentId;
	private string _currentThreadId;
	
	// Azure AI Foundry Agents API version
	// Reference: https://learn.microsoft.com/en-us/azure/ai-foundry/agents/quickstart
	// GA version: 2025-05-01, Latest preview: 2025-05-15-preview
	private const string ApiVersion = "2025-05-01";

	/// <summary>
	/// Initializes the AI agent client and ensures the assistant is created.
	/// This is a public method that can be called explicitly to initialize the agent before sending messages.
	/// </summary>
	/// <param name="ct">Cancellation token.</param>
	public async Task InitializeAsync(CancellationToken ct)
	{
		await EnsurePersistentClientsAsync(ct);
	}

	private async Task EnsurePersistentClientsAsync(CancellationToken ct)
	{
		if (_agentsHttpClient != null)
		{
			return;
		}

		if (string.IsNullOrEmpty(_configuration.Endpoint))
		{
			throw new InvalidOperationException("Agentic endpoint is not configured.");
		}

		_logger.LogInformation("Initializing Azure AI Agents HTTP client with Bearer token authentication");

		// Set environment variables for Service Principal authentication (if provided in config)
		// If not provided, DefaultAzureCredential will fall back to Azure CLI authentication
		if (!string.IsNullOrEmpty(_configuration.TenantId))
		{
			Environment.SetEnvironmentVariable("AZURE_TENANT_ID", _configuration.TenantId);
			_logger.LogInformation("Using Service Principal authentication (TenantId configured)");
		}

		if (!string.IsNullOrEmpty(_configuration.ClientId))
		{
			Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", _configuration.ClientId);
		}

		if (!string.IsNullOrEmpty(_configuration.ClientSecret))
		{
			Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", _configuration.ClientSecret);
		}

		// Create DefaultAzureCredential
		// This will try in order: Environment variables (Service Principal), Managed Identity, Azure CLI, etc.
		var credential = new DefaultAzureCredential();

		// Get an access token for Azure AI Foundry
		// NOTE: Azure AI Foundry requires the token audience to be "https://ai.azure.com"
		var tokenRequestContext = new TokenRequestContext(new[] { "https://ai.azure.com/.default" });
		
		_logger.LogInformation("Acquiring Azure access token for AI Foundry...");
		var token = await credential.GetTokenAsync(tokenRequestContext, ct);
		_logger.LogInformation("Successfully acquired access token (expires: {Expiry})", token.ExpiresOn);

		// Create HttpClient with Bearer token
		var baseAddress = _configuration.Endpoint;
		// Ensure the base address ends with a trailing slash for proper URI combination
		if (!baseAddress.EndsWith("/"))
		{
			baseAddress += "/";
		}
		
		_agentsHttpClient = new HttpClient
		{
			BaseAddress = new Uri(baseAddress)
		};
		// Use Bearer token authentication
		_agentsHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
		_agentsHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		_logger.LogInformation("Configured HTTP client with endpoint: {Endpoint} and Bearer token", baseAddress);

		// Ensure we have an agent with tools configured
		await EnsureAgentWithToolsAsync(ct);
	}

	private async Task EnsureAgentWithToolsAsync(CancellationToken ct)
	{
		// Load instructions from file if specified
		var instructions = await LoadInstructionsAsync(_configuration.AssistantInstructions, ct);

		// Build agent data with tools using REST API format
		var toolsList = GetNavigationToolDefinitions().ToList();
		var agentData = new
		{
			model = _configuration.DeploymentName,
			name = _configuration.AssistantName,
			instructions = instructions,
			temperature = (float)_configuration.Temperature,
			tools = toolsList.ToArray()
		};

		// Search for existing assistant by name
		_logger.LogInformation("Searching for existing assistant with name '{AssistantName}'", _configuration.AssistantName);
		
		var listResponse = await _agentsHttpClient.GetAsync($"assistants?api-version={ApiVersion}", ct);
		listResponse.EnsureSuccessStatusCode();
		
		var listJson = await listResponse.Content.ReadAsStringAsync(ct);
		var listData = JsonDocument.Parse(listJson).RootElement;
		
		string existingAssistantId = null;
		bool needsUpdate = false;
		
		if (listData.TryGetProperty("data", out var assistantsArray))
		{
			foreach (var assistant in assistantsArray.EnumerateArray())
			{
				if (assistant.TryGetProperty("name", out var nameProperty) &&
					nameProperty.GetString() == _configuration.AssistantName)
				{
					existingAssistantId = assistant.GetProperty("id").GetString();
					_logger.LogInformation("Found existing assistant '{AssistantName}' with ID {AssistantId}", _configuration.AssistantName, existingAssistantId);
					
					// Check if tools match
					if (assistant.TryGetProperty("tools", out var existingTools))
					{
						var existingToolNames = new HashSet<string>();
						foreach (var tool in existingTools.EnumerateArray())
						{
							if (tool.TryGetProperty("function", out var func) &&
								func.TryGetProperty("name", out var toolName))
							{
								existingToolNames.Add(toolName.GetString());
							}
						}
						
						var expectedToolNames = new HashSet<string>(new[] { "navigate_to_page", "get_current_page", "go_back", "open_settings", "logout" });
						
						if (!existingToolNames.SetEquals(expectedToolNames))
						{
							_logger.LogInformation("Assistant tools don't match. Expected: [{Expected}], Found: [{Found}]. Will update.",
								string.Join(", ", expectedToolNames), string.Join(", ", existingToolNames));
							needsUpdate = true;
						}
						else
						{
							_logger.LogInformation("Assistant has correct tools, will update configuration anyway to ensure latest version");
							needsUpdate = true; // Always update to ensure latest instructions and settings
						}
					}
					else
					{
						needsUpdate = true;
					}
					
					break;
				}
			}
		}
		
		if (existingAssistantId != null)
		{
			if (needsUpdate)
			{
				_logger.LogInformation("Updating assistant {AssistantId} with latest configuration", existingAssistantId);
				
				var json = JsonSerializer.Serialize(agentData);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				
				var updateResponse = await _agentsHttpClient.PostAsync($"assistants/{existingAssistantId}?api-version={ApiVersion}", content, ct);
				
				if (!updateResponse.IsSuccessStatusCode)
				{
					var errorContent = await updateResponse.Content.ReadAsStringAsync(ct);
					_logger.LogWarning("Failed to update assistant {AssistantId}. Status: {StatusCode}, Response: {Response}. Will continue with existing assistant.",
						existingAssistantId, updateResponse.StatusCode, errorContent);
				}
				else
				{
					_logger.LogInformation("Successfully updated assistant {AssistantId} with {ToolCount} tools", existingAssistantId, toolsList.Count);
				}
			}
			
			_currentAgentId = existingAssistantId;
			return;
		}

		// No existing assistant found, create a new one
		_logger.LogInformation("No assistant found with name '{AssistantName}', creating new assistant", _configuration.AssistantName);

		var createJson = JsonSerializer.Serialize(agentData);
		var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");

		var requestUri = $"assistants?api-version={ApiVersion}";
		var response = await _agentsHttpClient.PostAsync(requestUri, createContent, ct);
		
		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync(ct);
			_logger.LogError("Assistant creation failed. Status: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
		}
		
		response.EnsureSuccessStatusCode();

		var responseJson = await response.Content.ReadAsStringAsync(ct);
		var responseData = JsonDocument.Parse(responseJson).RootElement;
		_currentAgentId = responseData.GetProperty("id").GetString();

		_logger.LogInformation("Created new assistant '{AssistantName}' with ID {AssistantId} and {ToolCount} tools",
			_configuration.AssistantName, _currentAgentId, toolsList.Count);
	}

	private async Task<string> LoadInstructionsAsync(string instructionsOrPath, CancellationToken ct)
	{
		// Check if it's a file path (ends with .md or .txt)
		if (instructionsOrPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
			instructionsOrPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
		{
			try
			{
				// Try to load from embedded resource first (from Presentation assembly)
				var presentationAssembly = AppDomain.CurrentDomain.GetAssemblies()
					.FirstOrDefault(a => a.GetName().Name == "ApplicationTemplate.Presentation");

				if (presentationAssembly != null)
				{
					var resourceName = $"ApplicationTemplate.Presentation.{instructionsOrPath}";
					using var stream = presentationAssembly.GetManifestResourceStream(resourceName);
					if (stream != null)
					{
						using var reader = new System.IO.StreamReader(stream);
						_logger.LogInformation("Loading assistant instructions from embedded resource: {ResourceName}", resourceName);
						return await reader.ReadToEndAsync(ct);
					}
				}

				// Fallback: Try to load from file system
				var filePath = instructionsOrPath;
				if (!System.IO.Path.IsPathRooted(filePath))
				{
					// Relative path - try to resolve from app directory
					var appDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
					filePath = System.IO.Path.Combine(appDirectory, instructionsOrPath);
				}

				if (System.IO.File.Exists(filePath))
				{
					_logger.LogInformation("Loading assistant instructions from file: {FilePath}", filePath);
					return await System.IO.File.ReadAllTextAsync(filePath, ct);
				}
				else
				{
					_logger.LogWarning("Instructions file not found (resource or file): {InstructionsPath}, using default instructions", instructionsOrPath);
					return "You are a helpful AI assistant integrated into a mobile application. You can help users navigate the app, access information, and perform tasks using the available tools. Be concise and friendly.";
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading instructions from: {InstructionsPath}", instructionsOrPath);
				return "You are a helpful AI assistant integrated into a mobile application. You can help users navigate the app, access information, and perform tasks using the available tools. Be concise and friendly.";
			}
		}

		// It's a direct string instruction
		return instructionsOrPath;
	}

	private IEnumerable<object> GetNavigationToolDefinitions()
	{
		// Define navigate_to_page function
		yield return new
		{
			type = "function",
			function = new
			{
				name = "navigate_to_page",
				description = "Navigate to a specific page in the mobile application",
				parameters = new
				{
					type = "object",
					properties = new
					{
						page_name = new
						{
							type = "string",
							description = "The name of the page to navigate to. Available pages: DadJokes, Posts, Settings, UserProfile",
							@enum = new[] { "DadJokes", "Posts", "Settings", "UserProfile" }
						},
						clear_stack = new
						{
							type = "boolean",
							description = "Whether to clear the navigation stack before navigating",
							@default = false
						}
					},
					required = new[] { "page_name" }
				}
			}
		};

		// Define get_current_page function
		yield return new
		{
			type = "function",
			function = new
			{
				name = "get_current_page",
				description = "Get information about the current page the user is viewing in the application",
				parameters = new
				{
					type = "object",
					properties = new { }
				}
			}
		};

		// Define go_back function
		yield return new
		{
			type = "function",
			function = new
			{
				name = "go_back",
				description = "Navigate back to the previous page in the navigation stack",
				parameters = new
				{
					type = "object",
					properties = new { }
				}
			}
		};

		// Define open_settings function
		yield return new
		{
			type = "function",
			function = new
			{
				name = "open_settings",
				description = "Open the settings page of the application",
				parameters = new
				{
					type = "object",
					properties = new { }
				}
			}
		};

		// Define logout function
		yield return new
		{
			type = "function",
			function = new
			{
				name = "logout",
				description = "Log out the current user from the application",
				parameters = new
				{
					type = "object",
					properties = new { }
				}
			}
		};
	}

	/// <summary>
	/// Sends a single user message to the configured persistent agent and returns the assistant's reply (last assistant message).
	/// Assumes AgenticConfiguration.AssistantId is set to an existing persistent agent.
	/// </summary>
	/// <summary>
	/// Sends a single user message to the configured persistent agent and returns the assistant's reply (last assistant message).
	/// </summary>
	/// <param name="userMessage">The user's message to send.</param>
	/// <param name="ct">Cancellation token.</param>
	public async Task<ChatMessage> SendMessageToAssistantAsync(string userMessage, CancellationToken ct)
	{
		await EnsurePersistentClientsAsync(ct);

		if (_agentsHttpClient == null)
		{
			throw new InvalidOperationException("HTTP client not initialized.");
		}

		if (string.IsNullOrWhiteSpace(_currentAgentId))
		{
			throw new InvalidOperationException("Agent not initialized. Check configuration and agent creation.");
		}

		_logger.LogInformation("Using persistent agent {AgentId}", _currentAgentId);

		// Create new thread via REST API
		var threadResponse = await _agentsHttpClient.PostAsync($"threads?api-version={ApiVersion}", null, ct);
		threadResponse.EnsureSuccessStatusCode();
		var threadJson = await threadResponse.Content.ReadAsStringAsync(ct);
		var threadData = JsonDocument.Parse(threadJson).RootElement;
		_currentThreadId = threadData.GetProperty("id").GetString();
		_logger.LogInformation("Created thread with id {ThreadId}", _currentThreadId);

		// Post the user message
		var messageData = new { role = "user", content = userMessage };
		var messageJson = JsonSerializer.Serialize(messageData);
		var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
		var messageResponse = await _agentsHttpClient.PostAsync($"threads/{_currentThreadId}/messages?api-version={ApiVersion}", messageContent, ct);
		messageResponse.EnsureSuccessStatusCode();

		// Create a run (execute the agent)
		var runData = new { assistant_id = _currentAgentId };
		var runJson = JsonSerializer.Serialize(runData);
		var runContent = new StringContent(runJson, Encoding.UTF8, "application/json");
		var runResponse = await _agentsHttpClient.PostAsync($"threads/{_currentThreadId}/runs?api-version={ApiVersion}", runContent, ct);
		runResponse.EnsureSuccessStatusCode();
		var runResponseJson = await runResponse.Content.ReadAsStringAsync(ct);
		var runResponseData = JsonDocument.Parse(runResponseJson).RootElement;
		var runId = runResponseData.GetProperty("id").GetString();
		var runStatus = runResponseData.GetProperty("status").GetString();

		// Poll until terminal status or requires_action
		_logger.LogInformation("Polling run status for run {RunId}", runId);
		while (runStatus == "queued" || runStatus == "in_progress" || runStatus == "requires_action")
		{
			if (runStatus == "requires_action")
			{
				// Agent wants to call a tool - handle it
				_logger.LogInformation("Run requires action - processing tool calls");
				
				var statusResponse = await _agentsHttpClient.GetAsync($"threads/{_currentThreadId}/runs/{runId}?api-version={ApiVersion}", ct);
				statusResponse.EnsureSuccessStatusCode();
				var statusJson = await statusResponse.Content.ReadAsStringAsync(ct);
				var statusData = JsonDocument.Parse(statusJson).RootElement;
				
				// Extract tool calls
				var requiredAction = statusData.GetProperty("required_action");
				var submitToolOutputs = requiredAction.GetProperty("submit_tool_outputs");
				var toolCalls = submitToolOutputs.GetProperty("tool_calls");
				
				// Process each tool call
				var toolOutputs = new List<object>();
				foreach (var toolCall in toolCalls.EnumerateArray())
				{
					var toolCallId = toolCall.GetProperty("id").GetString();
					var functionName = toolCall.GetProperty("function").GetProperty("name").GetString();
					var functionArgs = toolCall.GetProperty("function").GetProperty("arguments").GetString();
					
					_logger.LogInformation("Tool call: {FunctionName} with args: {Args}", functionName, functionArgs);
					
					// Execute the tool and get output
					var output = ExecuteNavigationTool(functionName, functionArgs);
					
					toolOutputs.Add(new
					{
						tool_call_id = toolCallId,
						output = output
					});
				}
				
				// Submit tool outputs back to the run
				var toolOutputsData = new { tool_outputs = toolOutputs };
				var toolOutputsJson = JsonSerializer.Serialize(toolOutputsData);
				var toolOutputsContent = new StringContent(toolOutputsJson, Encoding.UTF8, "application/json");
				var submitResponse = await _agentsHttpClient.PostAsync(
					$"threads/{_currentThreadId}/runs/{runId}/submit_tool_outputs?api-version={ApiVersion}",
					toolOutputsContent,
					ct);
				submitResponse.EnsureSuccessStatusCode();
				
				_logger.LogInformation("Submitted {Count} tool outputs", toolOutputs.Count);
				
				// Continue polling
				runStatus = "in_progress";
				continue;
			}
			
			await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
			var statusResponse2 = await _agentsHttpClient.GetAsync($"threads/{_currentThreadId}/runs/{runId}?api-version={ApiVersion}", ct);
			statusResponse2.EnsureSuccessStatusCode();
			var statusJson2 = await statusResponse2.Content.ReadAsStringAsync(ct);
			var statusData2 = JsonDocument.Parse(statusJson2).RootElement;
			runStatus = statusData2.GetProperty("status").GetString();
			_logger.LogDebug("Run status: {Status}", runStatus);
		}

		if (runStatus != "completed")
		{
			_logger.LogWarning("Run ended with status {Status}", runStatus);
			throw new InvalidOperationException($"Run failed or was canceled: {runStatus}");
		}

		// Get messages from thread
		var messagesResponse = await _agentsHttpClient.GetAsync($"threads/{_currentThreadId}/messages?order=asc&api-version={ApiVersion}", ct);
		messagesResponse.EnsureSuccessStatusCode();
		var messagesJson = await messagesResponse.Content.ReadAsStringAsync(ct);
		var messagesData = JsonDocument.Parse(messagesJson).RootElement;

		// Find the last assistant message
		ChatMessage lastAssistantMessage = null;
		if (messagesData.TryGetProperty("data", out var dataArray))
		{
			foreach (var message in dataArray.EnumerateArray())
			{
				if (message.TryGetProperty("role", out var role) && role.GetString() == "assistant")
				{
					if (message.TryGetProperty("content", out var contentArray))
					{
						foreach (var contentItem in contentArray.EnumerateArray())
						{
							if (contentItem.TryGetProperty("text", out var textObj) &&
								textObj.TryGetProperty("value", out var textValue))
							{
								lastAssistantMessage = new ChatMessage
								{
									Role = "assistant",
									Content = textValue.GetString() ?? string.Empty,
									Timestamp = message.TryGetProperty("created_at", out var createdAt)
										? DateTimeOffset.FromUnixTimeSeconds(createdAt.GetInt64()).UtcDateTime
										: DateTime.UtcNow
								};
							}
						}
					}
				}
			}
		}

		return lastAssistantMessage ?? new ChatMessage
		{
			Role = "assistant",
			Content = string.Empty,
			Timestamp = DateTime.UtcNow
		};
	}

	/// <summary>
	/// Resets conversation by creating a new thread via REST API.
	/// </summary>
	/// <param name="ct">Cancellation token.</param>
	public async Task ResetConversationAsync(CancellationToken ct)
	{
		await EnsurePersistentClientsAsync(ct);
		if (_agentsHttpClient == null)
		{
			return;
		}

		// Create a new thread via REST API
		var threadResponse = await _agentsHttpClient.PostAsync($"threads?api-version={ApiVersion}", null, ct);
		threadResponse.EnsureSuccessStatusCode();
		var threadJson = await threadResponse.Content.ReadAsStringAsync(ct);
		var threadData = JsonDocument.Parse(threadJson).RootElement;
		_currentThreadId = threadData.GetProperty("id").GetString();
	}

	/// <summary>
	/// Deletes the current assistant if one exists.
	/// </summary>
	/// <param name="ct">Cancellation token.</param>
	public async Task DeleteAssistantAsync(CancellationToken ct)
	{
		await EnsurePersistentClientsAsync(ct);
		if (_agentsHttpClient == null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(_currentAgentId))
		{
			return;
		}

		// DELETE assistants/{assistant_id} via REST API
		var response = await _agentsHttpClient.DeleteAsync($"assistants/{_currentAgentId}?api-version={ApiVersion}", ct);
		response.EnsureSuccessStatusCode();
		_currentAgentId = null;
	}

	/// <summary>
	/// Executes a navigation tool function and returns the result as a string.
	/// </summary>
	private string ExecuteNavigationTool(string functionName, string functionArgsJson)
	{
		try
		{
			_logger.LogInformation("Executing navigation tool: {FunctionName}", functionName);

			switch (functionName)
			{
				case "navigate_to_page":
					var args = JsonSerializer.Deserialize<JsonElement>(functionArgsJson);
					var pageName = args.GetProperty("page_name").GetString();
					_logger.LogInformation("Navigation requested to page: {PageName}", pageName);
					
					// Raise navigation event
					NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.Page, pageName));
					
					return JsonSerializer.Serialize(new { success = true, message = $"Navigation to {pageName} page has been queued. The app will navigate shortly." });

				case "get_current_page":
					return JsonSerializer.Serialize(new { current_page = "AgenticChatPage", message = "You are currently on the Agentic Chat page." });

				case "go_back":
					_logger.LogInformation("Go back requested");
					
					// Raise navigation event
					NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.Back));
					
					return JsonSerializer.Serialize(new { success = true, message = "Navigation back has been queued." });

				case "open_settings":
					_logger.LogInformation("Open settings requested");
					
					// Raise navigation event
					NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.Settings));
					
					return JsonSerializer.Serialize(new { success = true, message = "Settings page will open shortly." });

				case "logout":
					_logger.LogInformation("Logout requested");
					
					// Raise navigation event
					NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.Logout));
					
					return JsonSerializer.Serialize(new { success = true, message = "Logout has been queued." });

				default:
					_logger.LogWarning("Unknown tool function: {FunctionName}", functionName);
					return JsonSerializer.Serialize(new { success = false, error = $"Unknown function: {functionName}" });
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing navigation tool {FunctionName}", functionName);
			return JsonSerializer.Serialize(new { success = false, error = ex.Message });
		}
	}
}
