// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Camera/CameraService.cs
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.PlatformServices;
#if WINDOWS
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

				if (activity == null)
					return null;

				Intent intent;
				if (mode == CameraCaptureMode.Video)
				{
					intent = new Intent(MediaStore.ActionVideoCapture);
				}
				else
				{
					intent = new Intent(MediaStore.ActionImageCapture);
				}

				if (intent.ResolveActivity(activity.PackageManager) != null)
				{
					var file = new File(activity.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures),
						$"capture_{System.DateTime.Now.Ticks}.{(mode == CameraCaptureMode.Video ? "mp4" : "jpg")}");

					var photoURI = FileProvider.GetUriForFile(activity,
						activity.PackageName, file);

					intent.PutExtra(MediaStore.ExtraOutput, photoURI);

					var launcher = activity.RegisterForActivityResult(
						new ActivityResultContracts.StartActivityForResult(),
						new ActivityResultCallback(result =>
						{
							if (result.ResultCode == (int)Android.App.Result.Ok)
							{
								tcs.SetResult(file.AbsolutePath);
							}
							else
							{
								tcs.SetResult(null);
							}
						}));

					launcher.Launch(intent);
					return await tcs.Task;
				}

				return null;
			}
			catch
			{
				return null;
			}
		}

		private class ActivityResultCallback : Java.Lang.Object, IActivityResultCallback
		{
			private readonly System.Action<AndroidX.Activity.Result.ActivityResult> _callback;

			public ActivityResultCallback(System.Action<AndroidX.Activity.Result.ActivityResult> callback)
			{
				_callback = callback;
			}

			public void OnActivityResult(Java.Lang.Object result)
			{
				_callback?.Invoke(result as AndroidX.Activity.Result.ActivityResult);
			}
		}
#endif

#if IOS
		private async Task<string> CapturePhotoiOS(CameraCaptureMode mode)
		{
			try
			{
				var tcs = new TaskCompletionSource<string>();

				var picker = new UIImagePickerController();
				picker.SourceType = UIImagePickerControllerSourceType.Camera;

				switch (mode)
				{
					case CameraCaptureMode.Photo:
						picker.MediaTypes = new string[] { "public.image" };
						break;
					case CameraCaptureMode.Video:
						picker.MediaTypes = new string[] { "public.movie" };
						break;
					case CameraCaptureMode.PhotoOrVideo:
						picker.MediaTypes = new string[] { "public.image", "public.movie" };
						break;
				}

				picker.Finished += (sender, e) =>
				{
					var documentsPath = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0];
					string fileName;
					string filePath;

					if (e.MediaType == "public.image")
					{
						fileName = $"capture_{System.DateTime.Now.Ticks}.jpg";
						filePath = System.IO.Path.Combine(documentsPath, fileName);
						var image = e.OriginalImage;
						var data = image.AsJPEG();
						data.Save(filePath, true);
					}
					else
					{
						fileName = $"capture_{System.DateTime.Now.Ticks}.mp4";
						filePath = System.IO.Path.Combine(documentsPath, fileName);
						var videoUrl = e.MediaUrl;
						var videoData = NSData.FromUrl(videoUrl);
						videoData.Save(filePath, true);
					}

					picker.DismissViewController(true, null);
					tcs.SetResult(filePath);
				};

				picker.Canceled += (sender, e) =>
				{
					picker.DismissViewController(true, null);
					tcs.SetResult(null);
				};

				var viewController = Platform.GetCurrentUIViewController();
				await viewController.PresentViewControllerAsync(picker, true);

				return await tcs.Task;
			}
			catch
			{
				return null;
			}
		}
#endif
	}
}
