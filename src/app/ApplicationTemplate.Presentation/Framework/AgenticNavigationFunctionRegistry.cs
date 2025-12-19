using System;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business.Agentic;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation.Framework;

/// <summary>
/// Registers navigation-related Agentic functions.
/// This class bridges the Business layer (AgenticToolExecutor) with the Presentation layer (Navigation).
/// </summary>
public class AgenticNavigationFunctionRegistry
{
	private readonly IAgenticToolExecutor _toolExecutor;
	private readonly ISectionsNavigator _sectionsNavigator;
	private readonly ILogger<AgenticNavigationFunctionRegistry> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="AgenticNavigationFunctionRegistry"/> class.
	/// </summary>
	/// <param name="toolExecutor">The Agentic tool executor.</param>
	/// <param name="sectionsNavigator">The sections navigator.</param>
	/// <param name="logger">The logger.</param>
	public AgenticNavigationFunctionRegistry(
		IAgenticToolExecutor toolExecutor,
		ISectionsNavigator sectionsNavigator,
		ILogger<AgenticNavigationFunctionRegistry> logger)
	{
		_toolExecutor = toolExecutor ?? throw new ArgumentNullException(nameof(toolExecutor));
		_sectionsNavigator = sectionsNavigator ?? throw new ArgumentNullException(nameof(sectionsNavigator));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <summary>
	/// Registers all navigation-related functions with the AI agent tool executor.
	/// Call this method during app startup after services are configured.
	/// </summary>
	public void RegisterFunctions()
	{
		_toolExecutor.RegisterFunctionHandler("navigate_to_page", NavigateToPageAsync);
		_toolExecutor.RegisterFunctionHandler("get_current_page", GetCurrentPageAsync);
		_toolExecutor.RegisterFunctionHandler("go_back", GoBackAsync);
		_toolExecutor.RegisterFunctionHandler("open_settings", OpenSettingsAsync);

		_logger.LogInformation("Registered {Count} navigation functions with AI agent", 4);
	}

	private async Task<string> NavigateToPageAsync(JsonDocument args, CancellationToken ct)
	{
		try
		{
			// Extract page name (required)
			if (!args.RootElement.TryGetProperty("pageName", out var pageNameElement))
			{
				return JsonSerializer.Serialize(new
				{
					success = false,
					error = "Missing required parameter: pageName. Available pages: DadJokes, Posts, Settings, UserProfile"
				});
			}

			var pageName = pageNameElement.GetString();
			if (string.IsNullOrWhiteSpace(pageName))
			{
				return JsonSerializer.Serialize(new
				{
					success = false,
					error = "pageName cannot be empty"
				});
			}

			// Extract optional parameters
			var clearStack = false;
			if (args.RootElement.TryGetProperty("clearStack", out var clearStackElement))
			{
				clearStack = clearStackElement.GetBoolean();
			}

			// Convert page name to section name (if applicable)
			var normalizedPageName = pageName.Replace(" ", string.Empty, StringComparison.Ordinal).ToLowerInvariant();

			switch (normalizedPageName)
			{
				case "dadjokes":
				case "jokes":
				case "home":
					await _sectionsNavigator.SetActiveSection(ct, "Home");
					return JsonSerializer.Serialize(new
					{
						success = true,
						message = "Navigated to DadJokes page",
						page_name = pageName
					});

				case "posts":
					// Example: You would implement this based on your navigation structure
					// await _sectionsNavigator.Navigate(ct, () => new PostsPageViewModel());
					return JsonSerializer.Serialize(new
					{
						success = false,
						error = "Posts page navigation not yet implemented in your navigation structure"
					});

				case "settings":
					// Example: Navigate to settings
					// await _sectionsNavigator.Navigate(ct, () => new SettingsPageViewModel());
					return JsonSerializer.Serialize(new
					{
						success = false,
						error = "Settings navigation requires ViewModel instantiation. Update this method to include your SettingsPageViewModel."
					});

				case "userprofile":
				case "profile":
					// Example: Navigate to user profile
					return JsonSerializer.Serialize(new
					{
						success = false,
						error = "UserProfile page navigation not yet implemented. Update this method to include your UserProfilePageViewModel."
					});

				default:
					return JsonSerializer.Serialize(new
					{
						success = false,
						error = $"Unknown page name: {pageName}. Available pages: DadJokes, Posts, Settings, UserProfile"
					});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in NavigateToPageAsync");
			return JsonSerializer.Serialize(new { success = false, error = $"Navigation error: {ex.Message}" });
		}
	}

	private async Task<string> GetCurrentPageAsync(JsonDocument args, CancellationToken ct)
	{
		try
		{
			await Task.CompletedTask; // For async consistency

			var activeViewModel = _sectionsNavigator.GetActiveViewModel();
			var activeStackNavigator = _sectionsNavigator.GetActiveStackNavigator();

			var pageName = activeViewModel?.GetType().Name ?? "Unknown";
			
			// Find the active section name by checking which section contains the active stack navigator
			var sectionName = "Unknown";
			foreach (var section in _sectionsNavigator.State.Sections)
			{
				if (section.Value == activeStackNavigator)
				{
					sectionName = section.Key;
					break;
				}
			}

			var stackDepth = activeStackNavigator?.State.Stack.Count ?? 0;

			return JsonSerializer.Serialize(new
			{
				success = true,
				current_page = pageName,
				current_section = sectionName,
				stack_depth = stackDepth,
				timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in GetCurrentPageAsync");
			return JsonSerializer.Serialize(new { success = false, error = $"Error getting current page: {ex.Message}" });
		}
	}

	private async Task<string> GoBackAsync(JsonDocument args, CancellationToken ct)
	{
		try
		{
			var activeStackNavigator = _sectionsNavigator.GetActiveStackNavigator();

			if (activeStackNavigator == null)
			{
				return JsonSerializer.Serialize(new { success = false, error = "No active stack navigator" });
			}

			if (activeStackNavigator.State.Stack.Count <= 1)
			{
				return JsonSerializer.Serialize(new { success = false, error = "Cannot go back, already at root page" });
			}

			await activeStackNavigator.NavigateBack(ct);

			return JsonSerializer.Serialize(new { success = true, message = "Navigated back successfully" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in GoBackAsync");
			return JsonSerializer.Serialize(new { success = false, error = $"Back navigation error: {ex.Message}" });
		}
	}

	private async Task<string> OpenSettingsAsync(JsonDocument args, CancellationToken ct)
	{
		try
		{
			// In a real implementation, you would navigate to the settings page
			// Example: await _sectionsNavigator.Navigate(ct, () => new SettingsPageViewModel());

			await Task.CompletedTask; // For async consistency

			return JsonSerializer.Serialize(new
			{
				success = false,
				error = "Settings page navigation requires ViewModel factory implementation. Update OpenSettingsAsync in AgenticNavigationFunctionRegistry.cs"
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in OpenSettingsAsync");
			return JsonSerializer.Serialize(new { success = false, error = $"Settings navigation error: {ex.Message}" });
		}
	}
}
