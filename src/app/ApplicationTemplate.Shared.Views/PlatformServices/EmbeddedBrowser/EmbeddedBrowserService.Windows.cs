// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.Windows.cs
#if WINDOWS
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

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
                // Create a WebView for embedded browsing and display in a ContentDialog.
                var webView = new WebView
                {
                    Source = uri
                };

                var dialog = new ContentDialog
                {
                    Title = "Embedded Browser",
                    Content = webView,
                    CloseButtonText = "Close",
                    XamlRoot = App.Instance.CurrentWindow.Content.XamlRoot  // Assumes access to the current window
                };

                // Show the dialog and wait for dismissal.
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., WebView loading failure).
                // Optionally log: this.GetService<ILogger>().LogError(ex, "Failed to launch embedded browser.");
                throw;  // Or fallback to system browser if needed.
            }
        }
    }
}
#endif
