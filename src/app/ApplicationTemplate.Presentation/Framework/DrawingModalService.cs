using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation.ViewModels.Agentic;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation.Framework;

/// <summary>
/// Service for displaying drawing content in a modal.
/// </summary>
public class DrawingModalService : Business.Agentic.IDrawingModalService
{
	private readonly ISectionsNavigator _sectionsNavigator;
	private readonly ILogger<DrawingModalService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="DrawingModalService"/> class.
	/// </summary>
	/// <param name="sectionsNavigator">The sections navigator.</param>
	/// <param name="logger">The logger.</param>
	public DrawingModalService(
		ISectionsNavigator sectionsNavigator,
		ILogger<DrawingModalService> logger)
	{
		_sectionsNavigator = sectionsNavigator ?? throw new ArgumentNullException(nameof(sectionsNavigator));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <inheritdoc/>
	public async Task ShowDrawingAsync(string svgContent, string title, string description = null, CancellationToken ct = default)
	{
		try
		{
			_logger.LogInformation("Showing drawing modal: {Title}", title);

			// Create and configure ViewModel
			var viewModel = new DrawingModalViewModel
			{
				SvgContent = svgContent,
				Title = title ?? "Drawing",
				Description = description
			};

			// Navigate to the drawing modal page using the sections navigator
			await _sectionsNavigator.Navigate(ct, () => viewModel);

			_logger.LogInformation("Drawing modal displayed successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error showing drawing modal");
			throw;
		}
	}
}
