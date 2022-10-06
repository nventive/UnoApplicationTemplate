using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using LaunchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;

namespace ApplicationTemplate;

sealed partial class App : Application
{
	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();

		//InitializeComponent();

		ConfigureOrientation();
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow => Window.Current;

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
		Shell = CurrentWindow.Content;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureViewSize();
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
		//+:cnd:noEmit
	}

	private void ConfigureStatusBar()
	{
		var resources = global::Microsoft.UI.Xaml.Application.Current.Resources;

		resources.Add("StatusBarDouble", 0);
		resources.Add("StatusBarThickness", new Thickness(0, 0, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(0, GridUnitType.Pixel));
	}
}
