using System;
using System.Windows.Input;
using Chinook.DynamicMvvm;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Behaviors;

/// <summary>
/// Behavior for the <see cref="RefreshContainer"/>.
/// </summary>
/// <remarks>
/// The code comes from Uno.Extensions.
/// See https://github.com/unoplatform/uno.extensions/blob/main/src/Uno.Extensions.Reactive.UI/Utils/RefreshContainerExtensions.cs for more details.
/// </remarks>
public static class RefreshContainerCommandBehavior
{
	/// <summary>
	/// The backing property for the <see cref="ICommand"/>.
	/// </summary>
	public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
		"Command", typeof(ICommand), typeof(RefreshContainerCommandBehavior), new PropertyMetadata(default(ICommand), OnCommandChanged));

	/// <summary>
	/// Gets the <see cref="ICommand"/> attached on a <see cref="RefreshContainer"/>.
	/// </summary>
	/// <param name="refreshContainer">The refresh container to get the command for.</param>
	/// <returns>The attached command, if any.</returns>
	public static ICommand GetCommand(RefreshContainer refreshContainer)
		=> (ICommand)refreshContainer.GetValue(CommandProperty);

	/// <summary>
	/// Attach the <see cref="ICommand"/> on the given <see cref="RefreshContainer"/>.
	/// </summary>
	/// <param name="refreshContainer">The refresh container on which command should be set.</param>
	/// <param name="command">The command to set.</param>
	public static void SetCommand(RefreshContainer refreshContainer, ICommand command)
		=> refreshContainer.SetValue(CommandProperty, command);

	/// <summary>
	/// The backing property for the command parameter.
	/// </summary>
	public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
		"CommandParameter", typeof(object), typeof(RefreshContainerCommandBehavior), new PropertyMetadata(default));

	/// <summary>
	/// Gets the command parameter attached on a <see cref="RefreshContainer"/>.
	/// </summary>
	/// <param name="refreshContainer">The refresh container to get the command parameter for.</param>
	/// <returns>The attached command parameter, if any.</returns>
	public static object GetCommandParameter(RefreshContainer refreshContainer)
		=> refreshContainer.GetValue(CommandParameterProperty);

	/// <summary>
	/// Attach the <paramref name="parameter"/> on the given <see cref="RefreshContainer"/>.
	/// </summary>
	/// <param name="refreshContainer">The refresh container on which command should be set.</param>
	/// <param name="parameter">The command parameter to set.</param>
	public static void SetCommandParameter(RefreshContainer refreshContainer, object parameter)
		=> refreshContainer.SetValue(CommandParameterProperty, parameter);

	private static void OnCommandChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
	{
		if (snd is not RefreshContainer refreshContainer)
		{
			return;
		}

		refreshContainer.RefreshRequested -= OnRefreshRequested;

		if (args.NewValue is ICommand)
		{
			refreshContainer.RefreshRequested += OnRefreshRequested;
		}
	}

	private static void OnRefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
	{
		var command = GetCommand(sender);
		var parameter = GetCommandParameter(sender);

		if (!command.CanExecute(parameter))
		{
			return;
		}

		var deferral = args.GetDeferral();

		if (command is IDynamicCommand dynamicCommand)
		{
			dynamicCommand.IsExecutingChanged -= OnIsExecutingChanged;
			dynamicCommand.IsExecutingChanged += OnIsExecutingChanged;

			dynamicCommand.Execute(parameter);

			void OnIsExecutingChanged(object sender, EventArgs e)
			{
				if (!dynamicCommand.IsExecuting)
				{
					deferral.Complete();

					dynamicCommand.IsExecutingChanged -= OnIsExecutingChanged;
				}
			}
		}
		else
		{
			command.Execute(parameter);
			deferral.Complete();
		}
	}
}
