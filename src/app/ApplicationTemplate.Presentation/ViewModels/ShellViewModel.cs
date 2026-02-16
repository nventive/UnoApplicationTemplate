using System.ComponentModel;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate.Presentation;

[Bindable(true)]
public class ShellViewModel : ViewModel
{
	public DiagnosticsOverlayViewModel DiagnosticsOverlay => this.GetChild<DiagnosticsOverlayViewModel>();

	public MenuViewModel Menu => this.GetChild<MenuViewModel>();
}
