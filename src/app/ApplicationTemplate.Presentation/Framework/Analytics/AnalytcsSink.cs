#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Presentation;

public sealed class AnalyticsSink : IAnalyticsSink
{
	private readonly ILogger<AnalyticsSink> _logger;
	private INavigableViewModel? _lastViewModel;

	public AnalyticsSink(ILogger<AnalyticsSink> logger)
	{
		_logger = logger;
	}

	public void TrackNavigation(SectionsNavigatorState navigatorState)
	{
		if (navigatorState.LastRequestState != NavigatorRequestState.Processed)
		{
			// Skip the requests that are still processing of that failed to process.
			return;
		}

		// Get the actual ViewModel instance.
		// This allows to track based on instances and not types (because there are scenarios where you can open the same page multiple times with different parameters).
		// Having the instance also allows casting into more specific types to get more information, such as navigation parameters, that could be relevant for analytics.
		var viewModel = navigatorState.GetActiveStackNavigator().State.Stack.LastOrDefault()?.ViewModel;
		if (viewModel is null || _lastViewModel == viewModel)
		{
			return;
		}

		// Gather analytics data.
		var pageName = viewModel.GetType().Name.Replace("ViewModel", string.Empty, StringComparison.OrdinalIgnoreCase);
		var isInModal = navigatorState.ActiveModal != null;
		var sectionName = navigatorState.ActiveSection.Name;

		// Send the analytics event.
		SendPageView(pageName, isInModal, sectionName);

		// Capture the last ViewModel instance to avoid duplicate events in the future.
		_lastViewModel = viewModel;
	}

	private void SendPageView(string pageName, bool isInModal, string sectionName)
	{
		// TODO: Implement page views using a real analytics provider.
		if (!_logger.IsEnabled(LogLevel.Information))
		{
			return;
		}

		if (isInModal)
		{
			_logger.LogInformation("Viewed page '{PageName}' in modal.", pageName);
		}
		else
		{
			_logger.LogInformation("Viewed page '{PageName}' in section '{SectionName}'.", pageName, sectionName);
		}
	}

	public void TrackCommand(string commandName, object? commandParameter, WeakReference<IViewModel>? viewModel)
	{
		// TODO: Implement command execution events using a real analytics provider.
		if (!_logger.IsEnabled(LogLevel.Information))
		{
			return;
		}

		if (viewModel?.TryGetTarget(out var vm) ?? false)
		{
			_logger.LogInformation("Invoked command '{CommandName}' from ViewModel '{ViewModelName}'.", commandName, vm.Name);
		}
		else
		{
			_logger.LogInformation("Invoked command '{CommandName}'.", commandName);
		}
	}
}
