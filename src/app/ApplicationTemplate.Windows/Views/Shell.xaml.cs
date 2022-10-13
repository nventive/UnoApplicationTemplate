using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml.Controls;
using Nventive.ExtendedSplashScreen;
using Windows.ApplicationModel.Activation;

namespace ApplicationTemplate;

public sealed partial class Shell : UserControl
{
	public Shell(IActivatedEventArgs e)
	{
		this.InitializeComponent();

		Instance = this;
	}

	public static Shell Instance { get; private set; }

	public IExtendedSplashScreen ExtendedSplashScreen => this.AppExtendedSplashScreen;

	public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;
}
