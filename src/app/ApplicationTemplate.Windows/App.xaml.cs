using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics.Display;
using Windows.Storage;
using WinRT.Interop;

namespace ApplicationTemplate;

sealed partial class App : Application
{
	private Window _window;

	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();

		this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
#endif

		ConfigureOrientation();
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow => _window;

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void InitializeAndStart()
	{
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
		_window = new Window();
		_window.Activate();
#else
        _window = Microsoft.UI.Xaml.Window.Current;
#endif

		Shell = CurrentWindow.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureStatusBar();

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
		var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this)));

		//-:cnd:noEmit
#if WINDOWS
		//+:cnd:noEmit
		var hasStatusBar = false;
		//-:cnd:noEmit
#else
		//+:cnd:noEmit
		var hasStatusBar = true;
		appWindow.TitleBar.ForegroundColor = Colors.White;
		//-:cnd:noEmit
#endif
		//+:cnd:noEmit

		var statusBarHeight = hasStatusBar ? appWindow.TitleBar.Height : 0;

		resources.Add("StatusBarDouble", (double)statusBarHeight);
		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
	}
}
