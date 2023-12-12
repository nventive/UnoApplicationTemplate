using System;
using Chinook.StackNavigation;

namespace Chinook.SectionsNavigation;

public static class SectionsNavigatorStateExtensions
{
	public static Type GetCurrentOrNextViewModelType(this SectionsNavigatorState sectionsNavigatorState)
	{
		switch (sectionsNavigatorState.LastRequestState)
		{
			case NavigatorRequestState.Processing:
				return sectionsNavigatorState.GetNextViewModelType();
			case NavigatorRequestState.Processed:
			case NavigatorRequestState.FailedToProcess:
				return sectionsNavigatorState.GetLastViewModelType();
			default:
				throw new NotSupportedException(
					$"The request state {sectionsNavigatorState.LastRequestState} is not supported.");
		}
	}
}
