using Chinook.SectionsNavigation;
using Nventive.ExtendedSplashScreen;
using Windows.ApplicationModel.Activation;
//-:cnd:noEmit
# WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif
//-:cnd:noEmit
namespace ApplicationTemplate;

public sealed partial class Shell : UserControl
{
	public Shell(IActivatedEventArgs e)
	{
		this.InitializeComponent();

		Instance = this;

//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
		AppExtendedSplashScreen.SplashScreen = e?.SplashScreen;
//-:cnd:noEmit
#endif
//+:cnd:noEmit
	}

	public static Shell Instance { get; private set; }

	public IExtendedSplashScreen ExtendedSplashScreen => this.AppExtendedSplashScreen;

	public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;
}
