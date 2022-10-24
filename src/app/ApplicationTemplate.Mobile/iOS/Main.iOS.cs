using System;
using Chinook.SectionsNavigation;
using UIKit;
using Uno.UI.Controls;

namespace ApplicationTemplate;

public partial class Application
{
	public static void Main(string[] args)
	{
		Microsoft.UI.Xaml.Window.ViewControllerGenerator = GetRootController;

		UIApplication.Main(args, null, typeof(App));
	}

	private static RootViewController GetRootController()
	{
		return new MostPresentedRootViewController();
	}
}
