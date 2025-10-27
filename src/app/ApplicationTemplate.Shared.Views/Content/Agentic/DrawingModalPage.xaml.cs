using System;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation.ViewModels.Agentic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Content;

/// <summary>
/// Page that displays SVG drawing content in a modal.
/// </summary>
public sealed partial class DrawingModalPage : Page
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DrawingModalPage"/> class.
	/// </summary>
	public DrawingModalPage()
	{
		this.InitializeComponent();
		this.Loaded += OnPageLoaded;
	}

	private void OnPageLoaded(object sender, RoutedEventArgs e)
	{
		if (DataContext is DrawingModalViewModel viewModel)
		{
			// Load SVG content into WebView2
			if (!string.IsNullOrWhiteSpace(viewModel.FullSvgMarkup))
			{
				_ = LoadSvgContent(viewModel.FullSvgMarkup);
			}
		}
	}

	private async Task LoadSvgContent(string svgMarkup)
	{
		try
		{
			// Ensure WebView2 is initialized
			await SvgWebView.EnsureCoreWebView2Async();

			// Create HTML wrapper for SVG
			var html = $@"
<!DOCTYPE html>
<html>
<head>
	<meta charset='utf-8'>
	<style>
		body {{
			margin: 0;
			padding: 0;
			display: flex;
			justify-content: center;
			align-items: center;
			min-height: 100vh;
			background: white;
		}}
		svg {{
			max-width: 100%;
			max-height: 100vh;
			display: block;
		}}
	</style>
</head>
<body>
	{svgMarkup}
</body>
</html>";

			// Navigate to the HTML content
			SvgWebView.NavigateToString(html);
		}
		catch (Exception ex)
		{
			// Log error
			System.Diagnostics.Debug.WriteLine($"Error loading SVG content: {ex.Message}");
		}
	}
}
