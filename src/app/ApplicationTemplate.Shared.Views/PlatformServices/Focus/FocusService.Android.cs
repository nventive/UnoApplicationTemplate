#if __ANDROID__
using Android.App;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <inheritdoc/>
public sealed partial class FocusService : IFocusService
{
	public void ClearFocus()
	{
		_dispatcherQueue.TryEnqueue(() =>
		{
			_logger.LogDebug("Clearing current focus.");

			_logger.LogTrace("Checking if current activity is null.");

			var activity = (Activity)Uno.UI.ContextHelper.Current;
			if (activity is null)
			{
				_logger.LogWarning("The current focus was not cleared because the current activity is null.");
				return;
			}

			_logger.LogTrace("Checking if current focus is null.");

			var currentFocus = activity.CurrentFocus;
			if (currentFocus is null)
			{
				_logger.LogWarning("The current focus was not cleared because the current focus is null.");
				return;
			}

			currentFocus.ClearFocus();

			_logger.LogInformation("Current focus cleared successfully.");
		});
	}
}
#endif
