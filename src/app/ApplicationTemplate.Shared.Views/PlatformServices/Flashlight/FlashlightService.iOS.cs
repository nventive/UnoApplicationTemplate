// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Flashlight/FlashlightService.iOS.cs
#if __IOS__
using System;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class FlashlightService : IFlashlightService
{
	private AVCaptureDevice _device;
	private bool _isOn;

	public float Brightness { get; set; } = 1.0f;

	public Task Initialize()
	{
		_device = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
		return Task.CompletedTask;
	}

	public void Toggle()
	{
		if (_device == null || !_device.HasTorch)
			return;

		try
		{
			_isOn = !_isOn;

			if (_device.LockForConfiguration(out var error))
			{
				if (_isOn && Brightness > 0)
				{
					if (_device.HasTorch)
					{
						_device.SetTorchModeLevel(Math.Max(0.01f, Math.Min(1.0f, Brightness)), out error);
					}
					else
					{
						_device.TorchMode = AVCaptureTorchMode.On;
					}
				}
				else
				{
					_device.TorchMode = AVCaptureTorchMode.Off;
				}

				_device.UnlockForConfiguration();
			}
		}
		catch (Exception)
		{
			// Handle camera access errors silently
		}
	}
}
#endif
