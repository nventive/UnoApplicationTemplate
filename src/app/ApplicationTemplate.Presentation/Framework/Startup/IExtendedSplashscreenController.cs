using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate
{
	public interface IExtendedSplashscreenController
	{
		/// <summary>
		/// Dismisses the extended splashscreen.
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
}
