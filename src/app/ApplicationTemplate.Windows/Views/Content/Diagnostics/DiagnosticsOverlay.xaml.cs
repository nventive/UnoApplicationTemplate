using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Content;

public sealed partial class DiagnosticsOverlay : UserControl
{
	public DiagnosticsOverlay()
	{
		this.InitializeComponent();
	}

	private void OnThemeButtonClicked(object sender, RoutedEventArgs e)
	{
		// Set theme for window root.
		if (App.Window.Content is FrameworkElement root)
		{
			switch (root.ActualTheme)
			{
				case ElementTheme.Default:
				case ElementTheme.Light:
					root.RequestedTheme = ElementTheme.Dark;
					break;
				case ElementTheme.Dark:
					root.RequestedTheme = ElementTheme.Light;
					break;
			}
		}
	}
}
