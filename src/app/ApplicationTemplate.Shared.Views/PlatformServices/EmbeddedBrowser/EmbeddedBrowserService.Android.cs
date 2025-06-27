#if __ANDROID__
using System;
using System.Threading;
using System.Threading.Tasks;
using AndroidX.Browser.CustomTabs;
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

		_dispatcherQueue.TryEnqueue(() =>
		{
			var context = Uno.UI.ContextHelper.Current;
			var intent = new CustomTabsIntent.Builder()
				.SetShowTitle(true)
				.Build();

			intent.LaunchUrl(context, Android.Net.Uri.Parse(uri.ToString()));
		});
	}
}
#endif
