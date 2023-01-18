using System;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Logging;
using Windows.System;

namespace ApplicationTemplate;

public sealed class LauncherService : ILauncherService
{
	public async Task Launch(Uri uri)
	{
		var launchSucceeded = await Launcher.LaunchUriAsync(uri);
		if (!launchSucceeded)
		{
			throw new LaunchFailedException($"Failed to launch URI: {uri}");
		}
	}
}
