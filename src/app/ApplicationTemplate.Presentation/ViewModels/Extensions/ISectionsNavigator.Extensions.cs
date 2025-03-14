using System;
using System.Linq;
using System.Reactive.Linq;
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
}
