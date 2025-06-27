// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.Windows.cs
#if __WINDOWS__
using System;
using System.Threading;
using System.Threading.Tasks;
using CPS.DataAccess;
using Microsoft.UI.Dispatching;
using Microsoft.Web.WebView2.Core;
using Windows.System;


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
		// For Windows, we'll use the system default browser as embedded browser
		// In a real implementation, you might want to create a WebView2 control
		await _dispatcherQueue.EnqueueAsync(async () =>
		{
			await Launcher.LaunchUriAsync(uri);
		});
	}
}
#endif
