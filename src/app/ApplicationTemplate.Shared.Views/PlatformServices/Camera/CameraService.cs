using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Windows.Media.Capture;
using Windows.Storage;
using WinRT.Interop;
#elif ANDROID
using AndroidX.Activity.Result;
using Android.Content;
using Android.Provider;
using AndroidX.Core.Content;
using Android.Net;
using Java.IO;
using Microsoft.UI.Xaml;
#elif IOS
using UIKit;
using Foundation;
using AVFoundation;
#endif

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	public sealed class CameraService : ICameraService
	{
		public async Task<string> CapturePhoto(CameraCaptureMode mode)
		{
#if WINDOWS
			return await CapturePhotoWindows(mode);
#elif ANDROID
			return await CapturePhotoAndroid(mode);
#elif IOS
			return await CapturePhotoiOS(mode);
#else
			return null;
#endif
		}

#if WINDOWS
		private async Task<string> CapturePhotoWindows(CameraCaptureMode mode)
		{
			try
			{
				var captureUI = new CameraCaptureUI();

				switch (mode)
				{
					case CameraCaptureMode.Photo:
						captureUI.PhotoSettings.AllowCropping = true;
						captureUI.VideoSettings.MaxResolution = CameraCaptureUIMaxVideoResolution.StandardDefinition;
						break;
					case CameraCaptureMode.Video:
						captureUI.VideoSettings.AllowTrimming = true;
						break;
					case CameraCaptureMode.PhotoOrVideo:
						captureUI.PhotoSettings.AllowCropping = true;
						captureUI.VideoSettings.AllowTrimming = true;
						break;
				}


				var file = mode == CameraCaptureMode.Video
					? await captureUI.CaptureFileAsync(CameraCaptureUIMode.Video)
					: await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);


				return file?.Path;
			}
			catch
			{
				return null;
			}
		}
#endif

#if ANDROID
		private async Task<string> CapturePhotoAndroid(CameraCaptureMode mode)
		{
			try
			{
				var tcs = new TaskCompletionSource<string>();
				var activity = Platform.CurrentActivity as AndroidX.AppCompat.App.AppCompatActivity;

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
