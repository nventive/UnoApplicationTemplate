using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Content;

public sealed partial class DiagnosticsOverlay : UserControl
{
	public DiagnosticsOverlay()
	{
		this.InitializeComponent();
	}

	private void OnThemeButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
	{
		// Set theme for window root.
		if (Windows.UI.Xaml.Window.Current.Content is FrameworkElement root)
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
