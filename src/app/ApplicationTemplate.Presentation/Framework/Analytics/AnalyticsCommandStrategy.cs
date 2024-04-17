using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// This <see cref="IDynamicCommandStrategy"/> tracks the success and failure of a command for analytics purposes.
/// </summary>
public sealed class AnalyticsCommandStrategy : DelegatingCommandStrategy
{
	private readonly IAnalyticsSink _analyticsSink;
	private readonly WeakReference<IViewModel> _viewModel;

	public AnalyticsCommandStrategy(IAnalyticsSink analyticsSink, IViewModel viewModel)
	{
		_analyticsSink = analyticsSink;
		_viewModel = new WeakReference<IViewModel>(viewModel);
	}

	public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
	{
		_analyticsSink.TrackCommand(command.Name, parameter, _viewModel);

		await base.Execute(ct, parameter, command);
	}
}
