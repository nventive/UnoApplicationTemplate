using System;
using System.Threading.Tasks;
using Windows.Devices.Lights;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The <see cref="IFlashlightService"/> implementation using Uno.
/// </summary>
public sealed class FlashlightService : IFlashlightService, IDisposable
{
	private Lamp _lamp;

	/// <inheritdoc/>
	public float Brightness
	{
		get => _lamp?.BrightnessLevel ?? 0f;
		set
		{
			if (_lamp is null)
			{
				return;
			}

			_lamp.BrightnessLevel = Math.Clamp(value, 0f, 1f);
		}
	}

	/// <inheritdoc/>
	public async Task Initialize()
	{
		_lamp ??= await Lamp.GetDefaultAsync();
	}

	/// <inheritdoc/>
	public void Toggle()
	{
		if (_lamp is null)
		{
			return;
		}

		_lamp.IsEnabled = !_lamp.IsEnabled;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_lamp?.Dispose();
		_lamp = null;
	}
}
