using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The fake <see cref="IFlashlightService"/> implementation for testing purposes.
/// </summary>
public sealed class FakeFlashlightService : IFlashlightService
{
	/// <inheritdoc/>
	public float Brightness { get; set; }

	/// <inheritdoc/>
	public Task Initialize()
	{
		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	public void Toggle()
	{
		return;
	}
}
