using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace ApplicationTemplate;

sealed partial class App : Application
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

	public Window CurrentWindow => Window.Current;

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		InitializeAndStart(args.UWPLaunchActivatedEventArgs);
	}

	private void InitializeAndStart(IActivatedEventArgs args)
	{
		Shell = CurrentWindow.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureStatusBar();

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
#if WINDOWS || __ANDROID__ || __IOS__
		return ApplicationData.Current.LocalFolder.Path;
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
		var folderPath = ApplicationData.Current.LocalFolder.Path; // TODO: Tests can use that?
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

	private void ConfigureStatusBar()
	{
		var resources = Application.Current.Resources;

		//-:cnd:noEmit
#if WINDOWS
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

		// TODO Add titlebar / status bar customization for net6
	}
}
