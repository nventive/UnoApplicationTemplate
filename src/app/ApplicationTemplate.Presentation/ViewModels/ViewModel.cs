using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// ViewModel to use as a base for all other ViewModels.
/// </summary>
public class ViewModel : ViewModelBase, INavigableViewModel
{
	// Add properties or commands you want to have on all your ViewModels

	public IDynamicCommand NavigateBack => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().NavigateBackOrCloseModal(ct);
	});

	public IDynamicCommand CloseModal => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().CloseModal(ct);
	});

	void INavigableViewModel.SetView(object view)
	{
		var factory = this.GetService<IDispatcherFactory>();
		Dispatcher = factory.Create(view);
	}

	/// <summary>
	/// Executes the specified <paramref name="action"/> on the dispatcher.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <param name="action">Action to execute.</param>
	/// <returns><see cref="Task"/>.</returns>
	public async Task RunOnDispatcher(CancellationToken ct, Func<CancellationToken, Task> action)
	{
		await this.GetService<IDispatcherScheduler>().Run(ct2 => action(ct2), ct);
	}

	/// <summary>
	/// Resolves a service of type <typeparamref name="TService"/> from the service provider.
	/// </summary>
	/// <typeparam name="TService">The type of service to resolve.</typeparam>
	/// <param name="service">The service variable in which to return the resolved service.</param>
	protected void ResolveService<TService>(out TService service)
	{
		service = this.GetService<TService>();
	}
}
