using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <inheritdoc/>
public sealed class FakeCameraService : ICameraService
{
	/// <inheritdoc/>
	public Task<string> CapturePhoto(CameraCaptureMode mode)
	{
		return Task.FromResult<string>(null);
	}
}
