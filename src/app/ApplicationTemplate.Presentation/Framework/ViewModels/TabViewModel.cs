using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using Chinook.DynamicMvvm.Deactivation;
using Uno;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// Represents a ViewModel of a Tab.
/// A tab can be active or not.
/// </summary>
public abstract class TabViewModel : DeactivatableViewModelBase
{
	/// <summary>
	/// Gets the title of this Tab.
	/// This is to be displayed in a TabBar.
	/// </summary>
	public string Title { get; init; }

	/// <summary>
	/// Gets or sets a value indicating whether this tab is the active one.
	/// </summary>
	public bool IsActiveTab { get; set; }

	/// <inheritdoc/>
	/// <remarks>
	/// In the specific case of <see cref="TabViewModel"/>, reactivation will not happen unless <see cref="IsActiveTab"/> is true.
	/// </remarks>
	public override void Reactivate()
	{
		if (IsActiveTab)
		{
			base.Reactivate();
		}
	}

	/// <summary>
	/// Resolves a service of type <typeparamref name="TService"/> from the service provider.
	/// </summary>
	/// <typeparam name="TService">The type of service to resolve.</typeparam>
	/// <param name="service">The service variable in which to return the resolved service.</param>
	protected void ResolveService<TService>(out TService service)
	{
		service = this.GetService<TService>();
	}
}

public static class TabViewModelExtensions
{
	/// <summary>
	/// Deactivates inactive tabs and reactivates the active tab.
	/// This ensures only 1 item from <paramref name="tabs"/> has <see cref="IDeactivatable.IsDeactivated"/> set to false.
	/// </summary>
	/// <param name="tabs">The items to deactivate and reactivate.</param>
	/// <param name="activeTabIndex">The active tab index indicating which tab should be reactivated.</param>
	public static void SetActiveTabIndex(this TabViewModel[] tabs, int activeTabIndex)
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			var tab = tabs[i];
			if (i == activeTabIndex)
			{
				tab.IsActiveTab = true;
				tab.Reactivate();
			}
			else
			{
				tab.IsActiveTab = false;
				tab.Deactivate();
			}
		}
	}
}
