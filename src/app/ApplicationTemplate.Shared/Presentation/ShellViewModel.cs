using System;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate
{
	public class ShellViewModel : ViewModel
	{
		public IViewModel DiagnosticsOverlay => this.GetChild<DiagnosticsOverlayViewModel>();

		public IViewModel Menu => this.GetChild<MenuViewModel>();
	}
}
