using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Uno.Extensions;
using Uno.UI;
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

//-:cnd:noEmit
#if __MOBILE__
		LeavingBackground += OnLeavingBackground;
		Resuming += OnResuming;
		Suspending += OnSuspending;
#endif
//+:cnd:noEmit
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void InitializeAndStart()
	{
//-:cnd:noEmit
#if  WINDOWS10_0_18362_0_OR_GREATER
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
			ConfigureWindow();
			ConfigureStatusBar();

			Startup.Initialize(GetContentRootPath(), GetSettingsFolderPath(), LoggingConfiguration.ConfigureLogging);

			Startup.ShellActivity.Start();

			CurrentWindow.Content = Shell = new Shell();

			Startup.ShellActivity.Stop();
		}

//-:cnd:noEmit
#if __MOBILE__
		CurrentWindow.Activate();
#endif
//+:cnd:noEmit

//-:cnd:noEmit
#if DEBUG
		CurrentWindow.EnableHotReload();
#endif
//+:cnd:noEmit

		_ = Task.Run(() => Startup.Start());
	}

//-:cnd:noEmit
#if __MOBILE__
	/// <summary>
	/// This is where your app launches if you use custom schemes, Universal Links, or Android App Links.
	/// </summary>
	/// <param name="args"><see cref="Windows.ApplicationModel.Activation.IActivatedEventArgs"/>.</param>
	protected override void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void OnLeavingBackground(object sender, Windows.ApplicationModel.LeavingBackgroundEventArgs e)
	{
		this.Log().LogInformation("Application is leaving background.");
	}

	private void OnResuming(object sender, object e)
	{
		this.Log().LogInformation("Application is resuming.");
	}

	private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
	{
		this.Log().LogInformation("Application is suspending.");
	}
#endif
//+:cnd:noEmit

	private static string GetContentRootPath()
	{
//-:cnd:noEmit
#if  WINDOWS10_0_18362_0_OR_GREATER || __MOBILE__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return string.Empty;
#endif
//+:cnd:noEmit
	}

	private static string GetSettingsFolderPath()
	{
//-:cnd:noEmit
#if  WINDOWS10_0_18362_0_OR_GREATER
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#endif
//+:cnd:noEmit
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
#if __MOBILE__
		Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Microsoft.UI.Colors.White;
		statusBarHeight = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
#endif
//+:cnd:noEmit

		resources.Add("StatusBarDouble", statusBarHeight);
		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
	}

	private void ConfigureWindow()
	{
//-:cnd:noEmit
#if  WINDOWS10_0_18362_0_OR_GREATER
		var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(CurrentWindow);
		var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
		var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
		appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 480, Height = 800 });

		// Sets a title bar icon and title.
		// Workaround. See https://github.com/microsoft/microsoft-ui-xaml/issues/6773 for more details.
		appWindow.SetIcon("Images\\TitleBarIcon.ico");
		appWindow.Title = "ApplicationTemplate";
#endif
//+:cnd:noEmit
	}
}
