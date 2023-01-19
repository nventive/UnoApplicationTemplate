using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace ApplicationTemplate;

public sealed class LauncherService : ILauncherService
{
	private readonly DispatcherQueue _dispatcherQueue;

	public LauncherService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue;
	}

	public async Task Launch(Uri uri)
	{
		var launchSucceeded = await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, async () => await Windows.System.Launcher.LaunchUriAsync(uri));
		if (!launchSucceeded)
		{
			throw new LaunchFailedException($"Failed to launch URI: {uri}");
		}
	}

	/// <summary>
	/// This method allows for executing an async Task with result on the <see cref="DispatcherQueue"/>.
	/// </summary>
	/// <typeparam name="TResult">The <see cref="DispatcherQueue"/> result type.</typeparam>
	/// <param name="dispatcherQueuePriority"><see cref="DispatcherQueuePriority"/>.</param>
	/// <param name="asyncFunc">A function that will be execute on the <see cref="DispatcherQueue"/>.</param>
	/// <returns><see cref="Task"/> of <typeparamref name="TResult"/>.</returns>
	private async Task<TResult> DispatcherRunTaskAsync<TResult>(DispatcherQueuePriority dispatcherQueuePriority, Func<Task<TResult>> asyncFunc)
	{
		var completion = new TaskCompletionSource<TResult>();
		await _dispatcherQueue.RunAsync(dispatcherQueuePriority, RunActionUI);
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
				completion.SetException(new LaunchFailedException("An error occured while trying to launch an URI on the UI thread.", exception));
			}
		}
	}
}
