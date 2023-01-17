using System;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Logging;
using Windows.System;

namespace ApplicationTemplate;

public sealed class BrowserService : IBrowserService
{
	public async Task OpenAsync(Uri uri)
	{
		var launchSucceeded = await Launcher.LaunchUriAsync(uri);
		if (!launchSucceeded)
		{
			this.Log().Error($"Failed to launch URI: {uri}");
		}
	}
}
