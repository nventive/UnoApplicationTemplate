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
