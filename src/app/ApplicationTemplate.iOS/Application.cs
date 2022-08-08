using System;
using Chinook.SectionsNavigation;
using UIKit;
using Uno.UI.Controls;

namespace ApplicationTemplate;

public class Application
{
	public static void Main(string[] args)
	{
		Windows.UI.Xaml.Window.ViewControllerGenerator = GetRootController;

		UIApplication.Main(args, null, typeof(App));
	}

	private static RootViewController GetRootController()
	{
		return new MostPresentedRootViewController();
	}
}
