using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using MessageDialogService;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// This class provides extensions on ViewModels for diagnostics features.
/// </summary>
public static class DiagnosticsViewModelExtensions
{
	/// <summary>
	/// Gets a command that notifies the user that the application needs to be restarted for the changes to take effect.
	/// </summary>
	/// <param name="viewModel">The ViewModel that owns the command.</param>
	public static IDynamicCommand GetNotifyNeedsRestartCommand(this IViewModel viewModel) => viewModel.GetCommandFromTask(async ct =>
	{
		await viewModel.GetService<IMessageDialogService>().ShowMessage(ct, mb => mb
			.Title("Diagnostics")
			.Content("Restart the application to apply your changes.")
			.OkCommand()
		);
	});
}
