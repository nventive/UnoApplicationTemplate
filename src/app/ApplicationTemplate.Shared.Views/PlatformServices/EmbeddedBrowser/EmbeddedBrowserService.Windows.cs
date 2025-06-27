#if __WINDOWS__
using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

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

		// For Windows, we'll use the system default browser.
		// In a real implementation, you might want to create a WebView2 control.
		await _dispatcherQueue.EnqueueAsync(async () =>
		{
			await Windows.System.Launcher.LaunchUriAsync(uri);
		});
	}
}
#endif
