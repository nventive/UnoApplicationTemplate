// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.iOS.cs
#if __IOS__
using System;
using System.Threading;
using System.Threading.Tasks;
using CPS.DataAccess;
using Foundation;
using Microsoft.UI.Dispatching;
using SafariServices;
using UIKit;


namespace ApplicationTemplate.Views.PlatformServices;


public class EmbeddedBrowserService : IEmbeddedBrowserService
{
    private readonly DispatcherQueue _dispatcherQueue;


    public EmbeddedBrowserService(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
    }


    public async Task NavigateTo(CancellationToken ct, Uri uri)
    {
        await Task.Run(() =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                var safariViewController = new SFSafariViewController(new NSUrl(uri.ToString()));
                var viewController = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                
                if (viewController != null)
                {
                    viewController.PresentViewController(safariViewController, true, null);
                }
            });
        });
    }
}
#endif
