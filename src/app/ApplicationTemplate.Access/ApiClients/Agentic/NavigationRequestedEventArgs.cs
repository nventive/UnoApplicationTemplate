using System;

namespace ApplicationTemplate.DataAccess.ApiClients.Agentic;

/// <summary>
/// Event arguments for navigation requests from the AI agent.
/// </summary>
public class NavigationRequestedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="NavigationRequestedEventArgs"/> class.
	/// </summary>
	/// <param name="navigationType">The type of navigation requested.</param>
	/// <param name="pageName">The name of the page to navigate to (for Page navigation type).</param>
	/// <param name="svgContent">The SVG content to display (for DrawContent navigation type).</param>
	/// <param name="title">The title for the drawing modal (for DrawContent navigation type).</param>
	/// <param name="description">The description for the drawing modal (for DrawContent navigation type).</param>
	public NavigationRequestedEventArgs(
		NavigationType navigationType,
		string pageName = "",
		string svgContent = "",
		string title = "",
		string description = "")
	{
		NavigationType = navigationType;
		PageName = pageName;
		SvgContent = svgContent;
		Title = title;
		Description = description;
	}

	/// <summary>
	/// Gets the type of navigation requested.
	/// </summary>
	public NavigationType NavigationType { get; }

	/// <summary>
	/// Gets the name of the page to navigate to (if applicable).
	/// </summary>
	public string PageName { get; }

	/// <summary>
	/// Gets the SVG content to display (for DrawContent navigation type).
	/// </summary>
	public string SvgContent { get; }

	/// <summary>
	/// Gets the title for the drawing modal (for DrawContent navigation type).
	/// </summary>
	public string Title { get; }

	/// <summary>
	/// Gets the description for the drawing modal (for DrawContent navigation type).
	/// </summary>
	public string Description { get; }
}

/// <summary>
/// Types of navigation actions the AI agent can request.
/// </summary>
public enum NavigationType
{
	/// <summary>
	/// Navigate to a specific page by name.
	/// </summary>
	Page,

	/// <summary>
	/// Navigate back to the previous page.
	/// </summary>
	Back,

	/// <summary>
	/// Open the settings page.
	/// </summary>
	Settings,

	/// <summary>
	/// Log out the user.
	/// </summary>
	Logout,

	/// <summary>
	/// Display drawing content in a modal.
	/// </summary>
	DrawContent
}
