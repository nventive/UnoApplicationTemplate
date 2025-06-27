#if __IOS__
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Microsoft.UI.Dispatching;
using SafariServices;
using UIKit;

namespace CPS.DataAccess;

public sealed class EmbeddedBrowserService : IEmbeddedBrowserService
{
	private readonly DispatcherQueue _dispatcherQueue;

	public EmbeddedBrowserService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue;
	}

	public async Task NavigateTo(CancellationToken ct, Uri uri)
	{
		ArgumentNullException.ThrowIfNull(uri, nameof(uri));

		_dispatcherQueue.TryEnqueue(() =>
		{
			var safariViewController = new SFSafariViewController(new NSUrl(uri.ToString()));

			var windowScene = UIApplication.SharedApplication.ConnectedScenes
				.OfType<UIWindowScene>()
				.FirstOrDefault();

			var viewController = windowScene?.Windows.FirstOrDefault()?.RootViewController;

			viewController?.PresentViewController(safariViewController, true, null);
		});
	}
}
#endif
