using System;
using System.Threading.Tasks;

namespace ApplicationTemplate;

/// <summary>
/// Extensions for <see cref="ILauncherService"/>.
/// </summary>
public static class LauncherServiceExtensions
{
	/// <summary>
	/// Opens the default application associated with the <see cref="Uri"/>.
	/// </summary>
	/// <param name="launcherService"><see cref="ILauncherService"/>.</param>
	/// <param name="uri">The <see cref="Uri"/> to launch.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static async Task Launch(this ILauncherService launcherService, string uri)
	{
		if (string.IsNullOrEmpty(uri))
		{
			throw new ArgumentNullException(nameof(uri));
		}
		await launcherService.Launch(new Uri(uri));
	}
}
