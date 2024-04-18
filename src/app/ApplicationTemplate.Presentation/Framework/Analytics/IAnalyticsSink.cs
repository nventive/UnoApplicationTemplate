#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// This service collects raw analytics data from the application and processes it to send to an analytics provider (such as AppCenter, Firebase, Segment, etc.).
/// </summary>
public interface IAnalyticsSink
{
	/// <summary>
	/// Tracks a navigation event from which to derive page views.
	/// </summary>
	/// <param name="navigatorState">The state of the navigator.</param>
	void TrackNavigation(SectionsNavigatorState navigatorState);

	/// <summary>
	/// Tracks a command execution initiation.
	/// </summary>
	/// <param name="commandName">The name of the command.</param>
	/// <param name="commandParameter">The optional command parameter.</param>
	/// <param name="viewModel">An optional weak reference to the ViewModel owning the command.</param>
	void TrackCommand(string commandName, object? commandParameter, WeakReference<IViewModel>? viewModel);
}
