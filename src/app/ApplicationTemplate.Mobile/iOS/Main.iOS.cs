using System;
using System.Threading;
using Chinook.SectionsNavigation;
using UIKit;
using Uno.UI.Controls;

namespace ApplicationTemplate;

public sealed class EntryPoint
{
	// This is the main entry point of the application.
	private static void Main(string[] args)
	{
		//Thread.Sleep(TimeSpan.FromSeconds(10));

		// This is required in order to show native popups (via MessageDialogService) from native modals (via FrameSectionsNavigator).
		Microsoft.UI.Xaml.Window.ViewControllerGenerator = GetRootController;

		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(App));
	}

	private static RootViewController GetRootController()
	{
		return new MostPresentedRootViewController();
	}
}
