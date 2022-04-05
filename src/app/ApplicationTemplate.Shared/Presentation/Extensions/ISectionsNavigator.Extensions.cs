﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.StackNavigation;

namespace Chinook.SectionsNavigation
{
	public static class SectionsNavigatorExtensions
	{
		/// <summary>
		/// Observe the active section and return the name
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
		/// Determines if the section navigator can navigate back.
		/// </summary>
		/// <param name="sectionsNavigator"><see cref="ISectionsNavigator"/>.</param>
		/// <returns>.</returns>
		public static bool CanNavigateBackOrCloseDismissableModal(this ISectionsNavigator sectionsNavigator)
		{
			var modalNavigator = sectionsNavigator.State.ActiveModal;
			if (modalNavigator != null)
			{
				return modalNavigator.CanNavigateBack();
			}

			// If the active section isn't a modal, use the default behavior.
			return sectionsNavigator.CanNavigateBackOrCloseModal();
		}
	}
}
