using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace ApplicationTemplate;

/// <summary>
/// Invokes calls to an <see cref="ILauncherService"/> on the <see cref="DispatcherQueue"/>.
/// </summary>
public class DispatcherLauncherServiceDecorator : ILauncherService
{
	private readonly ILauncherService _launcherService;
	private readonly DispatcherQueue _dispatcherQueue;

	public DispatcherLauncherServiceDecorator(ILauncherService launcherService, DispatcherQueue dispatcherQueue)
	{
		_launcherService = launcherService;
		_dispatcherQueue = dispatcherQueue;
	}

	public async Task Launch(string uri)
	{
		await Launch(new Uri(uri));
	}

	public async Task Launch(Uri uri)
	{
		await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, () => _launcherService.Launch(uri));
	}

	/// <summary>
	/// This method allows for executing an async Task with result on the CoreDispatcher.
	/// </summary>
	private async Task<TResult> DispatcherRunTaskAsync<TResult>(DispatcherQueuePriority priority, Func<Task<TResult>> asyncFunc)
	{
		var completion = new TaskCompletionSource<TResult>();
		await _dispatcherQueue.RunAsync(priority, RunActionUI);
		return await completion.Task;

		async void RunActionUI()
		{
			try
			{
				var result = await asyncFunc();
				completion.SetResult(result);
			}
			catch (Exception exception)
			{
				completion.SetException(exception);
			}
		}
	}

	/// <summary>
	/// This method allows for executing an async Task without result on the CoreDispatcher.
	/// </summary>
	private async Task DispatcherRunTaskAsync(DispatcherQueuePriority priority, Func<Task> asyncFunc)
	{
		var completion = new TaskCompletionSource<Unit>();
		await _dispatcherQueue.RunAsync(priority, RunActionUI);
		await completion.Task;

		async void RunActionUI()
		{
			try
			{
				await asyncFunc();
				completion.SetResult(Unit.Default);
			}
			catch (Exception exception)
			{
				completion.SetException(exception);
			}
		}
	}
}
