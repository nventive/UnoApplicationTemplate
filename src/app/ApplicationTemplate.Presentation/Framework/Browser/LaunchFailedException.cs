using System;

namespace ApplicationTemplate;

/// <summary>
/// Exception used in <see cref="ILauncherService"/> when launch failed.
/// </summary>
public sealed class LaunchFailedException : Exception
{
	public LaunchFailedException(string message)
		: base(message)
	{
	}
}
