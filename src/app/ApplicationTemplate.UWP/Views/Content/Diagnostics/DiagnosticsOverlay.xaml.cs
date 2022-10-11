#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

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
		if (Window.Current.Content is FrameworkElement root)
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
