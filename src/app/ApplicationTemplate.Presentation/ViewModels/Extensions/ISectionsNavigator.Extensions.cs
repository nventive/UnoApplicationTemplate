using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.StackNavigation;

namespace Chinook.SectionsNavigation;

public static class SectionsNavigatorExtensions
{
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
