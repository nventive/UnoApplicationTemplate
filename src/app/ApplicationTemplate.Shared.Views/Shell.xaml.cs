using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml.Controls;
#if false
using Nventive.ExtendedSplashScreen;
#endif
using Windows.ApplicationModel.Activation;

namespace ApplicationTemplate;

public sealed partial class Shell : UserControl
{
	public Shell(IActivatedEventArgs e)
	{
		this.InitializeComponent();

		Instance = this;

#if false
		//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
		AppExtendedSplashScreen.SplashScreen = e?.SplashScreen;
//-:cnd:noEmit
#endif
		//+:cnd:noEmit
#endif
	}

	public static Shell Instance { get; private set; }

#if false
	public IExtendedSplashScreen ExtendedSplashScreen => this.AppExtendedSplashScreen;

	public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;
#endif
}
