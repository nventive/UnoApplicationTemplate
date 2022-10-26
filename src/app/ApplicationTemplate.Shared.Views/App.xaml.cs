using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.ViewManagement;
#if WINDOWS
using WinRT.Interop;
#endif

namespace ApplicationTemplate;

public sealed partial class App : Microsoft.UI.Xaml.Application
{
	private static Window m_window;

	public static Window Window
	{
		get { return m_window; }
	}

	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();

		this.InitializeComponent();

#if HAS_UNO
		this.Suspending += OnSuspending;
#endif

		ConfigureOrientation();
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	/// <summary>
	/// Invoked when application execution is being suspended.  Application state is saved
	/// without knowing whether the application will be terminated or resumed with the contents
	/// of memory still intact.
	/// </summary>
	/// <param name="sender">The source of the suspend request.</param>
	/// <param name="e">Details about the suspend request.</param>
	private void OnSuspending(object sender, SuspendingEventArgs e)
	{
		var deferral = e.SuspendingOperation.GetDeferral();
		//TODO: Save application state and stop any background activity
		deferral.Complete();
	}

	private void InitializeAndStart()
	{
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
		m_window = new Window();
		m_window.Activate();
#else
		m_window = Microsoft.UI.Xaml.Window.Current;
#endif

		Shell = m_window.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureStatusBar();

			Startup.Initialize(GetContentRootPath(), GetSettingsFolderPath(), LoggingConfiguration.ConfigureLogging);

			Startup.ShellActivity.Start();

			m_window.Content = Shell = new Shell();

			Startup.ShellActivity.Stop();
		}

		m_window.Activate();

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
////		var resources = Instance.Resources;

////		//-:cnd:noEmit
////#if WINDOWS
////		//+:cnd:noEmit

////		var hasStatusBar = false;
////		//-:cnd:noEmit
////#else
////		//+:cnd:noEmit

////		var hasStatusBar = true;
////		Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Colors.White;

////		var statusBarHeight = hasStatusBar ? Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height : 0;

////		resources.Add("StatusBarDouble", (double)statusBarHeight);
////		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
////		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
////		//-:cnd:noEmit
////#endif
////		//+:cnd:noEmit
	}
}
