using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace ApplicationTemplate;

public sealed class LauncherService : ILauncherService
{
	private readonly DispatcherQueue _dispatcherQueue;

	public LauncherService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue;
	}

	public async Task Launch(Uri uri)
	{
		var launchSucceeded = await _dispatcherQueue.EnqueueAsync(InnerLaunch, DispatcherQueuePriority.Normal);
		if (!launchSucceeded)
		{
			throw new LaunchFailedException($"Failed to launch URI: {uri}");
		}

		async Task<bool> InnerLaunch()
		{
			return await Windows.System.Launcher.LaunchUriAsync(uri);
		}
	}
}
