using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.DataAccess;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed class NavigationDebuggerViewModel : TabViewModel
{
	private readonly ISectionsNavigator _sectionsNavigator;

	public NavigationDebuggerViewModel()
	{
		ResolveService(out _sectionsNavigator);

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

	/// <summary>
	/// Gets a command that raises the event that triggers the navigation to the force update page.
	/// </summary>
	public IDynamicCommand NavigateToForceUpdatePage => this.GetCommand(() =>
	{
		this.GetService<IMinimumVersionProvider>().CheckMinimumVersion();
	});

	/// <summary>
	/// Gets a command that raises the kill switch event.
	/// </summary>
	public IDynamicCommand TriggerKillSwitch => this.GetCommand(() =>
	{
		var killSwitchDataSource = this.GetService<IKillSwitchDataSource>();

		if (killSwitchDataSource is KillSwitchDataSourceMock killSwitchDataSourceMock)
		{
			killSwitchDataSourceMock.ChangeKillSwitchActivation();
		}
	});
}
