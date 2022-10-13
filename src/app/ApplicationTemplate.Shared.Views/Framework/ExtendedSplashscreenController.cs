using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Dispatching;

namespace ApplicationTemplate;

public class ExtendedSplashscreenController : IExtendedSplashscreenController
{
	private readonly DispatcherQueue _coreDispatcher;

	public ExtendedSplashscreenController(DispatcherQueue coreDispatcher)
	{
		_coreDispatcher = coreDispatcher;
	}

	public void Dismiss()
	{
		_coreDispatcher.TryEnqueue(DispatcherQueuePriority.Normal, DismissSplashScreen);

		void DismissSplashScreen() // Runs on UI thread
		{
			Shell.Instance.ExtendedSplashScreen.Dismiss();
		}
	}
}
