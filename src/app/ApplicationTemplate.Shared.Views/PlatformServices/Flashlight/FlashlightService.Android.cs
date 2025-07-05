// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Flashlight/FlashlightService.Android.cs
#if __ANDROID__
using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware.Camera2;
using AndroidX.Core.Content;
using Microsoft.UI.Xaml;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class FlashlightService : IFlashlightService
{
	private CameraManager _cameraManager;
	private string _cameraId;
	private bool _isOn;

	public float Brightness { get; set; } = 1.0f;

	public async Task Initialize()
	{
		var context = Platform.CurrentActivity ?? Android.App.Application.Context;
		_cameraManager = (CameraManager)context.GetSystemService(Context.CameraService);

		var cameraIds = _cameraManager.GetCameraIdList();
		foreach (var id in cameraIds)
		{
			var characteristics = _cameraManager.GetCameraCharacteristics(id);
			var flashAvailable = (Java.Lang.Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
			if (flashAvailable.BooleanValue())
			{
				_cameraId = id;
				break;
			}
		}
	}

	public void Toggle()
	{
		if (string.IsNullOrEmpty(_cameraId) || _cameraManager == null)
			return;

		try
		{
			_isOn = !_isOn;
			_cameraManager.SetTorchMode(_cameraId, _isOn && Brightness > 0);
		}
		catch (Exception)
		{
			// Handle camera access errors silently
		}
	}
}
#endif
