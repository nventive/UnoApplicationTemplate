// path/to/FlashlightService.cs
using Windows.Devices.Lights;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	public class FlashlightService : IFlashlightService
	{
		private Lamp _lamp;

		public float Brightness { get; set; } = 1.0f;

		public async Task Initialize()
		{
			_lamp = await Lamp.GetDefault();
		}

		public void Toggle()
		{
			if (_lamp != null)
			{
				if (_lamp.IsOn)
				{
					_lamp.Disable();
				}
				else
				{
					_lamp.Enable(Brightness);
				}
			}
		}
	}
}
