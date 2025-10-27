using System;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation.ViewModels.Agentic;

/// <summary>
/// ViewModel for the drawing modal that displays SVG content.
/// </summary>
public class DrawingModalViewModel : ViewModel
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DrawingModalViewModel"/> class.
	/// </summary>
	public DrawingModalViewModel()
	{
	}

	/// <summary>
	/// Gets or sets the SVG content to display.
	/// </summary>
	public string SvgContent
	{
		get => this.Get<string>();
		set
		{
			this.Set(value);
			// Regenerate full markup when content changes
			GenerateFullSvgMarkup();
		}
	}

	/// <summary>
	/// Gets or sets the title of the drawing.
	/// </summary>
	public string Title
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets or sets the description or caption of the drawing.
	/// </summary>
	public string Description
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	/// <summary>
	/// Gets the full SVG markup (auto-generated from SvgContent).
	/// </summary>
	public string FullSvgMarkup
	{
		get => this.Get<string>();
		private set => this.Set(value);
	}

	/// <summary>
	/// Command to close the modal.
	/// </summary>
	public IDynamicCommand CloseCommand => this.GetCommandFromTask(async ct =>
	{
		// Navigate back to close the modal
		var sectionsNavigator = this.GetService<ISectionsNavigator>();
		await sectionsNavigator.NavigateBackOrCloseModal(ct);
	});

	private void GenerateFullSvgMarkup()
	{
		if (string.IsNullOrWhiteSpace(SvgContent))
		{
			FullSvgMarkup = string.Empty;
			return;
		}

		// If content already contains <svg>, use it as-is
		if (SvgContent.TrimStart().StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
		{
			FullSvgMarkup = SvgContent;
			return;
		}

		// Otherwise, wrap path commands in SVG markup
		FullSvgMarkup = $@"<svg viewBox=""0 0 500 500"" xmlns=""http://www.w3.org/2000/svg"">
	{SvgContent}
</svg>";
	}
}
