using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
//-:cnd:noEmit
#if WINUI
using Microsoft.UI.Xaml;
using Application = Microsoft.UI.Xaml.Application;
using CurrentWindow = Microsoft.UI.Xaml.Window;
#else
using Application = Windows.UI.Xaml.Application;
using CurrentWindow = Windows.UI.Xaml.Window.Current;
using Windows.UI.Xaml;
#endif
//-:cnd:noEmit
namespace ApplicationTemplate;

sealed partial class App : Application
{
	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();
		//-:cnd:noEmit
#if !WINUI
		InitializeComponent();
#endif
		//-:cnd:noEmit
		ConfigureOrientation();
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow => CurrentWindow;

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeAndStart(args);
	}

	protected override void OnActivated(IActivatedEventArgs args)
	{
		// This is where your app launches if you use custom schemes, Universal Links, or Android App Links.
		InitializeAndStart(args);
	}

	private void InitializeAndStart(IActivatedEventArgs args)
	{
		Shell = CurrentWindow.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureViewSize();
#if !WINUI
			ConfigureStatusBar();
#endif
			Startup.Initialize(GetContentRootPath(), GetSettingsFolderPath(), LoggingConfiguration.ConfigureLogging);

			Startup.ShellActivity.Start();

			CurrentWindow.Content = Shell = new Shell(args);

			Startup.ShellActivity.Stop();
		}

		CurrentWindow.Activate();

		_ = Task.Run(() => Startup.Start());
	}

	private static string GetContentRootPath()
	{
		//-:cnd:noEmit
#if WINDOWS_UWP || __ANDROID__ || __IOS__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return string.Empty;
#endif
		//+:cnd:noEmit
	}

	private static string GetSettingsFolderPath()
	{
		//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
		var folderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path; // TODO: Tests can use that?
//-:cnd:noEmit
#elif __ANDROID__ || __IOS__
//+:cnd:noEmit
		var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//-:cnd:noEmit
#else
		//+:cnd:noEmit
		var folderPath = string.Empty;
		//-:cnd:noEmit
#endif
		//+:cnd:noEmit

		return folderPath;
	}

	private void ConfigureOrientation()
	{
		DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
	}

	private void ConfigureViewSize()
	{
		//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
		ApplicationView.PreferredLaunchViewSize = new Size(480, 800);
		ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
		ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 480));
//-:cnd:noEmit
#endif
		//+:cnd:noEmit
	}
	//-:cnd:noEmit
#if !WINUI
	private void ConfigureStatusBar()
	{
		var resources = Windows.UI.Xaml.Application.Current.Resources;

		//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
		var hasStatusBar = false;
//-:cnd:noEmit
#else
		//+:cnd:noEmit
		var hasStatusBar = true;
		Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Windows.UI.Colors.White;
		//-:cnd:noEmit
#endif
		//+:cnd:noEmit

		var statusBarHeight = hasStatusBar ? Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height : 0;

		resources.Add("StatusBarDouble", (double)statusBarHeight);
		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
	}
#endif
	//-:cnd:noEmit
}
