namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Fake implementation of <see cref="IScreenWakeLockService"/> for testing purposes.
/// </summary>
public sealed class FakeScreenWakeLockService : IScreenWakeLockService
{
	/// <inheritdoc/>
	public void Disable()
	{
		return;
	}

	/// <inheritdoc/>
	public void Enable()
	{
		return;
	}
}
