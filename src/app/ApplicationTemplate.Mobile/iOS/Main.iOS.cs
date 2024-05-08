using Chinook.SectionsNavigation;
using DynamicData;
using UIKit;
using Uno.UI.Controls;
using Windows.System;
using IntercomBinding = Binding.Intercom.iOS.Intercom;

namespace ApplicationTemplate;

public sealed class EntryPoint
{
	// This is the main entry point of the application.
	private static void Main(string[] args)
	{
		// This is required in order to show native popups (via MessageDialogService) from native modals (via FrameSectionsNavigator).
		Microsoft.UI.Xaml.Window.ViewControllerGenerator = GetRootController;

		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(App));
	}

	private static RootViewController GetRootController()
	{
		var controller = new MostPresentedRootViewController();

		return controller;
	}

	public static void LaunchIntercom()
	{
		IntercomBinding.SetApiKey("ios_sdk-4ce1e9561a4dddba8c774ef03bbcc75ca7307a8a", "ios_sdk -d4f143520892a443ead648ef92efb89b3a0d15c6");
		IntercomBinding.SetInAppMessagesVisible(false);
		var attributes = new Binding.Intercom.iOS.ICMUserAttributes() { Email = "bina80@cma.ca", Name = "Mrs. YOSSRA CLUNAS", UserId = "dcd50c9e-db98-4129-a564-7d244bcae4da" };
		attributes.CustomAttributes = new Foundation.NSDictionary<Foundation.NSString, Foundation.NSObject>(new Foundation.NSString("is_authenticated"), new Foundation.NSNumber(true));
		IntercomBinding.LoginUserWithUserAttributes(attributes, null, null);
		IntercomBinding.PresentMessageComposer("Hello default message");
	}
}
