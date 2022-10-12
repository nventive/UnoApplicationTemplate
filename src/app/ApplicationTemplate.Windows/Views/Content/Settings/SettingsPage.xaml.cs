using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Content;

public sealed partial class SettingsPage : Page
{
	public SettingsPage()
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
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
//+:cnd:noEmit
					// No need for the dispatcher here
					Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Windows.UI.Colors.Black;
//-:cnd:noEmit
#endif
//+:cnd:noEmit
					break;
				case ElementTheme.Dark:
					root.RequestedTheme = ElementTheme.Light;
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
//+:cnd:noEmit
					// No need for the dispatcher here
					Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Windows.UI.Colors.White;
//-:cnd:noEmit
#endif
//+:cnd:noEmit
					break;
			}
		}
	}
}
