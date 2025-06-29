using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml.Controls;
using Nventive.ExtendedSplashScreen;

namespace ApplicationTemplate;

public sealed partial class Shell : UserControl
{
	public Shell()
	{
		this.InitializeComponent();

		Instance = this;
	}

	public static Shell Instance { get; private set; }

	public IExtendedSplashScreen ExtendedSplashScreen => this.AppExtendedSplashScreen;

	public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;

	public Panel Panel => this.MyRootPanel;
}
