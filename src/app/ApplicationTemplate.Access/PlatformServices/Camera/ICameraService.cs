using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Determines whether the user interface for capturing from the attached camera allows capture of photos, videos, or both photos and videos.
/// </summary>
public enum CameraCaptureMode
{
	/// <summary>
	/// Either a photo or video can be captured.
	/// </summary>
	PhotoOrVideo,

	/// <summary>
	/// The user can only capture a photo.
	/// </summary>
	Photo,

	/// <summary>
	/// The user can only capture a video.
	/// </summary>
	Video,
}

/// <summary>
/// Provides a method for launching the camera capture user interface.
/// </summary>
public interface ICameraService
{
	/// <summary>
	/// Launches the camera capture UI user interface.
	/// </summary>
	/// <remarks>
	/// Does nothing on Windows.
	/// </remarks>
	/// <param name="mode">Specifies whether the user interface that will be shown allows the user to capture a photo, capture a video, or capture both photos and videos.</param>
	/// <returns>
	/// The file path of the captured photo or video, or null if the user cancelled the operation.
	/// </returns>
	Task<string> CapturePhoto(CameraCaptureMode mode);
}
