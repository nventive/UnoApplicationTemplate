using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nventive.ExtendedSplashScreen;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ApplicationTemplate
{
	public sealed partial class Shell : UserControl
	{
		public Shell(LaunchActivatedEventArgs e)
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
}
