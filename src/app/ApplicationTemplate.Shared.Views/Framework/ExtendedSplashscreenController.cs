using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Core;

namespace ApplicationTemplate;

public class ExtendedSplashscreenController : IExtendedSplashscreenController
{
	private readonly CoreDispatcher _coreDispatcher;

	public ExtendedSplashscreenController(CoreDispatcher coreDispatcher)
	{
		_coreDispatcher = coreDispatcher;
	}

	public void Dismiss()
	{
		_ = _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, DismissSplashScreen);

		void DismissSplashScreen() // Runs on UI thread
		{
			Shell.Instance.ExtendedSplashScreen.Dismiss();
		}
	}
}
