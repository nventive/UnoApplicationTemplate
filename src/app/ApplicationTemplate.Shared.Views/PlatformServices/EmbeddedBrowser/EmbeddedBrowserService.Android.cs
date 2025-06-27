// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.Android.cs
#if __ANDROID__
using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Browser.CustomTabs;
using Windows.System;  // For fallback Launcher if needed

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
                // Use Custom Tabs for an in-app embedded browser experience.
                var customTabsIntent = new CustomTabsIntent.Builder().Build();
                customTabsIntent.LaunchUrl(Android.App.Application.Context, Android.Net.Uri.Parse(uri.ToString()));
            }
            catch (Exception ex)
            {
                // Fallback to system browser if Custom Tabs fail (e.g., no compatible browser installed).
                await Launcher.LaunchUriAsync(uri);  // Uses system launcher as a backup.
                // Optionally log the error: e.g., this.GetService<ILogger>().LogError(ex, "Failed to launch embedded browser.");
            }
        }
    }
}
#endif
