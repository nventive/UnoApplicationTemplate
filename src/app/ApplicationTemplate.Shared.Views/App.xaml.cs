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
			ConfigureViewSize();
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
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return string.Empty;
#endif
//+:cnd:noEmit
	}

	private static string GetSettingsFolderPath()
	{
		var folderPath = string.Empty;

//-:cnd:noEmit
#if WINDOWS
		folderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path; // TODO: Tests can use that?
#else
		folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#endif
//+:cnd:noEmit

		return folderPath;
	}

	private static void ConfigureOrientation()
	{
		DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
	}

	private static void ConfigureStatusBar()
	{
		var resources = Current.Resources;
		var statusBarHeight = 0d;

//-:cnd:noEmit
#if __ANDROID__ || __IOS__
		Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Microsoft.UI.Colors.White;
		statusBarHeight = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
#endif
//+:cnd:noEmit

		resources.Add("StatusBarDouble", statusBarHeight);
		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
	}

	private void ConfigureViewSize()
	{
//-:cnd:noEmit
#if WINDOWS
		var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(CurrentWindow);
		var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
		var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
		appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 480, Height = 800 });
#endif
//+:cnd:noEmit
	}
}
