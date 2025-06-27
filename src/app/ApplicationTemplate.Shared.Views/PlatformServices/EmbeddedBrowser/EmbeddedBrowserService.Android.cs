// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.Android.cs
#if __ANDROID__
using System;
using System.Threading;
using System.Threading.Tasks;
using AndroidX.Browser.CustomTabs;
using CPS.DataAccess;
using Microsoft.UI.Dispatching;


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
                var context = Platform.CurrentActivity ?? Android.App.Application.Context;
                var intent = new CustomTabsIntent.Builder()
                    .SetShowTitle(true)
                    .Build();
                
                intent.LaunchUrl(context, Android.Net.Uri.Parse(uri.ToString()));
            });
        });
    }
}
#endif
