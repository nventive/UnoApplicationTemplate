namespace ReviewService.Abstractions;

/// <summary>
/// The custom review prompt settings used for prompt conditions.
/// </summary>
public record ReviewSettingsCustom : ReviewSettings
{
	public bool HasCompletedOnboarding { get; init; }
}
