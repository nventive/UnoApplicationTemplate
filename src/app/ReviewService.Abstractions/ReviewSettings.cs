using System;

namespace ReviewService.Abstractions;

/// <summary>
/// The review prompt settings used for prompt conditions.
/// </summary>
public record ReviewSettings
{
	public int PrimaryActionCompletedCount { get; init; }

	public int SecondaryActionCompletedCount { get; init; }

	public int ApplicationLaunchCount { get; init; }

	public DateTimeOffset? ApplicationFirstLaunched { get; init; }
}
