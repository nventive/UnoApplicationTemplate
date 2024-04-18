using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed partial class NavigationDebuggerViewModel : TabViewModel
{
	[Inject] private ISectionsNavigator _sectionsNavigator;

	public NavigationDebuggerViewModel()
	{
		Title = "Navigation";
	}

	/// <summary>
	/// Gets a command that clears all modals and sections.
	/// This should make the app look empty.
	/// </summary>
	public IDynamicCommand Clear => this.GetCommandFromTask(async ct =>
	{
		while (_sectionsNavigator.State.ActiveModal != null)
		{
			await _sectionsNavigator.CloseModal(ct);
		}
		foreach (var section in _sectionsNavigator.State.Sections)
		{
			await section.Value.Clear(ct);
		}
	});

	/// <summary>
	/// Gets a command that reinitializes the app navigation to its initial state.
	/// </summary>
	public IDynamicCommand Reinitialize => this.GetCommandFromTask(async ct =>
	{
		await CoreStartup.ExecuteInitialNavigation(ct, ServiceProvider);
	});
}
