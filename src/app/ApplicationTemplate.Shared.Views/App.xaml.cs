using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml;
using Windows.Graphics.Display;

namespace ApplicationTemplate;

public sealed partial class App : Application
{
	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();

		InitializeComponent();

		ConfigureOrientation();
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
//-:cnd:noEmit
#if WINDOWS
		CurrentWindow = new Window();
		CurrentWindow.Activate();
#else
		CurrentWindow = Microsoft.UI.Xaml.Window.Current;
#endif
//+:cnd:noEmit

		Shell = CurrentWindow.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			/*
			ConfigureViewSize();
			ConfigureStatusBar();
			*/
			Startup.Initialize(GetContentRootPath(), GetSettingsFolderPath(), LoggingConfiguration.ConfigureLogging);

			Startup.ShellActivity.Start();

			CurrentWindow.Content = Shell = new Shell();

			Startup.ShellActivity.Stop();
		}

		CurrentWindow.Activate();

		_ = Task.Run(() => Startup.Start());
	}

	private static string GetContentRootPath()
	{
//-:cnd:noEmit
#if WINDOWS || __ANDROID__ || __IOS__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return string.Empty;
#endif
//+:cnd:noEmit
	}

	private static string GetSettingsFolderPath()
	{
//-:cnd:noEmit
#if WINDOWS
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

#if false
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
}
