// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Flashlight/FlashlightService.Windows.cs
#if __WINDOWS__
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class FlashlightService : IFlashlightService
{
	private string _deviceId;
	private bool _isOn;

	public float Brightness { get; set; } = 1.0f;

	public async Task Initialize()
	{
		try
		{
			var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
			var backCamera = devices.FirstOrDefault(d => d.EnclosureLocation?.Panel == Windows.Devices.Enumeration.Panel.Back);
			_deviceId = backCamera?.Id;
		}
		catch (Exception)
		{
			// Handle device enumeration errors silently
		}
	}

	public void Toggle()
	{
		if (string.IsNullOrEmpty(_deviceId))
			return;

		try
		{
			_isOn = !_isOn;

			if (_isOn && Brightness > 0)
			{
				var torchControl = TorchControl.GetDefault();
				if (torchControl != null && torchControl.Supported)
				{
					torchControl.PowerPercent = Math.Max(1, (int)(Brightness * 100));
					torchControl.Enabled = true;
				}
			}
			else
			{
				var torchControl = TorchControl.GetDefault();
				if (torchControl != null)
				{
					torchControl.Enabled = false;
				}
			}
		}
		catch (Exception)
		{
			// Handle torch control errors silently
		}
	}
}
#endif
