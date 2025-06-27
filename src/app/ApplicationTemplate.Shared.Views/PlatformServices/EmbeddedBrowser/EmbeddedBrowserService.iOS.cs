// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.iOS.cs
#if __IOS__
using System;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using SafariServices;
using UIKit;

namespace ApplicationTemplate.Views.PlatformServices.EmbeddedBrowser
{
	public partial class EmbeddedBrowserService
	{
		public override async Task NavigateTo(CancellationToken ct, Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			ct.ThrowIfCancellationRequested();

			try
			{
				// Use SFSafariViewController for an in-app embedded browser.
				var safariViewController = new SFSafariViewController(new NSUrl(uri.ToString()));
				var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
				await topController.PresentViewControllerAsync(safariViewController, true);
			}
			catch (Exception ex)
			{
				// Fallback or handle errors (e.g., invalid URI or presentation failure).
				// Optionally log: this.GetService<ILogger>().LogError(ex, "Failed to launch embedded browser.");
				throw;  // Or provide a user-friendly message via IMessageDialogService.
			}
		}
	}
}
#endif
