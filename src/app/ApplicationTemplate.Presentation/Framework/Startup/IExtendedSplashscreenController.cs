using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate;

/// <summary>
/// Controls the extended splash screen.
/// </summary>
public interface IExtendedSplashscreenController
{
	/// <summary>
	/// Dismisses the extended splash screen.
	/// </summary>
	void Dismiss();
}

/// <summary>
/// This implementation of <see cref="IExtendedSplashscreenController"/> does nothing.
/// </summary>
public class MockExtendedSplashscreenController : IExtendedSplashscreenController
{
	public void Dismiss()
	{
	}
}
