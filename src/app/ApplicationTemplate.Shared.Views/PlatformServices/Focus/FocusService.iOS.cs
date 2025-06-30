#if __IOS__
using System.Linq;
using Microsoft.Extensions.Logging;
using UIKit;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <inheritdoc/>
public sealed partial class FocusService : IFocusService
{
	public void ClearFocus()
	{
		_dispatcherQueue.TryEnqueue(() =>
		{
			_logger.LogDebug("Clearing current focus.");

			var window = UIApplication.SharedApplication.ConnectedScenes
				.OfType<UIWindowScene>()
				.Where(scene => scene.ActivationState == UISceneActivationState.ForegroundActive)
				.SelectMany(scene => scene.Windows)
				.FirstOrDefault(window => window.IsKeyWindow);

			_logger.LogTrace("Checking if current window is null.");

			if (window is null)
			{
				_logger.LogWarning("The current focus was not cleared because the current window is null.");
				return;
			}

			window.EndEditing(true);

			_logger.LogInformation("Current focus cleared successfully.");
		});
	}
}
#endif
