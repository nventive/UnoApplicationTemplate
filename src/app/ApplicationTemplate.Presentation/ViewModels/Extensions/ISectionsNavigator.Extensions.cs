using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading;
using Chinook.StackNavigation;

namespace Chinook.SectionsNavigation;

/// <summary>
/// Extensions for <see cref="ISectionsNavigator"/>.
/// </summary>
public static class SectionsNavigatorExtensions
{
	/// <summary>
	/// Gets an observable of the last page type from currently active section.
	/// The observable pushes a value whenever a navigation request is processed with the type of the last page ViewModel.
	/// </summary>
	/// <param name="sectionsNavigator">The sections navigator.</param>
	/// <returns>An observable of types.</returns>
	public static IObservable<Type> ObserveActiveSectionLastPageType(this ISectionsNavigator sectionsNavigator)
	{
		return sectionsNavigator
			.ObserveStateChanged()
			.Where(args => args.EventArgs.CurrentState.LastRequestState == NavigatorRequestState.Processed)
			.Select(args =>
			{
				var state = args.EventArgs.CurrentState;
				return state.ActiveSection?.State.Stack.LastOrDefault()?.ViewModel.GetType();
			})
			.StartWith(sectionsNavigator.State.ActiveSection?.State.Stack.LastOrDefault()?.ViewModel.GetType());
	}

	public static IObservable<SectionsNavigatorEventArgs> ObserveProcessedState(this ISectionsNavigator sectionsNavigator)
	{
		return sectionsNavigator
			.ObserveStateChanged()
			.Where(args => args.EventArgs.CurrentState.LastRequestState == NavigatorRequestState.Processed)
			.Select(args => args.EventArgs);
	}

	/// <summary>
	/// Observes the active section and returns its name.
	/// </summary>
	/// <param name="sectionsNavigator">The sections navigator.</param>
	/// <returns>A string of the active section.</returns>
	public static IObservable<string> ObserveActiveSectionName(this ISectionsNavigator sectionsNavigator) =>
		sectionsNavigator
			.ObserveStateChanged()
			.Where(args => args.EventArgs.CurrentState.LastRequestState == NavigatorRequestState.Processed)
			.Select(args =>
			{
				var state = args.EventArgs.CurrentState;
				return state.ActiveSection.Name;
			});

	/// <summary>
	/// Clears the sections and sets the active section to <paramref name="activeSectionName"/> if not null or empty.
	/// </summary>
	/// <param name="sectionsNavigator"><see cref="ISectionsNavigator"/>.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <param name="activeSectionName">The active section name to set after clearing them all.</param>
	/// <remarks>
	/// There are good chances that the page requesting this operation gets disposed, so you should use <see cref="CancellationToken.None"/>.
	/// <br/>
	/// If you don't want any section set as active don't provide <paramref name="activeSectionName"/>.
	/// </remarks>
	public static async Task ClearSections(this ISectionsNavigator sectionsNavigator, CancellationToken ct, string activeSectionName = default)
	{
		// Clearing those pages prevents bindings from updating in background.
		// For better performance, it's better to re-create the pages when we'll need them.
		foreach (var section in sectionsNavigator.State.Sections.Values)
		{
			await section.Clear(ct);
		}

		if (!string.IsNullOrEmpty(activeSectionName))
		{
			await sectionsNavigator.SetActiveSection(ct, activeSectionName);
		}
	}

	/// <summary>
	/// Closes all modals.
	/// </summary>
	/// <param name="sectionsNavigator"><see cref="ISectionsNavigator"/>.</param>
	/// <param name="ct">The cancellation token.</param>
	public static async Task CloseModals(this ISectionsNavigator sectionsNavigator, CancellationToken ct)
	{
		foreach (var modal in sectionsNavigator.State.Modals)
		{
			await sectionsNavigator.CloseModal(ct, SectionsNavigatorRequest.GetCloseModalRequest(modalName: modal.Name));
		}
	}
}
