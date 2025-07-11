using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Windows.Media.Capture;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <inheritdoc/>
public sealed class CameraService : ICameraService
{
	private readonly DispatcherQueue _dispatcherQueue;

	public CameraService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
	}

	/// <inheritdoc/>
	public async Task<string> CapturePhoto(CameraCaptureMode mode)
	{
		return await _dispatcherQueue.EnqueueAsync(async () =>
		{
			var captureUI = new CameraCaptureUI();

			captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

			var captureMode = mode switch
			{
				CameraCaptureMode.Photo => CameraCaptureUIMode.Photo,
				CameraCaptureMode.Video => CameraCaptureUIMode.Video,
				_ => CameraCaptureUIMode.PhotoOrVideo,
			};

			var file = await captureUI.CaptureFileAsync(captureMode);

			return file?.Path;
		});
	}
}
